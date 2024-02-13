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
    public partial class frm_OrderList : Form
    {
        public frm_OrderList()
        {
            InitializeComponent();
        }
        clsOrder obj_clsOrder = new clsOrder();
        clsOrder_Detail obj_clsOrderDetail = new clsOrder_Detail();
        clsMainDB obj_clsMainDB = new clsMainDB();
        UserControl OrderDetail;

        string SPString = "";
        private void ShowOrder()
        {
            DataGridViewTextBoxColumn DGCol = new DataGridViewTextBoxColumn();
            DGCol.DefaultCellStyle.NullValue = "+";
            DGCol.HeaderText = "";
            DGCol.Width = 30;
            DGCol.ReadOnly = true;
            DGCol.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvOrder.Columns.Add(DGCol);

            SPString = string.Format("SP_Select_cOrder N'{0}',N'{1}',N'{2}'", "0", "0", "0");
            dgvOrder.DataSource = obj_clsMainDB.SelectData(SPString);

            dgvOrder.Columns[1].Width = (dgvOrder.Width / 100) * 5;    //no
            dgvOrder.Columns[2].Visible = false;                        //orderid
            dgvOrder.Columns[3].Width = (dgvOrder.Width / 100) * 20;    //bill
            dgvOrder.Columns[4].Width = (dgvOrder.Width / 100) * 10;    //orderdate
            dgvOrder.Columns[5].Visible = false;                        //cashierid
            dgvOrder.Columns[6].Width = (dgvOrder.Width / 100) * 15;    //cashiername
            dgvOrder.Columns[7].Width = (dgvOrder.Width / 100) * 10;    //totalamount
            dgvOrder.Columns[8].Width = (dgvOrder.Width / 100) * 10;    //tax
            dgvOrder.Columns[9].Width = (dgvOrder.Width / 100) * 10;    //discount
            dgvOrder.Columns[10].Width = (dgvOrder.Width / 100) * 10;    //grandtotal
            dgvOrder.Columns[11].Visible = false;                        //cashierid
            dgvOrder.Columns[12].Width = (dgvOrder.Width / 100) * 15;    //ccode
            obj_clsMainDB.ToolStripTextBoxData(ref tstSearchWith, SPString, "OrderDate");
        }
        private void ShowOrderDetail()
        {
            OrderDetail = new ctl_OrderDetail();
            OrderDetail.Hide();
            Controls.Add(OrderDetail);
            Controls.SetChildIndex(OrderDetail, 0);
        }

        private void frm_OrderList_Load(object sender, EventArgs e)
        {
            ShowOrder();
            ShowOrderDetail();
        }

        private void dgvOrder_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                if (dgvOrder[e.ColumnIndex, e.RowIndex].Value == null)
                    dgvOrder[e.ColumnIndex, e.RowIndex].Value = "+";

                if (dgvOrder[e.ColumnIndex, e.RowIndex].Value.ToString().Trim() == "+")
                {
                    Rectangle cellBounds = dgvOrder.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);
                    Point offsetLocation = new Point(cellBounds.X, cellBounds.Y + cellBounds.Height);
                    offsetLocation.Offset(dgvOrder.Location);
                    OrderDetail.Location = offsetLocation;
                    int OrderID = (Convert.ToInt32(dgvOrder.CurrentRow.Cells["OrderID"].Value.ToString()));

                    DataGridView DGV = ((CafePOS.Order.ctl_OrderDetail)(OrderDetail)).dgvOrderDetail;
                    SPString = string.Format("SP_Select_cOrder_Detail N'{0}',N'{1}',N'{2}'", OrderID, "0", "0");
                    DGV.DataSource = obj_clsMainDB.SelectData(SPString);

                    DGV.Columns[0].Visible = false;                 //orderid
                    DGV.Columns[1].Visible = false;                 //productid
                    DGV.Columns[2].Width = (DGV.Width / 100) * 50;  //pname
                    DGV.Columns[3].Width = (DGV.Width / 100) * 20;  //qtysold
                    DGV.Columns[4].Width = (DGV.Width / 100) * 20;  //pricesold

                    OrderDetail.Show();
                    dgvOrder[e.ColumnIndex, e.RowIndex].Value = "-";
                }
                else
                {
                    OrderDetail.Hide();
                    dgvOrder[e.ColumnIndex, e.RowIndex].Value = "+";
                }
            }
        }

        private void tsbNew_Click(object sender, EventArgs e)
        {
            frm_Order frm = new frm_Order();
            frm.ShowDialog();
            SPString = string.Format("SP_Select_cOrder N'{0}', N'{1}', N'{2}'", "0", "0", "0");
            dgvOrder.DataSource = obj_clsMainDB.SelectData(SPString);
        }

        private void tsmDate_Click(object sender, EventArgs e)
        {
            tslLabel.Text = "OrderDate";
            SPString = string.Format("SP_Select_cOrder N'{0}', N'{1}', N'{2}'", "0", "0", "0");
            obj_clsMainDB.ToolStripTextBoxData(ref tstSearchWith, SPString, "OrderDate");
        }

        private void tsmCashierName_Click(object sender, EventArgs e)
        {
            tslLabel.Text = "CashierName";
            SPString = string.Format("SP_Select_cOrder N'{0}', N'{1}', N'{2}'", "0", "0", "0");
            obj_clsMainDB.ToolStripTextBoxData(ref tstSearchWith, SPString, "CashierName");
        }

        private void tstSearchWith_TextChanged(object sender, EventArgs e)
        {
            if (tslLabel.Text == "OrderDate")
            {                                                                  //m
                SPString = string.Format("SP_Select_cOrder N'{0}',N'{1}',N'{2}'", tstSearchWith.Text.Trim().ToString(), "0", "4");
            }

            else if (tslLabel.Text == "CashierName")
            {                                                                     //100
                SPString = string.Format("SP_Select_cOrder N'{0}',N'{1}',N'{2}'", tstSearchWith.Text.Trim().ToString(), "0", "5");
            }
            dgvOrder.DataSource = obj_clsMainDB.SelectData(SPString);
        }

        private void tsbPrint_Click(object sender, EventArgs e)
        {
            if (dgvOrder.Rows.Count > 0)
            {
                DataTable DT = new DataTable();
                if (tslLabel.Text == "OrderDate")
                {
                    SPString = string.Format("SP_Select_cOrderReport N'{0}',N'{1}',N'{2}'", tstSearchWith.Text.Trim().ToString(), "0", "1");
                }

                else if (tslLabel.Text == "CashierName")
                {
                    SPString = string.Format("SP_Select_cOrderReport N'{0}',N'{1}',N'{2}'", tstSearchWith.Text.Trim().ToString(), "0", "2");
                }
                DT = obj_clsMainDB.SelectData(SPString);
                frm_Report frmReport = new frm_Report();
                crpt_Order crpt = new crpt_Order();
                crpt.SetDataSource(DT);
                frmReport.crystalReportViewer1.ReportSource = crpt;
                frmReport.ShowDialog();

                SPString = string.Format("SP_Select_cOrder N'{0}', N'{1}', N'{2}'", "0", "0", "0");
                dgvOrder.DataSource = obj_clsMainDB.SelectData(SPString);
                tstSearchWith.Text = " ";
            }
            else
            {
                MessageBox.Show("There is no data.");
            }
        }
    }
}
