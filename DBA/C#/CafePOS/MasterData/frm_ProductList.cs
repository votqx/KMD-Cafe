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
    public partial class frm_ProductList : Form
    {
        public frm_ProductList()
        {
            InitializeComponent();
        }
        clsProduct obj_clsProduct = new clsProduct();
        clsMainDB obj_clsMainDB = new clsMainDB();
        string SPString = "";
        DataTable DT = new DataTable();

        private void ShowData()
        {
            SPString = string.Format("SP_Select_Product N'{0}',N'{1}',N'{2}'", "0", "0", "0");
            dgvProduct.DataSource = obj_clsMainDB.SelectData(SPString);

            dgvProduct.Columns[0].Width = (dgvProduct.Width / 100) * 10;
            dgvProduct.Columns[1].Visible = false;
            dgvProduct.Columns[2].Width = (dgvProduct.Width / 100) * 40;
            dgvProduct.Columns[3].Width = (dgvProduct.Width / 100) * 10;
            dgvProduct.Columns[4].Width = (dgvProduct.Width / 100) * 10;
            dgvProduct.Columns[5].Width = (dgvProduct.Width / 100) * 30;

            obj_clsMainDB.ToolStripTextBoxData(ref tstSearchWith, SPString, "ProductName");
            tslLabel.Text = "Product Name";
        }
        private void ShowEntry()
        {
            if(dgvProduct.CurrentRow.Cells[0].Value.ToString()==string.Empty)
            {
                MessageBox.Show("There is No Data!!!");
            }
            else
            {
                frm_Product frm = new frm_Product();
                frm._ProductID = Convert.ToInt32(dgvProduct.CurrentRow.Cells["ProductID"].Value.ToString());
                frm.txtProductName.Text = dgvProduct.CurrentRow.Cells["ProductName"].Value.ToString();
                frm.txtQty.Text = dgvProduct.CurrentRow.Cells["Qty"].Value.ToString();
                frm.txtPrice.Text = dgvProduct.CurrentRow.Cells["Price"].Value.ToString();

                frm._IsEdit = true;
                frm.ShowDialog();
                ShowData();
            }
        }

        private void frm_ProductList_Load(object sender, EventArgs e)
        {
            ShowData();
        }

        private void tsbNew_Click(object sender, EventArgs e)
        {
            frm_Product frm = new frm_Product();
            frm.ShowDialog();
            ShowData();
        }

        private void dgvProduct_DoubleClick(object sender, EventArgs e)
        {
            ShowEntry();
        }

        private void tsbEdit_Click(object sender, EventArgs e)
        {
            ShowEntry();
        }

        private void tsbDelete_Click(object sender, EventArgs e)
        {
            string ProductID = dgvProduct.CurrentRow.Cells["ProductID"].Value.ToString();
            if(ProductID == string.Empty)
            {
                MessageBox.Show("There is No Data.");
            }
            else if(dgvProduct.CurrentRow.Cells["Qty"].Value.ToString()!= "0")
            {
                MessageBox.Show("This Product has a Quantity. Cannot be Deleted!!!");
            }
            else
            {
                if(MessageBox.Show("Are you sure you want to Delete?","Confirm",MessageBoxButtons.YesNo)==DialogResult.Yes)
                {
                    obj_clsProduct.PRODUCTID = Convert.ToInt32(ProductID);
                    obj_clsProduct.ACTION = 2;
                    obj_clsProduct.SaveData();
                    MessageBox.Show("Successfully Deleted");
                    ShowData();
                }
            }
        }

        private void tsmProductName_Click(object sender, EventArgs e)
        {
            tslLabel.Text = "ProductName";
            SPString = string.Format("SP_Select_Product N'{0}',N'{1}',N'{2}'", "0", "0", "0");
            obj_clsMainDB.ToolStripTextBoxData(ref tstSearchWith, SPString, "ProductName");
        }

        private void tsmQty_Click(object sender, EventArgs e)
        {
            tslLabel.Text = "Quantity";
            SPString = string.Format("SP_Select_Product N'{0}',N'{1}',N'{2}'", "0", "0", "0");
            obj_clsMainDB.ToolStripTextBoxData(ref tstSearchWith, SPString, "Qty");
        }

        private void tsmPrice_Click(object sender, EventArgs e)
        {
            tslLabel.Text = "Price";
            SPString = string.Format("SP_Select_Product N'{0}',N'{1}',N'{2}'", "0", "0", "0");
            obj_clsMainDB.ToolStripTextBoxData(ref tstSearchWith, SPString, "Price");
        }

        private void tstSearchWith_TextChanged(object sender, EventArgs e)
        {
            if(tslLabel.Text == "ProductName")
            {
                SPString = string.Format("SP_Select_Product N'{0}', N'{1}', N'{2}'", tstSearchWith.Text.Trim().ToString(), "0", "2");
            }
            else if (tslLabel.Text == "Quantity")
            {
                SPString = string.Format("SP_Select_Product N'{0}', N'{1}', N'{2}'", tstSearchWith.Text.Trim().ToString(), "0", "3");
            }
            else if (tslLabel.Text == "Price")
            {
                SPString = string.Format("SP_Select_Product N'{0}', N'{1}', N'{2}'", tstSearchWith.Text.Trim().ToString(), "0", "4");
            }
            dgvProduct.DataSource = obj_clsMainDB.SelectData(SPString);
        }

        private void tsbPrint_Click(object sender, EventArgs e)
        {
            if(dgvProduct.Rows.Count > 1)
            {
                DataTable DT = new DataTable();
                DT = obj_clsMainDB.SelectData(SPString);
                frm_Report frmReport = new frm_Report();
                crpt_Product crpt = new crpt_Product();
                crpt.SetDataSource(DT);
                frmReport.crystalReportViewer1.ReportSource = crpt;
                frmReport.ShowDialog();
                ShowData();
            }
            else
            {
                MessageBox.Show("There is No Data!!!");
            }
        }
    }
}
