﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;

namespace CafePOS.DBA
{
    class clsMainDB
    {
        public SqlConnection con;
        DataSet DS = new DataSet();

        public void DataBaseConn()
        {
            try
            {
                con = new SqlConnection(CafePOS.Properties.Settings.Default.POSCon);

                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                con.Open();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error In DataBaseConn");
            }

        }

        public DataTable SelectData(string SPString)
        {
            DataTable DT = new DataTable();
            try
            {
                DataBaseConn();
                SqlDataAdapter Adpt = new SqlDataAdapter(SPString, con);
                Adpt.Fill(DT);
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error In SelectData");
            }

            finally
            {
                con.Close();
            }
            return DT;
        }

        public void ToolStripTextBoxData(ref ToolStripTextBox tstToolStrip, String SPString, String FieldName)
        {
            DataTable DT = new DataTable();
            AutoCompleteStringCollection Source = new AutoCompleteStringCollection();

            try
            {
                DataBaseConn();

                SqlDataAdapter Adpt = new SqlDataAdapter(SPString, con);

                Adpt.Fill(DT);

                if (DT.Rows.Count > 0)
                {
                    tstToolStrip.AutoCompleteCustomSource.Clear();

                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        Source.Add(DT.Rows[i][FieldName].ToString());
                    }

                    tstToolStrip.AutoCompleteCustomSource = Source;
                    tstToolStrip.Text = "";
                    tstToolStrip.Focus();
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error In ToolStripTextBoxData");
            }

            finally
            {
                con.Close();
            }
        }
    }
}
