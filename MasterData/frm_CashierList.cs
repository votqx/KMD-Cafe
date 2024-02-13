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
using CafePOS.MasterData;
using CafePOS.Report;

namespace CafePOS.MasterData
{
    public partial class frm_CashierList : Form
    {
        public frm_CashierList()
        {
            InitializeComponent();
        }
        clsCashier obj_clsCashier = new clsCashier();
        clsMainDB obj_clsMainDB = new clsMainDB();
        frm_Cashier frm = new frm_Cashier();
        string SPString = "";

        private void ShowData()
        {
            SPString = string.Format("SP_Select_Cashier N'{0}',N'{1}',N'{2}'", "0", "0", "0");
            dgvCashier.DataSource = obj_clsMainDB.SelectData(SPString);
            dgvCashier.Columns[0].Width = (dgvCashier.Width / 100) * 10;
            dgvCashier.Columns[1].Visible = false;
            dgvCashier.Columns[2].Width = (dgvCashier.Width / 100) * 30;
            dgvCashier.Columns[3].Visible = false;
            dgvCashier.Columns[4].Width = (dgvCashier.Width / 100) * 60;
            

            obj_clsMainDB.ToolStripTextBoxData(ref tstSearchWith, SPString, "CashierName");
        }

        private void frm_CashierList_Load(object sender, EventArgs e)
        {
            ShowData();
        }

        private void ShowEntry()
        {
            if (dgvCashier.CurrentRow.Cells[0].Value.ToString() == string.Empty)
            {
                MessageBox.Show("There Is No Data");
            }
            else
            {
                frm.CashierID = Convert.ToInt32(dgvCashier.CurrentRow.Cells["CashierID"].Value.ToString());//6
                frm.txtCashierName.Text = dgvCashier.CurrentRow.Cells["CashierName"].Value.ToString();
                frm.txtPassword.Text = dgvCashier.CurrentRow.Cells["Password"].Value.ToString();
                frm.txtConfirmPassword.Text = dgvCashier.CurrentRow.Cells["Password"].Value.ToString();
                frm.CashierLevel = dgvCashier.CurrentRow.Cells["CashierLevel"].Value.ToString();
                frm._IsEdit = true;
                frm.ShowDialog();
                ShowData();
            }
        }

        private void tsbNew_Click(object sender, EventArgs e)
        {
            frm_Cashier frm = new frm_Cashier();
            frm.ShowDialog();
            ShowData();
        }

        private void tsbEdit_Click(object sender, EventArgs e)
        {
            ShowEntry();
        }

        private void tsbDelete_Click(object sender, EventArgs e)
        {
            if (dgvCashier.CurrentRow.Cells[0].Value.ToString() == string.Empty)
            {
                MessageBox.Show("There Is No Data");
            }
            else
            {
                if (MessageBox.Show("Are You Sure You Want To Delete?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    obj_clsCashier.CASHIERID = Convert.ToInt32(dgvCashier.CurrentRow.Cells["CashierID"].Value.ToString());
                    obj_clsCashier.ACTION = 2;
                    obj_clsCashier.SaveData();
                    MessageBox.Show("Successfully Delete");
                    ShowData();
                }
            }
        }

        private void tstSearchWith_TextChanged(object sender, EventArgs e)
        {
            SPString = string.Format("SP_Select_Cashier N'{0}',N'{1}',N'{2}'", tstSearchWith.Text.Trim().ToString(), "0", "2");
            dgvCashier.DataSource = obj_clsMainDB.SelectData(SPString);
        }

        private void tsbPrint_Click(object sender, EventArgs e)
        {
            if (dgvCashier.Rows.Count > 0)
            {
                DataTable DT = new DataTable();
                DT = obj_clsMainDB.SelectData(SPString);
                frm_Report frmReport = new frm_Report();
                crpt_Casher crpt = new crpt_Casher();
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

        private void dgvCashier_DoubleClick(object sender, EventArgs e)
        {
            ShowEntry();
        }
    }
}
