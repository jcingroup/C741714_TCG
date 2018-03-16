using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;
using OutWeb.Service;

namespace OutWeb.Service
{
    public class Focus
    {

        string conn_str = WebConfigurationManager.ConnectionStrings["conn_string"].ConnectionString.ToString();
        string csql = "";
        string cate_dbf_name = "FOCUS_CATE";
        string dbf_name = "FOCUS";
        string img_kind = "Focus";

        DataSet ds = new DataSet();
        Service CService = new Service();

        //Log 記錄
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        #region 類別

        #region 類別 Cate_List
        public DataTable Cate_List(ref string err_msg, string cate_id = "", string sort = "", string status = "", string title_query = "", string lang_id = "")
        {
            DataTable dt = new DataTable();
            SqlConnection conn = new SqlConnection(conn_str);
            SqlCommand cmd = new SqlCommand();

            string[] Array_cate_id;
            string[] Array_title_query;


            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                cmd.Connection = conn;

                Array_cate_id = cate_id.Split(',');
                Array_title_query = title_query.Split(',');

                csql = "select "
                     + "  a1.* "
                     + "from "
                     + "("
                     + "select distinct "
                     + "  b1.ID, b1.CATE_NAME, b1.CATE_DESC "
                     + ", b1.SORT, b1.STATUS, b1.LANG_ID, b2.LANG_NAME "
                     + "from "
                     + "   " + cate_dbf_name + " b1 "
                     + "LEFT JOIN LANG b2 ON b1.LANG_ID = b2.LANG_ID "
                     + "where "
                     + "  b1.status <> 'D' ";

                if (status.Trim().Length > 0)
                {
                    csql = csql + " and b1.status = @status ";
                }

                if (cate_id.Trim().Length > 0)
                {
                    csql = csql + " and b1.id in (";
                    for (int i = 0; i < Array_cate_id.Length; i++)
                    {
                        if (i > 0)
                        {
                            csql = csql + ",";
                        }
                        csql = csql + "@str_cate_id" + i.ToString();
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
                        csql = csql + " b1.cate_name like @str_title_query" + i.ToString() + " ";
                    }
                    csql = csql + ") ";
                }

                if (lang_id.Trim().Length > 0)
                {
                    csql = csql + " and b1.lang_id = @lang_id ";
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

                cmd.CommandText = csql;

                //---------------------------------------------------------------//
                cmd.Parameters.Clear();
                if (status.Trim().Length > 0)
                {
                    cmd.Parameters.AddWithValue("@status", status);
                }

                if (cate_id.Trim().Length > 0)
                {
                    for (int i = 0; i < Array_cate_id.Length; i++)
                    {
                        cmd.Parameters.AddWithValue("@str_cate_id" + i.ToString(), Array_cate_id[i]);
                    }
                }

                if (title_query.Trim().Length > 0)
                {
                    for (int i = 0; i < Array_title_query.Length; i++)
                    {
                        cmd.Parameters.AddWithValue("@str_title_query" + i.ToString(), "%" + Array_title_query[i] + "%");
                    }
                }

                if (lang_id.Trim().Length > 0)
                {
                    cmd.Parameters.AddWithValue("@lang_id", lang_id);
                }
                //--------------------------------------------------------------//

                if (ds.Tables["cate"] != null)
                {
                    ds.Tables["cate"].Clear();
                }

                SqlDataAdapter cate_ada = new SqlDataAdapter();
                cate_ada.SelectCommand = cmd;
                cate_ada.Fill(ds, "cate");
                cate_ada = null;

                dt = ds.Tables["cate"];
            }
            catch (Exception ex)
            {
                err_msg = ex.Message;
                //logger.Error(CService.rtn_errmsg(ex));
                logger.Error(CService.rtn_errmsg(ex));
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

        #region 類別新增 Cate_Insert
        public string Cate_Insert(string cate_name = "", string cate_desc = "", string is_show = "", string sort = "", string lang_id = "")
        {
            string c_msg = "";

            SqlConnection conn = new SqlConnection(conn_str);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            try
            {
                //========抓取sort==============================================================//
                csql = "select isnull((max(sort) + 1),0) as sort from " + cate_dbf_name + " where lang_id = @lang_id";
                cmd.CommandText = csql;

                ////讓ADO.NET自行判斷型別轉換
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@lang_id", lang_id);

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

                csql = @"insert into " + cate_dbf_name + "(cate_name,cate_desc,sort,status,lang_id) "
                     + "values(@cate_name,@cate_desc,@sort,@is_show,@lang_id)";

                cmd.CommandText = csql;

                ////讓ADO.NET自行判斷型別轉換
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@cate_name", cate_name);
                cmd.Parameters.AddWithValue("@cate_desc", cate_desc);
                cmd.Parameters.AddWithValue("@sort", sort);
                cmd.Parameters.AddWithValue("@is_show", is_show);
                cmd.Parameters.AddWithValue("@lang_id", lang_id);

                cmd.ExecuteNonQuery();

                ////抓取其編號
                //csql = @"select distinct "
                //     + "  cate_id "
                //     + "from "
                //     + "   Partner_Cate "
                //     + "where "
                //     + "    cate_name = @cate_name "
                //     + "and cate_desc = @cate_desc "
                //     + "and sort = @sort "
                //     + "and status = @is_show ";

                //cmd.CommandText = csql;

                //////讓ADO.NET自行判斷型別轉換
                //cmd.Parameters.Clear();
                //cmd.Parameters.AddWithValue("@cate_name", cate_name);
                //cmd.Parameters.AddWithValue("@cate_desc", cate_desc);
                //cmd.Parameters.AddWithValue("@sort", sort);
                //cmd.Parameters.AddWithValue("@is_show", is_show);

                //if (ds.Tables["chk_Partner_Cate"] != null)
                //{
                //    ds.Tables["chk_Partner_Cate"].Clear();
                //}

                //SqlDataAdapter prod_chk_ada = new SqlDataAdapter();
                //prod_chk_ada.SelectCommand = cmd;
                //prod_chk_ada.Fill(ds, "chk_Partner_Cate");
                //prod_chk_ada = null;

                //if (ds.Tables["chk_Partner_Cate"].Rows.Count > 0)
                //{
                //    cate_id = ds.Tables["chk_Partner_Cate"].Rows[0]["cate_id"].ToString();
                //    if (img_no.Trim().Length > 0)
                //    {
                //        csql = @"update "
                //             + "  Partner_Cate_img "
                //             + "set "
                //             + "  img_no = @cate_id "
                //             + "where "
                //             + "  img_no = @img_no ";

                //        cmd.CommandText = csql;

                //        ////讓ADO.NET自行判斷型別轉換
                //        cmd.Parameters.Clear();
                //        cmd.Parameters.AddWithValue("@cate_id", cate_id);
                //        cmd.Parameters.AddWithValue("@img_no", img_no);

                //        cmd.ExecuteNonQuery();
                //    }
                //}


            }
            catch (Exception ex)
            {
                c_msg = ex.Message;
                logger.Error(CService.rtn_errmsg(ex));
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

        #region 類別更新 Cate_Update
        //更新內容
        public string Cate_Update(string cate_id = "", string cate_name = "", string cate_desc = "", string is_show = "", string sort = "", string lang_id = "")
        {
            string c_msg = "";
            SqlConnection conn = new SqlConnection(conn_str);
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
                     + "  id = @cate_id ";

                cmd.CommandText = csql;

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@cate_id", cate_id);
                cmd.Parameters.AddWithValue("@cate_name", cate_name);
                cmd.Parameters.AddWithValue("@cate_desc", cate_desc);
                cmd.Parameters.AddWithValue("@sort", sort);
                cmd.Parameters.AddWithValue("@is_show", is_show);
                cmd.Parameters.AddWithValue("@lang_id", lang_id);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                c_msg = ex.Message;
                logger.Error(CService.rtn_errmsg(ex));
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

        #region 類別刪除 Cate_Del
        public string Cate_Del(string cate_id = "")
        {
            string c_msg = "";

            SqlConnection conn = new SqlConnection(conn_str);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            try
            {
                //刪除消息圖片
                csql = "delete from "
                     + "  IMG "
                     + "where "
                     + "    img_item = '" + img_kind + "' "
                     + "and img_no in ("
                     + "    select "
                     + "      Convert(nvarchar,a1.id) "
                     + "    from "
                     + "      " + dbf_name + " a1 "
                     + "    where "
                     + "      a1.cate_id = @cate_id "
                     + "  ) ";

                cmd.CommandText = csql;

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@cate_id", cate_id);

                cmd.ExecuteNonQuery();
                //刪除消息資料
                csql = "delete from "
                     + "  " + dbf_name + " "
                     + "where "
                     + "  cate_id = @cate_id ";

                cmd.CommandText = csql;

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@cate_id", cate_id);

                cmd.ExecuteNonQuery();

                //刪除類別資料
                csql = @"delete from "
                     + "  " + cate_dbf_name + " "
                     + "where "
                     + "  id = @cate_id ";

                cmd.CommandText = csql;

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@cate_id", cate_id);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                c_msg = ex.Message;
                logger.Error(CService.rtn_errmsg(ex));
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
        public DataTable List(ref string err_msg, string id = "", string sort = "", string status = "", string title_query = "", string start_date = "", string end_date = "", string is_index = "", string cate_id = "", string lang_id = "")
        {
            DataTable dt = new DataTable();

            SqlConnection conn = new SqlConnection(conn_str);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            string[] Array_id;
            string[] Array_title_query;
            string[] Array_cate_id;
            string[] Array_lang_id;

            try
            {
                Array_id = id.Split(',');
                Array_title_query = title_query.Split(',');
                Array_lang_id = lang_id.Split(',');
                Array_cate_id = cate_id.Split(',');

                csql = "select "
                     + "  a1.* "
                     + "from "
                     + "("
                     + "select distinct "
                     + "  a1.id, a1.c_title, a1.c_desc, a1.sort, a1.status "
                     + ", a1.lang_id, a2.lang_name, a1.cate_id, a3.cate_name "
                     + ", convert(nvarchar(10),a1.c_date,23) as c_date, a1.is_index "
                     + "from "
                     + "   " + dbf_name + " a1 "
                     + "LEFT JOIN LANG a2 ON a1.LANG_ID = a2.LANG_ID "
                     + "LEFT JOIN " + cate_dbf_name + " a3 ON a1.cate_id = a3.id "
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

                if (start_date.Trim().Length > 0)
                {

                    csql = csql + "and a1.c_date >= @start_date ";
                }

                if (end_date.Trim().Length > 0)
                {

                    csql = csql + "and a1.c_date <= @end_date ";
                }

                if (is_index.Trim().Length > 0)
                {
                    csql = csql + "and a1.is_index = @is_index ";
                }   

                if (lang_id.Trim().Length > 0)
                {
                    csql = csql + " and a1.lang_id in (";
                    for (int i = 0; i < Array_lang_id.Length; i++)
                    {
                        if (i > 0)
                        {
                            csql = csql + ",";
                        }
                        csql = csql + "@str_lang_id" + i.ToString();
                    }
                    csql = csql + ") ";
                }

                if (cate_id.Trim().Length > 0)
                {
                    csql = csql + " and a1.cate_id in (";
                    for (int i = 0; i < Array_cate_id.Length; i++)
                    {
                        if (i > 0)
                        {
                            csql = csql + ",";
                        }
                        csql = csql + "@str_cate_id" + i.ToString();
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
                    csql = csql + " order by a1.cate_id, a1.sort desc ";
                }

                cmd.CommandText = csql;

                //---------------------------------------------------------------//
                cmd.Parameters.Clear();
                if (status.Trim().Length > 0)
                {
                    cmd.Parameters.AddWithValue("@status", status);
                }

                if (start_date.Trim().Length > 0)
                {
                    cmd.Parameters.AddWithValue("@start_date", start_date);
                }

                if (end_date.Trim().Length > 0)
                {
                    cmd.Parameters.AddWithValue("@end_date", end_date);
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

                if (lang_id.Trim().Length > 0)
                {
                    for (int i = 0; i < Array_lang_id.Length; i++)
                    {
                        cmd.Parameters.AddWithValue("@str_lang_id" + i.ToString(), Array_lang_id[i]);
                    }
                }

                if (cate_id.Trim().Length > 0)
                {
                    for (int i = 0; i < Array_cate_id.Length; i++)
                    {
                        cmd.Parameters.AddWithValue("@str_cate_id" + i.ToString(), Array_cate_id[i]);
                    }
                }

                if (is_index.Trim().Length > 0)
                {
                    cmd.Parameters.AddWithValue("@is_index", is_index);
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
                logger.Error(CService.rtn_errmsg(ex));
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
        public string Insert(string c_title = "", string c_date = "", string c_desc = "", string is_show = "", string is_index = "", string sort = "", string lang_id = "", string cate_id = "", string img_no = "")
        {
            string c_msg = "";
            string id = "";
            DateTime cdate;

            //cdate = DateTime.ParseExact(c_date, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.AllowWhiteSpaces);
            //cdate = Convert.ToDateTime(c_date);
            SqlConnection conn = new SqlConnection(conn_str);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            try
            {
                //========抓取sort==============================================================//
                csql = "select "
                     + "    (max(sort) + 1) as sort "
                     + "from "
                     + " " + dbf_name + " "
                     + "where "
                     + "    cate_id = @cate_id "
                     + "and lang_id = @lang_id ";

                cmd.CommandText = csql;

                ////讓ADO.NET自行判斷型別轉換
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@cate_id", cate_id);
                cmd.Parameters.AddWithValue("@lang_id", lang_id);

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

                csql = @"insert into " + dbf_name + "(c_title,c_date,c_desc,is_index,sort,status,lang_id,cate_id) "
                     + "values(@c_title,@c_date,@c_desc,@is_index,@sort,@is_show,@lang_id,@cate_id)";

                cmd.CommandText = csql;

                ////讓ADO.NET自行判斷型別轉換
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@c_title", c_title);
                cmd.Parameters.AddWithValue("@c_date", c_date);
                cmd.Parameters.AddWithValue("@c_desc", c_desc);
                cmd.Parameters.AddWithValue("@sort", sort);
                cmd.Parameters.AddWithValue("@is_index", is_index);
                cmd.Parameters.AddWithValue("@is_show", is_show);
                cmd.Parameters.AddWithValue("@lang_id", lang_id);
                cmd.Parameters.AddWithValue("@cate_id", cate_id);

                cmd.ExecuteNonQuery();

                //=============================================================================//
                //抓取序號
                csql = "select "
                     + "  * "
                     + "from "
                     + "  " + dbf_name + " "
                     + "where "
                     + "    c_title = @c_title "
                     + "and c_desc = @c_desc "
                     + "and sort = @sort "
                     + "and status = @is_show "
                     + "and lang_id = @lang_id "
                     + "and cate_id = @cate_id "
                     + "and c_date = @c_date "
                     + "and is_index = @is_index ";

                cmd.CommandText = csql;

                ////讓ADO.NET自行判斷型別轉換
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@c_title", c_title);
                cmd.Parameters.AddWithValue("@c_date", c_date);
                cmd.Parameters.AddWithValue("@c_desc", c_desc);
                cmd.Parameters.AddWithValue("@sort", sort);
                cmd.Parameters.AddWithValue("@is_index", is_index);
                cmd.Parameters.AddWithValue("@is_show", is_show);
                cmd.Parameters.AddWithValue("@lang_id", lang_id);
                cmd.Parameters.AddWithValue("@cate_id", cate_id);

                if (ds.Tables["chk"] != null)
                {
                    ds.Tables["chk"].Clear();
                }

                SqlDataAdapter chk_ada = new SqlDataAdapter();
                chk_ada.SelectCommand = cmd;
                chk_ada.Fill(ds, "chk");
                chk_ada = null;

                if (ds.Tables["chk"].Rows.Count > 0)
                {
                    id = ds.Tables["chk"].Rows[0]["id"].ToString();
                    if (img_no.Trim().Length > 0)
                    {
                        csql = @"UPDATE "
                             + " IMG "
                             + "SET "
                             + "  IMG_NO = @id "
                             + "WHERE "
                             + "    IMG_KIND = '" + img_kind + "' "
                             + "AND IMG_NO = @img_no ";
                        cmd.CommandText = csql;

                        ////讓ADO.NET自行判斷型別轉換
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@img_no", img_no);

                        cmd.ExecuteNonQuery();
                    }
                }
                //===========================================================================//

            }
            catch (Exception ex)
            {
                c_msg = ex.Message;
                logger.Error(CService.rtn_errmsg(ex));
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
        public string Update(string id = "", string c_title = "", string c_date = "", string c_desc = "", string is_show = "", string is_index = "", string sort = "", string lang_id = "", string cate_id = "")
        {
            string c_msg = "";
            SqlConnection conn = new SqlConnection(conn_str);
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
                     + "  c_title = @c_title "
                     + ", c_desc = @c_desc "
                     + ", c_date = @c_date "
                     + ", status = @is_show "
                     + ", sort = @sort "
                     + ", lang_id = @lang_id "
                     + ", cate_id = @cate_id "
                     + ", is_index = @is_index "
                     + ", UPD_ID = 'System' "
                     + ", UPD_DT = getdate() "
                     + "where "
                     + "  id = @id ";

                cmd.CommandText = csql;

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@c_title", c_title);
                cmd.Parameters.AddWithValue("@c_date", c_date);
                cmd.Parameters.AddWithValue("@c_desc", c_desc);
                cmd.Parameters.AddWithValue("@sort", sort);
                cmd.Parameters.AddWithValue("@is_index", is_index);
                cmd.Parameters.AddWithValue("@is_show", is_show);
                cmd.Parameters.AddWithValue("@lang_id", lang_id);
                cmd.Parameters.AddWithValue("@cate_id", cate_id);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                c_msg = ex.Message;
                logger.Error(CService.rtn_errmsg(ex));
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

            SqlConnection conn = new SqlConnection(conn_str);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            try
            {
                //======== 刪除圖片 ====================//
                csql = @"delete from IMG SET IMG_KIND='" + img_kind + "' WHERE IMG_NO = @id ";
                cmd.CommandText = csql;

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@id", id);

                cmd.ExecuteNonQuery();
                //====================================//
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
                logger.Error(CService.rtn_errmsg(ex));
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