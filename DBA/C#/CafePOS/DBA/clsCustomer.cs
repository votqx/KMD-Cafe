using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;

namespace CafePOS.DBA
{
    class clsCustomer
    {
        public int CUSID{ get; set; }
        public string CUSCODE { get; set; }
        public string MONTH { get; set; }
        public string UPDATE { get; set; }
        public int ACTION { get; set; }

        clsMainDB obj_clsMainDB = new clsMainDB();

        public void SaveData()
        {
            try
            {
                obj_clsMainDB.DataBaseConn();
                SqlCommand sql = new SqlCommand("SP_Insert_Customer", obj_clsMainDB.con);

                sql.CommandType = CommandType.StoredProcedure;
                sql.Parameters.AddWithValue("@CustomerID", CUSID);
                sql.Parameters.AddWithValue("@CustomerCode", CUSCODE);
                sql.Parameters.AddWithValue("@BirthMonth", MONTH);
                sql.Parameters.AddWithValue("@UpdateDate", UPDATE);
                sql.Parameters.AddWithValue("@action", ACTION);
                sql.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error In Saving Data!!!");
            }
            finally
            {
                obj_clsMainDB.con.Close();
            }
        }
    }
}
