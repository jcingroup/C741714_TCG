using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace OutWeb.Service
{
    //使用者
    public class User
    {
        //string conn_str = WebConfigurationManager.ConnectionStrings["conn_string"].ConnectionString.ToString();
        string IsDebug = WebConfigurationManager.AppSettings["Debug"].ToString();
        string csql = "";
        string cate_dbf_name = "USR_GROUP";
        string dbf_name = "USR";
        string img_kind = "Usr";

        DataSet ds = new DataSet();
        Service CService = new Service();

        //Log 記錄
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        #region 群組

        #region 群組 Group_List
        public DataTable Group_List(ref string err_msg, string grp_id = "", string sort = "", string status = "", string title_query = "")
        {
            DataTable dt = new DataTable();
            SqlConnection conn = new SqlConnection(CService.conn_string());
            SqlCommand cmd = new SqlCommand();

            string[] Array_group_id;
            string[] Array_title_query;


            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                cmd.Connection = conn;

                Array_group_id = grp_id.Split(',');
                Array_title_query = title_query.Split(',');

                csql = "select "
                     + "  a1.* "
                     + "from "
                     + "("
                     + "select distinct "
                     + "  b1.ID, b1.GRP_NAME, b1.GRP_DESC, b1.GRP_AUTH, b1.STATUS, b1.SORT "
                     + "from "
                     + "   " + cate_dbf_name + " b1 "
                     + "where "
                     + "  b1.status <> 'D' ";

                if (status.Trim().Length > 0)
                {
                    csql = csql + " and b1.status = @status ";
                }

                if (grp_id.Trim().Length > 0)
                {
                    csql = csql + " and b1.id in (";
                    for (int i = 0; i < Array_group_id.Length; i++)
                    {
                        if (i > 0)
                        {
                            csql = csql + ",";
                        }
                        csql = csql + "@str_group_id" + i.ToString();
                    }
                    csql = csql + ") ";
                }

                if (title_query.Trim().Length > 0)
                {
                    csql = csql + " and (";
                    for (int i = 0; i < Array_title_query.Length; i++)
                    {
                        if (i > 0)
                        {
                            csql = csql + " or ";
                        }
                        csql = csql + " b1.grp_name like @str_title_query" + i.ToString() + " ";
                    }
                    csql = csql + ") ";
                }

                csql = csql + ")a1 ";

                if (sort.Trim().Length > 0)
                {
                    csql = csql + " order by " + sort + " ";
                }
                else
                {
                    csql = csql + " order by a1.sort ";
                }

                //--------------------------------------//
                if (IsDebug == "On")
                {
                    string cc_msg = "csql:" + csql;
                    CService.msg_write("Debug", cc_msg, "", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
                //--------------------------------------//	

                cmd.CommandText = csql;

                //---------------------------------------------------------------//
                cmd.Parameters.Clear();
                if (status.Trim().Length > 0)
                {
                    cmd.Parameters.AddWithValue("@status", status);
                }

                if (grp_id.Trim().Length > 0)
                {
                    for (int i = 0; i < Array_group_id.Length; i++)
                    {
                        cmd.Parameters.AddWithValue("@str_group_id" + i.ToString(), Array_group_id[i]);
                    }
                }

                if (title_query.Trim().Length > 0)
                {
                    for (int i = 0; i < Array_title_query.Length; i++)
                    {
                        cmd.Parameters.AddWithValue("@str_title_query" + i.ToString(), "%" + Array_title_query[i] + "%");
                    }
                }

                //--------------------------------------------------------------//

                if (ds.Tables["group"] != null)
                {
                    ds.Tables["group"].Clear();
                }

                SqlDataAdapter cate_ada = new SqlDataAdapter();
                cate_ada.SelectCommand = cmd;
                cate_ada.Fill(ds, "group");
                cate_ada = null;

                dt = ds.Tables["group"];
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

        #region 群組新增 Group_Insert
        public string Group_Insert(string grp_name = "", string grp_desc = "", string is_show = "", string sort = "")
        {
            string c_msg = "";

            SqlConnection conn = new SqlConnection(CService.conn_string());
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            try
            {
                //========抓取sort==============================================================//
                csql = "select (max(sort) + 1) as sort from " + cate_dbf_name + " ";
                cmd.CommandText = csql;

                if (ds.Tables["chk_sort"] != null)
                {
                    ds.Tables["chk_sort"].Clear();
                }

                SqlDataAdapter chk_sort_ada = new SqlDataAdapter();
                chk_sort_ada.SelectCommand = cmd;
                chk_sort_ada.Fill(ds, "chk_sort");
                chk_sort_ada = null;
                if (ds.Tables["chk_sort"].Rows.Count > 0)
                {
                    sort = ds.Tables["chk_sort"].Rows[0]["sort"].ToString();
                }
                else
                {
                    sort = "0";
                }
                //===============================================================================//

                csql = @"insert into " + cate_dbf_name + "(grp_name,grp_desc,sort,status) "
                     + "values(@grp_name,@grp_desc,@sort,@is_show)";

                cmd.CommandText = csql;

                ////讓ADO.NET自行判斷型別轉換
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@grp_name", grp_name);
                cmd.Parameters.AddWithValue("@grp_desc", grp_desc);
                cmd.Parameters.AddWithValue("@sort", sort);
                cmd.Parameters.AddWithValue("@is_show", is_show);

                cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                c_msg = ex.Message;
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

            return c_msg;
        }
        #endregion

        #region 群組更新 Grp_Update
        //更新內容
        public string Grp_Update(string id = "", string grp_name = "", string grp_desc = "", string is_show = "", string sort = "")
        {
            string c_msg = "";
            SqlConnection conn = new SqlConnection(CService.conn_string());
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            try
            {
                csql = @"update "
                     + "  " + cate_dbf_name + " "
                     + "set "
                     + "  cate_name = @cate_name "
                     + ", cate_desc = @cate_desc "
                     + ", status = @is_show "
                     + ", sort = @sort "
                     + ", lang_id = @lang_id "
                     + ", UPD_ID = 'System' "
                     + ", UPD_DT = getdate() "
                     + "where "
                     + "  id = @id ";

                cmd.CommandText = csql;

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@grp_name", grp_name);
                cmd.Parameters.AddWithValue("@grp_desc", grp_desc);
                cmd.Parameters.AddWithValue("@sort", sort);
                cmd.Parameters.AddWithValue("@is_show", is_show);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                c_msg = ex.Message;
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

            return c_msg;

        }
        #endregion

        #region 群組刪除 Grp_Del
        public string Grp_Del(string id = "")
        {
            string c_msg = "";

            SqlConnection conn = new SqlConnection(CService.conn_string());
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            try
            {


                //刪除類別資料
                //csql = @"delete from "
                //     + "  " + cate_dbf_name + " "
                //     + "where "
                //     + "  id = @cate_id ";

                csql = @"update " + cate_dbf_name + " "
                       + "set status = 'D' "
                       + "where "
                       + "  id = @id ";

                cmd.CommandText = csql;

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@id", id);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                c_msg = ex.Message;
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

            return c_msg;
        }
        #endregion

        #endregion

        #region 基本資料

        #region 資料抓取 List
        public DataTable List(ref string err_msg, string id = "", string sort = "", string status = "", string title_query = "", string grp_id = "", string signin_id = "")
        {
            DataTable dt = new DataTable();

            SqlConnection conn = new SqlConnection(CService.conn_string());
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            string[] Array_id;
            string[] Array_title_query;
            string[] Array_grp_id;
            string[] Array_signin_id;

            try
            {
                Array_id = id.Split(',');
                Array_title_query = title_query.Split(',');
                Array_grp_id = grp_id.Split(',');
                Array_signin_id = signin_id.Split(',');

                csql = "select "
                     + "  a1.* "
                     + "from "
                     + "("
                     + "select distinct "
                     + "  a1.ID, a1.SIGNIN_ID, a1.SIGNIN_PWD, a1.USR_NAME "
                     + ", a1.USR_ENAME, a1.USR_EML, a1.USR_RANK, a1.USR_DESC "
                     + ", a1.USR_GRP, a1.USR_STATES, a1.SIGNIN_DT, a1.STATUS "
                     + ", a2.GRP_NAME, a2.GRP_DESC, a2.GRP_AUTH, a2.STATUS as GRP_STATUS "
                     + ", a1.SORT "
                     + "from "
                     + "   " + dbf_name + " a1 "
                     + "LEFT JOIN " + cate_dbf_name + " a2 ON a1.USR_GRP = a2.id "
                     + "where "
                     + "  a1.status <> 'D' ";

                if (status.Trim().Length > 0)
                {
                    csql = csql + " and a1.status = @status ";
                }

                if (id.Trim().Length > 0)
                {
                    csql = csql + " and a1.id in (";
                    for (int i = 0; i < Array_id.Length; i++)
                    {
                        if (i > 0)
                        {
                            csql = csql + ",";
                        }
                        csql = csql + "@str_id" + i.ToString();
                    }
                    csql = csql + ") ";
                }

                if (title_query.Trim().Length > 0)
                {
                    csql = csql + " and (";
                    for (int i = 0; i < Array_title_query.Length; i++)
                    {
                        if (i > 0)
                        {
                            csql = csql + " or ";
                        }
                        csql = csql + " a1.c_title like @str_title_query" + i.ToString() + " ";
                    }
                    csql = csql + ") ";
                }

                if (grp_id.Trim().Length > 0)
                {
                    csql = csql + " and a1.usr_grp in (";
                    for (int i = 0; i < Array_grp_id.Length; i++)
                    {
                        if (i > 0)
                        {
                            csql = csql + ",";
                        }
                        csql = csql + "@str_grp_id" + i.ToString();
                    }
                    csql = csql + ") ";
                }

                if (signin_id.Trim().Length > 0)
                {
                    csql = csql + " and a1.signin_id in (";
                    for (int i = 0; i < Array_signin_id.Length; i++)
                    {
                        if (i > 0)
                        {
                            csql = csql + ",";
                        }
                        csql = csql + "@str_signin_id" + i.ToString();
                    }
                    csql = csql + ") ";
                }

                csql = csql + ")a1 ";

                if (sort.Trim().Length > 0)
                {
                    csql = csql + " order by " + sort + " ";
                }
                else
                {
                    csql = csql + " order by a1.usr_grp, a1.sort desc ";
                }

                cmd.CommandText = csql;

                //---------------------------------------------------------------//
                cmd.Parameters.Clear();
                if (status.Trim().Length > 0)
                {
                    cmd.Parameters.AddWithValue("@status", status);
                }

                if (id.Trim().Length > 0)
                {
                    for (int i = 0; i < Array_id.Length; i++)
                    {
                        cmd.Parameters.AddWithValue("@str_id" + i.ToString(), Array_id[i]);
                    }
                }

                if (title_query.Trim().Length > 0)
                {
                    for (int i = 0; i < Array_title_query.Length; i++)
                    {
                        cmd.Parameters.AddWithValue("@str_title_query" + i.ToString(), "%" + Array_title_query[i] + "%");
                    }
                }

                if (grp_id.Trim().Length > 0)
                {
                    for (int i = 0; i < Array_grp_id.Length; i++)
                    {
                        cmd.Parameters.AddWithValue("@str_grp_id" + i.ToString(), Array_grp_id[i]);
                    }
                }

                if (signin_id.Trim().Length > 0)
                {
                    for (int i = 0; i < Array_signin_id.Length; i++)
                    {
                        cmd.Parameters.AddWithValue("@str_signin_id" + i.ToString(), Array_signin_id[i]);
                    }
                }
                //--------------------------------------------------------------//

                if (ds.Tables["list"] != null)
                {
                    ds.Tables["list"].Clear();
                }

                SqlDataAdapter list_ada = new SqlDataAdapter();
                list_ada.SelectCommand = cmd;
                list_ada.Fill(ds, "list");
                list_ada = null;

                dt = ds.Tables["list"];

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

        #region 資料新增 Insert
        public string Insert(string SIGNIN_ID = "", string SIGNIN_PWD = "", string USR_NAME = "", string USR_ENAME = "", string USR_EML = "", string USR_RANK = "", string USR_DESC = "", string USR_GRP = "", string USR_STATES = "",string STATUS = "", string OP_ID = "System")
        {
            string c_msg = "";
            string id = "";
            SqlConnection conn = new SqlConnection(CService.conn_string());
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            try
            {
                csql = @"insert into " + dbf_name + "(SIGNIN_ID, SIGNIN_PWD, USR_NAME, USR_ENAME, USR_EML, USR_RANK, USR_DESC, USR_GRP, USR_STATES, STATUS) "
                     + "values(@SIGNIN_ID, @SIGNIN_PWD, @USR_NAME, @USR_ENAME, @USR_EML, @USR_RANK, @USR_DESC, @USR_GRP, @USR_STATES, @STATUS)";

                cmd.CommandText = csql;

                ////讓ADO.NET自行判斷型別轉換
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@SIGNIN_ID", SIGNIN_ID);
                cmd.Parameters.AddWithValue("@SIGNIN_PWD", SIGNIN_PWD);
                cmd.Parameters.AddWithValue("@USR_NAME", USR_NAME);
                cmd.Parameters.AddWithValue("@USR_ENAME", USR_ENAME);
                cmd.Parameters.AddWithValue("@USR_EML", USR_EML);
                cmd.Parameters.AddWithValue("@USR_RANK", USR_RANK);
                cmd.Parameters.AddWithValue("@USR_DESC", USR_DESC);
                cmd.Parameters.AddWithValue("@USR_GRP", USR_GRP);
                cmd.Parameters.AddWithValue("@USR_STATES", USR_STATES);
                cmd.Parameters.AddWithValue("@STATUS", STATUS);

                cmd.ExecuteNonQuery();
               
            }
            catch (Exception ex)
            {
                c_msg = ex.Message;
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

            return c_msg;
        }
        #endregion

        #region 資料更新 Update
        //更新內容
        public string Update(string id = "", string SIGNIN_ID = "", string SIGNIN_PWD = "", string USR_NAME = "", string USR_ENAME = "", string USR_EML = "", string USR_RANK = "", string USR_DESC = "", string USR_GRP = "", string USR_STATES = "",string STATUS = "", string OP_ID = "System")
        {
            string c_msg = "";
            SqlConnection conn = new SqlConnection(CService.conn_string());
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            try
            {
                csql = @"update "
                     + "  " + dbf_name + " "
                     + "set "
                     + "  SIGNIN_ID = @SIGNIN_ID "
                     + ", SIGNIN_PWD = @SIGNIN_PWD "
                     + ", USR_NAME = @USR_NAME "
                     + ", USR_ENAME = @USR_ENAME "
                     + ", USR_EML = @USR_EML "
                     + ", USR_RANK = @USR_RANK "
                     + ", USR_DESC = @USR_DESC "
                     + ", USR_GRP = @USR_GRP "
                     + ", USR_STATES = @USR_STATES "
                     + ", STATUS = @STATUS "
                     + ", UPD_ID = @OP_ID "
                     + ", UPD_DT = getdate() "
                     + "where "
                     + "  id = @id ";

                cmd.CommandText = csql;

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@SIGNIN_ID", SIGNIN_ID);
                cmd.Parameters.AddWithValue("@SIGNIN_PWD", SIGNIN_PWD);
                cmd.Parameters.AddWithValue("@USR_NAME", USR_NAME);
                cmd.Parameters.AddWithValue("@USR_ENAME", USR_ENAME);
                cmd.Parameters.AddWithValue("@USR_EML", USR_EML);
                cmd.Parameters.AddWithValue("@USR_RANK", USR_RANK);
                cmd.Parameters.AddWithValue("@USR_DESC", USR_DESC);
                cmd.Parameters.AddWithValue("@USR_GRP", USR_GRP);
                cmd.Parameters.AddWithValue("@USR_STATES", USR_STATES);
                cmd.Parameters.AddWithValue("@STATUS", STATUS);
                cmd.Parameters.AddWithValue("@OP_ID", OP_ID);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                c_msg = ex.Message;
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

            return c_msg;

        }
        #endregion

        #region 資料刪除 Del
        public string Del(string id = "")
        {
            string c_msg = "";

            SqlConnection conn = new SqlConnection(CService.conn_string());
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            try
            {
                //======== 刪除資料 ===================//
                csql = @"delete from "
                     + "  " + dbf_name + " "
                     + "where "
                     + "  id = @id ";

                cmd.CommandText = csql;

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@id", id);

                cmd.ExecuteNonQuery();
                //===================================//
            }
            catch (Exception ex)
            {
                c_msg = ex.Message;
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

            return c_msg;
        }
        #endregion

        #endregion

    }
}