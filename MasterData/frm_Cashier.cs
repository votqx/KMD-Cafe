using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CafePOS.DBA;
using System.Windows.Forms;
using CafePOS.MasterData;

namespace CafePOS.MasterData
{
    public partial class frm_Cashier : Form
    {
        clsCashier obj_clsCashier = new clsCashier();
        clsMainDB obj_clsMainDB = new clsMainDB();

        frmMain obj_frmMain = new frmMain();

        DataTable DT = new DataTable();
        public bool _IsEdit = false;
        string SPString = "";
        public string CashierLevel = "";
        public int CashierID = 0;
        public frm_Cashier()
        {
            InitializeComponent();
        }
        private void ShowUserLevel()
        {
            chkCashierLevel.Items.Clear();
            for (int i = 1; i < obj_frmMain.menuStrip1.Items.Count; i++)
            {
                ToolStripMenuItem mnuMain = (ToolStripMenuItem)obj_frmMain.menuStrip1.Items[i];
                foreach (ToolStripItem mnuSub in mnuMain.DropDownItems)
                {
                    chkCashierLevel.Items.Add(mnuSub.Text.ToString());
                }
            }
            if (_IsEdit)
            {
                string[] Arr_CashierLevel = CashierLevel.Split(',');
                for (int i = 0; i < chkCashierLevel.Items.Count; i++)
                {
                    for (int j = 0; j < Arr_CashierLevel.Length; j++)
                    {
                        if (chkCashierLevel.Items[i].ToString() == Arr_CashierLevel[j].ToString())
                        {
                            chkCashierLevel.SetItemChecked(i, true);
                        }
                    }
                }
            }
        }

        private void frm_Cashier_Load(object sender, EventArgs e)
        {
            ShowUserLevel();
            string Day = string.Format("{0:D2}", DateTime.Now.Day);
            string Month = string.Format("{0:D2}", DateTime.Now.Month);
            string Year = string.Format("{0:D2}", DateTime.Now.Year);
            lblUpdateDate.Text = Month + "/" + Day + "/" + Year;
            txtCashierName.Focus();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            CashierLevel = string.Empty;
            foreach (object itemChecked in chkCashierLevel.CheckedItems)
            {
                CashierLevel = CashierLevel + itemChecked.ToString() + ",";
            }
            if (txtCashierName.Text.Trim().ToString() == string.Empty)
            {
                MessageBox.Show("Please Type CashierName: ");
                txtCashierName.Focus();
            }
            else if (txtPassword.Text.Trim().ToString() == string.Empty)
            {
                MessageBox.Show("Please Type Password: ");
                txtPassword.Focus();
            }
            else if (txtConfirmPassword.Text.Trim().ToString() == string.Empty)
            {
                MessageBox.Show("Please Type ConfirmPassword: ");
                txtConfirmPassword.Focus();
            }
            else if (txtPassword.Text.Trim().ToString() != txtConfirmPassword.Text.Trim().ToString())
            {
                MessageBox.Show("Password and Confirm Password should be the same.");
                txtConfirmPassword.Focus();
                txtConfirmPassword.SelectAll();
            }
            else if (CashierLevel.ToString() == string.Empty)
            {
                MessageBox.Show("Please Choose Cashier Level: ");
            }
            else
            {                                      //@para1     @para2    @action           @para1=keyboad        @para2  @action
                SPString = string.Format("SP_Select_Cashier N'{0}',N'{1}',N'{2}'", txtCashierName.Text.Trim().ToString(), txtPassword.Text.Trim().ToString(), "1");
                DT = obj_clsMainDB.SelectData(SPString);
                //0>0 F         0 != 1 T
                if ((DT.Rows.Count > 0) && (CashierID != Convert.ToInt32(DT.Rows[0]["CashierID"].ToString())))
                {
                    MessageBox.Show("This Cashier Is Already Exist");
                    txtCashierName.Focus();
                    txtCashierName.SelectAll();
                }
                else
                {
                    obj_clsCashier.CNAME = txtCashierName.Text.Trim().ToString();
                    obj_clsCashier.PASS = txtPassword.Text.Trim().ToString();//Keyboard
                    obj_clsCashier.CASHIERLEVEL = CashierLevel;//0
                    obj_clsCashier.UPDATE = lblUpdateDate.Text;//12/23/2023
                    if (_IsEdit)// _IsEdit==true
                    {
                        obj_clsCashier.CASHIERID = CashierID;
                        obj_clsCashier.ACTION = 1;
                        obj_clsCashier.SaveData();
                        MessageBox.Show("Successfully Edit", "Successfully", MessageBoxButtons.OK);
                        this.Close();
                    }
                    else
                    {
                        obj_clsCashier.ACTION = 0;
                        obj_clsCashier.SaveData();
                        MessageBox.Show("Successfully Save", "Successfully", MessageBoxButtons.OK);
                        this.Close();
                    }
                }
            }
        }
    }
}
        
