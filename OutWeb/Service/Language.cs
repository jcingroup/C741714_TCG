using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace OutWeb.Service
{
    public class Language
    {
        //string conn_str = WebConfigurationManager.ConnectionStrings["conn_string"].ConnectionString.ToString();
        string IsDebug = WebConfigurationManager.AppSettings["Debug"].ToString();
        string csql = "";

        DataSet ds = new DataSet();
        Service CService = new Service();

        //Log 記錄
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        #region 語系資料 Lang_List
        public DataTable Lang_List(ref string err_msg, string lang_id = "")
        {
            DataTable dt = new DataTable();
            string[] clang_id;
            string str_lang_id = "";

            SqlConnection conn = new SqlConnection(CService.conn_string());
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            try
            {
                clang_id = lang_id.Split(',');
                for (int i = 0; i < clang_id.Length; i++)
                {
                    if (i > 0)
                    {
                        str_lang_id = str_lang_id + ",";
                    }
                    str_lang_id = str_lang_id + clang_id[i];
                }

                csql = "select "
                    + "  * "
                    + "from "
                    + "  lang "
                    + "where "
                    + "  status = 'Y' ";


                if (lang_id.Trim().Length > 0)
                {
                    csql = csql + " and lang_id in (";
                    for (int i = 0; i < clang_id.Length; i++)
                    {
                        if (i > 0)
                        {
                            csql = csql + ",";
                        }
                        csql = csql + "'@str_lang_id" + i.ToString() + "'";
                    }
                    csql = csql + ") ";
                }
                csql = csql + "order by sort ";

                cmd.CommandText = csql;

                if (lang_id.Trim().Length > 0)
                {
                    cmd.Parameters.Clear();
                    for (int i = 0; i < lang_id.Length; i++)
                    {
                        cmd.Parameters.AddWithValue("@str_lang_id" + i.ToString(), lang_id[i]);
                    }
                }


                if (ds.Tables["lang"] != null)
                {
                    ds.Tables["lang"].Clear();
                }

                SqlDataAdapter lang_ada = new SqlDataAdapter();
                lang_ada.SelectCommand = cmd;
                lang_ada.Fill(ds, "lang");
                lang_ada = null;

                dt = ds.Tables["lang"];
            }
            catch (Exception ex)
            {
                err_msg = ex.Message;
                CService.msg_write("Error", ex.Message, ex.StackTrace, System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                cmd = null;
                conn = null;
            }



            return dt;
        }
        #endregion
    }
}