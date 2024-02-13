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

namespace CafePOS.MasterData
{
    public partial class frm_Customer : Form
    {
        public frm_Customer()
        {
            InitializeComponent();
        }
        clsCustomer obj_clsCustomer = new clsCustomer();
        clsMainDB obj_clsMainDB = new clsMainDB();

        DataTable DT = new DataTable();
        public bool _IsEdit = false;
        public int _CusID = 0;
        string SPString = "";

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void frm_Customer_Load(object sender, EventArgs e)
        {
            string Day = string.Format("{0:D2}", DateTime.Now.Day);
            string Month = string.Format("{0:D2}", DateTime.Now.Month);
            string Year = string.Format("{0:D2}", DateTime.Now.Year);
            lblUpdateDate.Text = Month + "/" + Day + "/" + Year;
            cboMonth.SelectedItem = "January";

            SPString = String.Format("SP_Select_Customer N'{0}',N'{1}',N'{2}'", "0", "0", "5");
            string _CusCode = obj_clsMainDB.GetCode(SPString);
            lblCustomerCode.Text = _CusCode;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SPString = String.Format("SP_Select_Customer N'{0}',N'{1}',N'{2}'", "0", "0", "5");
            string _CusCode = obj_clsMainDB.GetCode(SPString);

            SPString = string.Format("SP_Select_Customer N'{0}',N'{1}',N'{2}'", _CusCode, "0", "1");
            DT = obj_clsMainDB.SelectData(SPString);
            if (DT.Rows.Count > 0 && _CusID != Convert.ToInt32(DT.Rows[0]["CustomerID"]))
            {
                MessageBox.Show("This Customer Already Exits!!");
            }
            else
            {
                obj_clsCustomer.CUSID = _CusID;
                obj_clsCustomer.CUSCODE = lblCustomerCode.Text;
                obj_clsCustomer.MONTH = Convert.ToString(cboMonth.SelectedItem);
                obj_clsCustomer.UPDATE = lblUpdateDate.Text;
                if (_IsEdit)
                {
                    obj_clsCustomer.ACTION = 1;
                    obj_clsCustomer.SaveData();
                    MessageBox.Show("Successfuly Edited!", "Successfully", MessageBoxButtons.OK);
                    this.Close();
                }
                else
                {
                    obj_clsCustomer.ACTION = 0;
                    obj_clsCustomer.SaveData();
                    MessageBox.Show("Successfuly Saved!", "Successfully", MessageBoxButtons.OK);
                    this.Close();
                }
            }
        }
    }
}
