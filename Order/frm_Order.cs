using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using CafePOS.DBA;
using CafePOS.Report;

namespace CafePOS.Order
{
    public partial class frm_Order : Form
    {
        public frm_Order()
        {
            InitializeComponent();
        }
        clsOrder obj_clsOrder = new clsOrder();
        clsOrder_Detail obj_clsOrderDetail = new clsOrder_Detail();
        clsCustomer obj_clsCustomer = new clsCustomer();
        clsProduct obj_clsProduct = new clsProduct();
        clsMainDB obj_clsMainDB = new clsMainDB();

        DataTable DT = new DataTable();
        DataTable DTOrder = new DataTable();
        string SPString = "";
        string BirthMonth = "";
        int _OrderID = 0;

        private void CreateTable()
        {
            DTOrder.Rows.Clear();
            DTOrder.Columns.Clear();

            DTOrder.Columns.Add("ProductID");
            DTOrder.Columns.Add("ProductName");
            DTOrder.Columns.Add("QtyOnHand");
            DTOrder.Columns.Add("PriceSold");
            DTOrder.Columns.Add("QtySold");
            DTOrder.Columns.Add("Total");
            dgvOrder.DataSource = DTOrder;

            dgvOrder.Columns[0].Visible = false;
            dgvOrder.Columns[1].Width = (dgvOrder.Width / 100) * 30;
            dgvOrder.Columns[2].Width = (dgvOrder.Width / 100) * 13;
            dgvOrder.Columns[2].ReadOnly = true;
            dgvOrder.Columns[3].Width = (dgvOrder.Width / 100) * 24;
            dgvOrder.Columns[3].ReadOnly = true;
            dgvOrder.Columns[4].Width = (dgvOrder.Width / 100) * 10;
            dgvOrder.Columns[5].Width = (dgvOrder.Width / 100) * 26;
            dgvOrder.Columns[5].ReadOnly = true;
        }
        private void CalculateTotal()
        {
            int GrandTotal = 0;
            int TotalSaleQty = 0;

            for (int i = 0; i < dgvOrder.Rows.Count - 1; i++)
            {
                DataGridViewRow DR = dgvOrder.Rows[i];
                int QtySold = Convert.ToInt32(DR.Cells["QtySold"].Value.ToString());
                int PriceSold = Convert.ToInt32(DR.Cells["PriceSold"].Value.ToString());
                int Total = QtySold * PriceSold;

                DR.Cells["Total"].Value = Total.ToString();

                GrandTotal += Total;
                TotalSaleQty += QtySold;
            }
            lblTotalAmount.Text = GrandTotal.ToString();
            lblTax.Text = (TotalSaleQty * 50).ToString();

            if (cboCustomerCode.Text.Trim().ToString() == string.Empty)
            {
                MessageBox.Show("Please Choose CustomerCode");
                cboCustomerCode.Focus();
            }

            SPString = string.Format("SP_Select_Customer N'{0}',N'{1}',N'{2}'", cboCustomerCode.Text, "0", "4");
            DT = obj_clsMainDB.SelectData(SPString);
            BirthMonth = DT.Rows[0]["BirthMonth"].ToString();
            string CurrentMonth = DateTime.Now.ToString("MMMM");
            if (DT.Rows.Count>0)
            {
                if (BirthMonth == CurrentMonth )
                {
                    lblDiscount.Text = ((GrandTotal / 100) * 20).ToString();
                }
                else if(BirthMonth != CurrentMonth || BirthMonth == "N/A" )
                {
                    lblDiscount.Text = "0";
                }
            }
            
            lblGrandTotal.Text = (Convert.ToInt32(lblTotalAmount.Text) + Convert.ToInt32(lblTax.Text) - Convert.ToInt32(lblDiscount.Text)).ToString();
            txtPayment.Text = "0";
            lblRefund.Text = "0";

        }
        private void frm_Order_Load(object sender, EventArgs e)
        {
            CreateTable();

            SPString = string.Format("SP_Select_Customer N'{0}',N'{1}',N'{2}'", "0", "0", "0");
            obj_clsMainDB.AddCombo(ref cboCustomerCode, SPString, "CustomerCode", "CustomerID");

            SPString = String.Format("SP_Select_cOrder N'{0}',N'{1}',N'{2}'",dtpDate.Value.ToShortDateString(), "0","1");
            lblBill.Text = obj_clsMainDB.GetBill(SPString, dtpDate.Value.ToShortDateString());
        }

        private void mnuNew_Click(object sender, EventArgs e)
        {
            CreateTable();
            lblTotalAmount.Text = "";
            lblTax.Text = "";
            lblDiscount.Text = "";
            lblGrandTotal.Text = "";
            txtPayment.Text = "";
            lblRefund.Text = "";
            dgvOrder.Focus();
        }

        private void mnuPayment_Click(object sender, EventArgs e)
        {
            if (lblGrandTotal.Text == "")
            {
                MessageBox.Show("There is No Product Record.");
                dgvOrder.Focus();
            }
            else
            {
                txtPayment.Text = "";
                txtPayment.Focus();
            }
        }

        private void mnuSave_Click(object sender, EventArgs e)
        {
            if (dgvOrder.Rows.Count <= 1)
            {
                MessageBox.Show("There is No Product Record.");
                dgvOrder.Focus();
            }
            else if (lblRefund.Text == "" || lblRefund.Text == "0")
            {
                MessageBox.Show("Please Check Payment");
                txtPayment.Focus();
            }
            else
            {
                obj_clsOrder.BILL = lblBill.Text;
                obj_clsOrder.ODATE = dtpDate.Value.ToShortDateString();
                obj_clsOrder.TOTALAMT = Convert.ToInt32(lblTotalAmount.Text);
                obj_clsOrder.TAX = Convert.ToInt32(lblTax.Text);
                obj_clsOrder.DISCOUNT = Convert.ToInt32(lblDiscount.Text);
                obj_clsOrder.GRANDTOTAL = Convert.ToInt32(lblGrandTotal.Text);
                obj_clsOrder.CASHIERID = Program.CashierID;
                obj_clsOrder.CUSTOMERID = Convert.ToInt32(cboCustomerCode.SelectedIndex);
                obj_clsOrder.ACTION = 0;
                obj_clsOrder.SaveData();

                SPString = string.Format("SP_Select_cOrder N'{0}',N'{1}',N'{2}'", "0", "0", "3");
                DT = obj_clsMainDB.SelectData(SPString);
                _OrderID = Convert.ToInt32(DT.Rows[0]["OrderID"].ToString());

                for (int i = 0; i < DTOrder.Rows.Count; i++)
                {
                    obj_clsOrderDetail.OID = _OrderID;
                    obj_clsOrderDetail.PID = Convert.ToInt32(DTOrder.Rows[i]["ProductID"].ToString());
                    obj_clsOrderDetail.SQTY = Convert.ToInt32(DTOrder.Rows[i]["QtySold"].ToString());
                    obj_clsOrderDetail.OPRICE = Convert.ToInt32(DTOrder.Rows[i]["PriceSold"].ToString());
                    //discount?
                    obj_clsOrderDetail.ACTION = 0;
                    obj_clsOrderDetail.SaveData();

                    obj_clsProduct.PRODUCTID = Convert.ToInt32(DTOrder.Rows[i]["ProductID"].ToString());
                    obj_clsProduct.QTY = Convert.ToInt32(DTOrder.Rows[i]["QtySold"].ToString());
                    obj_clsProduct.ACTION = 4;
                    obj_clsProduct.SaveData();
                }

                SPString = string.Format("SP_Select_cOrderReport N'{0}',N'{1}',N'{2}'", "0", "0", "0");
                DT = obj_clsMainDB.SelectData(SPString);
                frm_Report frmReport = new frm_Report();
                crpt_Bill crpt = new crpt_Bill();
                crpt.SetDataSource(DT);
                frmReport.crystalReportViewer1.ReportSource = crpt;
                frmReport.ShowDialog();
                MessageBox.Show("Successfully Save", "Successfully", MessageBoxButtons.OK);
                this.Close();

            }
        }

        private void dgvOrder_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            int CurRow = 0;
            int CurCol = 0;
            string ProductName = "";
            string QtySold = "0";

            CurRow = dgvOrder.CurrentCell.RowIndex;
            CurCol = dgvOrder.CurrentCell.ColumnIndex;

            ProductName = dgvOrder.Rows[CurRow].Cells["ProductName"].Value.ToString();

            if (ProductName != "")
            {
                if (CurCol - 1 == 1)
                {
                    SPString = string.Format("SP_Select_Product N'{0}',N'{1}',N'{2}'", ProductName, "0", "6");
                    DT = obj_clsMainDB.SelectData(SPString);
                    if (DT.Rows.Count <= 0)
                    {
                        MessageBox.Show("This Product Name Does Not Exist !!!");
                        SendKeys.Send("{HOME}");
                    }
                    else
                    {
                        bool AddRow = true;
                        for (int i = 0; i < dgvOrder.Rows.Count - 1; i++)
                        {
                            if (dgvOrder.Rows[i].Cells["ProductName"].Value.ToString() == ProductName && i != CurRow)
                            {
                                MessageBox.Show("This Product is Already SoldOut.");
                                AddRow = false;
                                SendKeys.Send("{HOME}");
                            }
                        }
                        if (AddRow)
                        {
                            dgvOrder.Rows[CurRow].Cells["ProductID"].Value = DT.Rows[0]["ProductID"].ToString();
                            dgvOrder.Rows[CurRow].Cells["QtyOnHand"].Value = DT.Rows[0]["Qty"].ToString();
                            dgvOrder.Rows[CurRow].Cells["PriceSold"].Value = DT.Rows[0]["Price"].ToString();
                            dgvOrder.Rows[CurRow].Cells["QtySold"].Value = "0";
                            dgvOrder.Rows[CurRow].Cells["Total"].Value = "0";
                            SendKeys.Send("{TAB}");
                            SendKeys.Send("{TAB}");
                            CalculateTotal();
                        }
                    }
                }
                if (CurCol - 1 == 4)
                {
                    dgvOrder.CurrentRow.Cells["Total"].Value = "0";

                    int OK;
                    QtySold = dgvOrder.Rows[CurRow].Cells["QtySold"].Value.ToString();
                    if (int.TryParse(QtySold, out OK) == false)
                    {
                        MessageBox.Show("Ordering Amount Should Be Number.");
                        SendKeys.Send("{HOME}");
                        SendKeys.Send("{TAB}");
                    }
                    else if (Convert.ToInt32(QtySold) <= 0 || Convert.ToInt32(QtySold) > Convert.ToInt32(dgvOrder.CurrentRow.Cells["QtyOnHand"].Value.ToString()))
                    {
                        MessageBox.Show("Ordering Amount Should Be Between 1 And " + dgvOrder.CurrentRow.Cells["QtyOnHand"].Value.ToString());
                        SendKeys.Send("{HOME}");
                        SendKeys.Send("{TAB}");
                    }
                    else
                    {
                        CalculateTotal();
                    }
                }
            }
        }

        private void txtPayment_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (lblTotalAmount.Text == "" || lblTotalAmount.Text == "0")
            {
                MessageBox.Show("There is No Item Record");
                dgvOrder.Focus();
            }
            else
            {
                int OK;
                lblRefund.Text = "0";

                if (e.KeyChar.Equals('\r'))
                {
                    if (txtPayment.Text == "")
                    {
                        MessageBox.Show("Please Type Payment !!");
                        txtPayment.Focus();
                    }
                    else if (int.TryParse(txtPayment.Text, out OK) == false)
                    {
                        MessageBox.Show("Payment Should Be A Number.");
                        txtPayment.Focus();
                        txtPayment.SelectAll();
                    }
                    else if (Convert.ToInt32(txtPayment.Text) < Convert.ToInt32(lblGrandTotal.Text))
                    {
                        MessageBox.Show("Payment Amount Should Be Above " + lblGrandTotal.Text);
                        txtPayment.Focus();
                        txtPayment.SelectAll();
                    }
                    else
                    {
                        lblRefund.Text = (Convert.ToInt32(txtPayment.Text) - Convert.ToInt32(lblGrandTotal.Text)).ToString();
                    }
                }
            }
        }

        private void dtpDate_ValueChanged(object sender, EventArgs e)
        {

            SPString = string.Format("SP_Select_cOrder N'{0}',N'{1}',N'{2}'", dtpDate.Value.ToShortDateString(), "0", "2");
            DT = obj_clsMainDB.SelectData(SPString);
            int DateDiff = Convert.ToInt32(DT.Rows[0]["No"]);
            if (DateDiff > 0)
            {
                MessageBox.Show("Please Check Order Date");
                dtpDate.Text = DateTime.Now.ToShortDateString();
            }
            else if (DateDiff <= -7)
            {
                MessageBox.Show("Please Check Order Date");
                dtpDate.Text = DateTime.Now.ToShortDateString();
            }
            else
            {
                SPString = string.Format("SP_Select_Sale N'{0}',N'{1}',N'{2}'", dtpDate.Value.ToShortDateString(), "0", "1");
                lblBill.Text = obj_clsMainDB.GetBill(SPString, dtpDate.Value.ToShortDateString());
            }
        }

        private void dgvOrder_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            TextBox txtProductName = (TextBox)e.Control;
            txtProductName.AutoCompleteCustomSource.Clear();

            int Curcol = 0;
            Curcol = dgvOrder.CurrentCell.ColumnIndex;
            if (Curcol == 1)
            {
                SPString = string.Format("SP_Select_Product N'{0}',N'{1}',N'{2}'", "0", "0", "5");
                obj_clsMainDB.TextBoxData(ref txtProductName, SPString, "ProductName");
            }
        }

        
    }
}
