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
using CafePOS.Order;

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
        public int _CusID = 0;
        string SPString = "";
        DataTable DT = new DataTable();

        UserControl OrderDetail;
        private void ShowData()
        {
            SPString = string.Format("SP_Select_Customer N'{0}',N'{1}',N'{2}'", "0", "0", "0");
            dgvCustomer.DataSource = obj_clsMainDB.SelectData(SPString);

            dgvCustomer.Columns[1].Visible = false;
            dgvCustomer.Columns[2].Width = (dgvCustomer.Width / 100) * 20;
            dgvCustomer.Columns[3].Width = (dgvCustomer.Width / 100) * 40;
            dgvCustomer.Columns[4].Width = (dgvCustomer.Width / 100) * 40;

            obj_clsMainDB.ToolStripTextBoxData(ref tstSearchWith, SPString, "CustomerID");
            tslLabel.Text = "CustomerCode";
        }
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
                frm.lblCustomerCode.Text = dgvCustomer.CurrentRow.Cells["CustomerCode"].Value.ToString();
                frm.cboMonth.SelectedItem = dgvCustomer.CurrentRow.Cells["BirthMonth"].Value.ToString();
                frm.lblUpdateDate.Text = dgvCustomer.CurrentRow.Cells["UpdateDate"].Value.ToString();

                frm._IsEdit = true;
                frm.ShowDialog();
                ShowData();
            }
        }
        
        private void NotRegister()
        {
            string Day = string.Format("{0:D2}", DateTime.Now.Day);
            string Month = string.Format("{0:D2}", DateTime.Now.Month);
            string Year = string.Format("{0:D2}", DateTime.Now.Year);
            string CurrentDateTime = Month + "/" + Day + "/" + Year;

            SPString = String.Format("SP_Select_Customer N'{0}',N'{1}',N'{2}'", "0", "0", "5");
            string _CusCode = obj_clsMainDB.GetCode(SPString);

            SPString = string.Format("SP_Select_Customer N'{0}',N'{1}',N'{2}'", _CusCode, "0", "1");
            DT = obj_clsMainDB.SelectData(SPString);
            if (DT.Rows.Count > 0 && _CusID != Convert.ToInt32(DT.Rows[0]["CustomerID"]))
            {
                MessageBox.Show("This Customer Already Exits!!");
            }

            obj_clsCustomer.CUSID = _CusID;
            obj_clsCustomer.CUSCODE = _CusCode;
            obj_clsCustomer.MONTH = "N/A";
            obj_clsCustomer.UPDATE = CurrentDateTime;

            obj_clsCustomer.ACTION = 0;
            obj_clsCustomer.SaveData();
            MessageBox.Show("Successfuly Saved!", "Successfully", MessageBoxButtons.OK);
        }
        private void frm_CustomerList_Load(object sender, EventArgs e)
        {
            if (MessageBox.Show("Would you like to enter your BirthMonth for discount opportunities?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
                ShowData();
            else
            {
                NotRegister();
                ShowData();
            }
        }

        private void tsbNew_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Would you like to enter your BirthMonth for discount opportunities?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                frm_Customer frm = new frm_Customer();
                frm.ShowDialog();
                ShowData();
            }
            else
            {
                NotRegister();
                ShowData();
            }
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

        private void tstSearchWith_TextChanged(object sender, EventArgs e)
        {
            if (tslLabel.Text == "CustomerCode")
            {
                SPString = string.Format("SP_Select_Customer N'{0}', N'{1}', N'{2}'", tstSearchWith.Text.Trim().ToString(), "0", "2");
            }
            else if (tslLabel.Text == "BirthMonth")
            {
                SPString = string.Format("SP_Select_Customer N'{0}', N'{1}', N'{2}'", tstSearchWith.Text.Trim().ToString(), "0", "3");
            }
            dgvCustomer.DataSource = obj_clsMainDB.SelectData(SPString);
        }

        private void tsbPrint_Click(object sender, EventArgs e)
        {
            if (dgvCustomer.Rows.Count > 0)
            {
                DataTable DT = new DataTable();
                DT = obj_clsMainDB.SelectData(SPString);
                frm_Report frmReport = new frm_Report();
                crpt_Customer crpt = new crpt_Customer();
                crpt.SetDataSource(DT);
                frmReport.crystalReportViewer1.ReportSource = crpt;
                frmReport.ShowDialog();
                ShowData();
            }
            else
            {
                MessageBox.Show("There is no data.");
            }
        }

    }
}
