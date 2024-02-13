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

namespace CafePOS.MasterData
{
    public partial class frm_CustomerList : Form
    {
        public frm_CustomerList()
        {
            InitializeComponent();
        }
        clsCustomer obj_clsCustomer = new clsCustomer();
        clsMainDB obj_clsMainDB = new clsMainDB();
        string SPString = "";
        DataTable DT = new DataTable();

        private void ShowData()
        {
            /*DataGridViewTextBoxColumn DGCol = new DataGridViewTextBoxColumn();
            DGCol.DefaultCellStyle.NullValue = "+";
            DGCol.HeaderText = "";
            DGCol.Width = 30;
            DGCol.ReadOnly = true;
            DGCol.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvCustomer.Columns.Add(DGCol);*/
            
            SPString = string.Format("SP_Select_Customer N'{0}',N'{1}',N'{2}'", "0", "0", "0");
            dgvCustomer.DataSource = obj_clsMainDB.SelectData(SPString);

            dgvCustomer.Columns[1].Visible = false;
            dgvCustomer.Columns[2].Width = (dgvCustomer.Width / 100) * 10;
            dgvCustomer.Columns[3].Width = (dgvCustomer.Width / 100) * 30;
            dgvCustomer.Columns[4].Width = (dgvCustomer.Width / 100) * 30;

            obj_clsMainDB.ToolStripTextBoxData(ref tstSearchWith, SPString, "CustomerID");
            tslLabel.Text = "CustomerID";
        }
        /*private void ShowOrderCtl()
        {
            OrderDetail = new ctl_OrderDetail();
            OrderDetail.Hide();
            Controls.Add(SaleDetail);
            Controls.SetChildIndex(SaleDetail, 0);
        }*/
        private void ShowEntry()
        {
            if (dgvCustomer.CurrentRow.Cells[0].Value.ToString() == string.Empty)
            {
                MessageBox.Show("There is No Data!!!");
            }
            else
            {
                frm_Customer frm = new frm_Customer();
                frm._CusID = Convert.ToInt32(dgvCustomer.CurrentRow.Cells["CustomerID"].Value.ToString());
                frm.txtCustomerCode.Text = dgvCustomer.CurrentRow.Cells["CustomerCode"].Value.ToString();
                frm.cboMonth.SelectedItem = dgvCustomer.CurrentRow.Cells["BirthMonth"].Value.ToString();
                frm.lblUpdateDate.Text = dgvCustomer.CurrentRow.Cells["UpdateDate"].Value.ToString();

                frm._IsEdit = true;
                frm.ShowDialog();
                ShowData();
            }
        }

        private void frm_CustomerList_Load(object sender, EventArgs e)
        {
            ShowData();
        }

        private void tsbNew_Click(object sender, EventArgs e)
        {
            frm_Customer frm = new frm_Customer();
            frm.ShowDialog();
            ShowData();
        }

        private void tsbEdit_Click(object sender, EventArgs e)
        {
            ShowEntry();
        }

        private void dgvCustomer_DoubleClick(object sender, EventArgs e)
        {
            ShowEntry();
        }

        private void tsbDelete_Click(object sender, EventArgs e)
        {
            string CusID = dgvCustomer.CurrentRow.Cells["CustomerID"].Value.ToString();
            if (CusID == string.Empty)
            {
                MessageBox.Show("There is No Data.");
            }
            else
            {
                if (MessageBox.Show("Are you sure you want to Delete?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    obj_clsCustomer.CUSID = Convert.ToInt32(CusID);
                    obj_clsCustomer.ACTION = 2;
                    obj_clsCustomer.SaveData();
                    MessageBox.Show("Successfully Deleted");
                    ShowData();
                }
            }
        }


        private void tsmBirthMonth_Click(object sender, EventArgs e)
        {
            tslLabel.Text = "BirthMonth";
            SPString = string.Format("SP_Select_Customer N'{0}',N'{1}',N'{2}'", "0", "0", "0");
            obj_clsMainDB.ToolStripTextBoxData(ref tstSearchWith, SPString, "BirthMonth");
        }

        private void tsmCustomerCode_Click(object sender, EventArgs e)
        {
            tslLabel.Text = "CustomerCode";
            SPString = string.Format("SP_Select_Customer N'{0}',N'{1}',N'{2}'", "0", "0", "0");
            obj_clsMainDB.ToolStripTextBoxData(ref tstSearchWith, SPString, "CustomerCode");
        }

        private void tstSearchWith_Click(object sender, EventArgs e)
        {
            if (tslLabel.Text == "CustomerCode")
            {
                SPString = string.Format("SP_Select_Product N'{0}', N'{1}', N'{2}'", tstSearchWith.Text.Trim().ToString(), "0", "2");
            }
            else if (tslLabel.Text == "BirthMonth")
            {
                SPString = string.Format("SP_Select_Product N'{0}', N'{1}', N'{2}'", tstSearchWith.Text.Trim().ToString(), "0", "3");
            }
            dgvCustomer.DataSource = obj_clsMainDB.SelectData(SPString);
        }
    }
}
