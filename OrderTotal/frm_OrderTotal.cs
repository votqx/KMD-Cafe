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

namespace CafePOS.OrderTotal
{
    public partial class frm_OrderTotal : Form
    {
        public frm_OrderTotal()
        {
            InitializeComponent();
        }


        clsMainDB obj_clsMainDB = new clsMainDB();
        DataTable DTData = new DataTable();
        DataTable DTOrder = new DataTable();
        DataTable DTProduct = new DataTable();
        DataTable DTChart = new DataTable();
        DataTable DT = new DataTable();
        DataTable DTNew = new DataTable();
        DataRow DRData;

        string SPString = "";
        int GrandSaleTotal;
        private void ShowData()
        {
            GrandSaleTotal = 0;

            SPString = string.Format("SP_Select_OrderTotal N'{0}',N'{1}',N'{2}',N'{3}'", dtpStartDate.Value.ToShortDateString(), dtpEndDate.Value.ToShortDateString(), "0", "1");
            DT = obj_clsMainDB.SelectData(SPString);
            int DateDiff = Convert.ToInt32(DT.Rows[0]["No"]);
            if (DateDiff < 0)
            {
                MessageBox.Show("Please check start date and end date");
                dtpStartDate.Text = DateTime.Now.ToShortDateString();
                dtpEndDate.Text = DateTime.Now.ToShortDateString();
                return;
            }
            SPString = String.Format("SP_Select_OrderTotal N'{0}',N'{1}',N'{2}',N'{3}'", txtProductName.Text, dtpStartDate.Value.ToShortDateString(), dtpEndDate.Value.ToShortDateString(), "0");
            DTOrder = obj_clsMainDB.SelectData(SPString);

            SPString = String.Format("SP_Select_Product N'{0}',N'{1}',N'{2}'", txtProductName.Text, "0", "2");
            DTProduct = obj_clsMainDB.SelectData(SPString);

            DTData.Rows.Clear();
            DTData.Columns.Clear();
            DTData.Columns.Add("No");
            DTData.Columns.Add("ProductName");
            DTData.Columns.Add("TotalQuantitySold");
            DTData.Columns.Add("TotalSalePrice");

            foreach (DataRow DRProduct in DTProduct.Rows)
            {
                DRData = DTData.NewRow();
                DRData["No"] = DRProduct["No"];
                DRData["ProductName"] = DRProduct["ProductName"];
                
                DataRow[] DROrder = DTOrder.Select("ProductName='" + DRData["ProductName"] + "'");
                DRData["TotalQuantitySold"] = DROrder[0]["TotalQuantitySold"];
                if (DROrder.Length > 0)
                    DRData["TotalSalePrice"] = DROrder[0]["TotalSalePrice"];
                else
                    DRData["TotalSalePrice"] = "0";

                int TotalSold = Convert.ToInt32(DRData["TotalSalePrice"]);
                GrandSaleTotal += TotalSold;
                DTData.Rows.Add(DRData);
            }
            DRData = DTData.NewRow();
            DRData["TotalQuantitySold"] = "Grand Total";
            DRData["TotalSalePrice"] = GrandSaleTotal.ToString();
            DTData.Rows.Add(DRData);

            dgvOrderTotal.DataSource = DTData;
            dgvOrderTotal.Columns[0].Width = (dgvOrderTotal.Width / 100) * 10;
            dgvOrderTotal.Columns[1].Width = (dgvOrderTotal.Width / 100) * 40;
            dgvOrderTotal.Columns[2].Width = (dgvOrderTotal.Width / 100) * 25;
            dgvOrderTotal.Columns[3].Width = (dgvOrderTotal.Width / 100) * 25;

            int LastIndex = dgvOrderTotal.Rows.Count - 1;
            dgvOrderTotal.Rows[LastIndex].DefaultCellStyle.BackColor = Color.Yellow;
        }
        private void ShowChart()
        {
            chtBar.Show();

            if(txtProductName.Text.Trim().ToString() == string.Empty)
            {
                SPString = String.Format("SP_Select_OrderTotal N'{0}',N'{1}',N'{2}',N'{3}'", "", dtpStartDate.Value.ToShortDateString(), dtpEndDate.Value.ToShortDateString(), "4");
                DTNew = obj_clsMainDB.SelectData(SPString);
            }
            else
            {
                SPString = String.Format("SP_Select_OrderTotal N'{0}',N'{1}',N'{2}',N'{3}'", txtProductName.Text, dtpStartDate.Value.ToShortDateString(), dtpEndDate.Value.ToShortDateString(), "3");
                DTNew = obj_clsMainDB.SelectData(SPString);
            }
            for (int i = 1; i<=DTNew.Rows.Count; i++)
            {
                SPString = String.Format("SP_Select_OrderTotal N'{0}',N'{1}',N'{2}',N'{3}'", i, dtpStartDate.Value.ToShortDateString(), dtpEndDate.Value.ToShortDateString(), "2");
                DTChart = obj_clsMainDB.SelectData(SPString);
                
                chtBar.Series["QtySold"].Points.AddXY(DTChart.Rows[0]["ProductName"], DTChart.Rows[0]["TotalQty"]);
            }
        }
        private void frm_OrderTotal_Load(object sender, EventArgs e)
        {
            ShowData();
            chtBar.Hide();
            SPString = String.Format("SP_Select_Product N'{0}',N'{1}',N'{2}'", "0", "0", "0");
            obj_clsMainDB.TextBoxData(ref txtProductName, SPString, "ProductName");
        }

        private void txtProductName_TextChanged(object sender, EventArgs e)
        {
            ShowData();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            ShowData();
        }

        private void btnShowAll_Click(object sender, EventArgs e)
        {
            txtProductName.Text = "";
            dtpStartDate.Text = DateTime.Now.ToShortDateString();
            dtpStartDate.Text = DateTime.Now.ToShortDateString();
            ShowData();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (dgvOrderTotal.ColumnCount > 1)
            {
                frm_Report frmReport = new frm_Report();
                crpt_OrderTotal crpt = new crpt_OrderTotal();
                crpt.SetDataSource(DTData);
                frmReport.crystalReportViewer1.ReportSource = crpt;
                frmReport.ShowDialog();
                txtProductName.Text = "";
                dtpStartDate.Text = DateTime.Now.ToLongDateString();
                dtpEndDate.Text = DateTime.Now.ToLongDateString();
                ShowData();
            }
            else
            {
                MessageBox.Show("There is no data.");
            }
        }

        private void btnChart_Click(object sender, EventArgs e)
        {
            foreach(var series in chtBar.Series)
            {
                series.Points.Clear();
            }
            ShowChart();
        }
    }
}
