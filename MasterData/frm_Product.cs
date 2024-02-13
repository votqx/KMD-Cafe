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
    public partial class frm_Product : Form
    {
        public frm_Product()
        {
            InitializeComponent();
        }
        clsProduct obj_clsProduct = new clsProduct();
        clsMainDB obj_clsMainDB = new clsMainDB();

        DataTable DT = new DataTable();
        public bool _IsEdit = false;
        public int _ProductID = 0;
        string SPString = "";

        private void frm_Product_Load(object sender, EventArgs e)
        {
            string Day = string.Format("{0:D2}", DateTime.Now.Day);
            string Month = string.Format("{0:D2}", DateTime.Now.Month);
            string Year = string.Format("{0:D2}", DateTime.Now.Year);
            lblDate.Text = Month + "/" + Day + "/" + Year;
            txtQty.Text = "0";
            txtPrice.Text = "0";
            txtProductName.Focus();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            int ok;
            if (txtProductName.Text.Trim().ToString() == string.Empty)
            {
                MessageBox.Show("Please Type the Product Name");
                txtProductName.Focus();
            }
            else if (txtQty.Text.Trim().ToString() == string.Empty)
            {
                MessageBox.Show("Please Type Quantity");
                txtQty.Focus();
            }
            else if (int.TryParse(txtQty.Text, out ok) == false)
            {
                MessageBox.Show("Quantity should be a number.");
                txtQty.Focus();
                txtQty.SelectAll();
            }
            else if (Convert.ToInt32(txtQty.Text) < 0 || Convert.ToInt32(txtQty.Text) > 100)
            {
                MessageBox.Show("Quantity should be between 0 and 100.");
                txtQty.Focus();
                txtQty.SelectAll();
            }
            else if (txtPrice.Text.Trim().ToString() == string.Empty)
            {
                MessageBox.Show("Please Type the Price");
                txtPrice.Focus();
            }
            else if (int.TryParse(txtPrice.Text, out ok) == false)
            {
                MessageBox.Show("The Price should be a number.");
                txtPrice.Focus();
                txtPrice.SelectAll();
            }
            else if (Convert.ToInt32(txtPrice.Text) != 0 && (Convert.ToInt32(txtPrice.Text) < 500 || Convert.ToInt32(txtPrice.Text) > 10000))
            {
                MessageBox.Show("The Price should be between 500 and 10000. It must not be 0.");
                txtPrice.Focus();
                txtPrice.SelectAll();
            }
            else
            {
                SPString = string.Format("SP_Select_Product N'{0}',N'{1}',N'{2}'", txtProductName.Text.Trim().ToString(), "0", "1");
                DT = obj_clsMainDB.SelectData(SPString);
                if(DT.Rows.Count >0 && _ProductID!= Convert.ToInt32(DT.Rows[0]["ProductID"]))
                {
                    MessageBox.Show("This Product Already Exits!!");
                    txtProductName.Focus();
                    txtProductName.SelectAll();
                }
                else
                {
                    obj_clsProduct.PRODUCTID = _ProductID;
                    obj_clsProduct.PRODUCTNAME = txtProductName.Text;
                    obj_clsProduct.QTY = Convert.ToInt32(txtQty.Text);
                    obj_clsProduct.PRICE = Convert.ToInt32(txtPrice.Text);
                    obj_clsProduct.UPDATE = lblDate.Text;
                    if(_IsEdit)
                    {
                        obj_clsProduct.ACTION = 1;
                        obj_clsProduct.SaveData();
                        MessageBox.Show("Successfuly Edited!", "Successfully", MessageBoxButtons.OK);
                        this.Close();
                    }
                    else
                    {
                        obj_clsProduct.ACTION = 0;
                        obj_clsProduct.SaveData();
                        MessageBox.Show("Successfuly Saved!", "Successfully", MessageBoxButtons.OK);
                        this.Close();
                    }
                }
            }
        }
    }
}
