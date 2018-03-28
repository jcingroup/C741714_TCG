using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace OutWeb.Service
{
    public class News
    {
        //string conn_str = WebConfigurationManager.ConnectionStrings["conn_string"].ConnectionString.ToString();
        string IsDebug = WebConfigurationManager.AppSettings["Debug"].ToString();
        string csql = "";

        DataSet ds = new DataSet();
        Service CService = new Service();

        //Log 記錄
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        #region 最新消息 News

        #region 最新消息類別
        #region 最新消息類別 News_Cate_List
        public DataTable News_Cate_List(ref string err_msg, string cate_id = "", string sort = "", string status = "", string title_query = "", string lang_id = "")
        {
            DataTable dt = new DataTable();
            SqlConnection conn = new SqlConnection(CService.conn_string());
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
                     + "   NEWS_CATE b1 "
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

                if (ds.Tables["news_cate"] != null)
                {
                    ds.Tables["news_cate"].Clear();
                }

                SqlDataAdapter NEWS_cate_ada = new SqlDataAdapter();
                NEWS_cate_ada.SelectCommand = cmd;
                NEWS_cate_ada.Fill(ds, "news_cate");
                NEWS_cate_ada = null;

                dt = ds.Tables["news_cate"];
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

        #region 消息類別新增 News_Cate_Insert
        public string News_Cate_Insert(string cate_name = "", string cate_desc = "", string is_show = "", string sort = "", string lang_id = "")
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
                ////========抓取sort==============================================================//
                //csql = "select (max(sort) + 1) as sort from News_Cate where lang_id = @lang_id";
                //cmd.CommandText = csql;

                //////讓ADO.NET自行判斷型別轉換
                //cmd.Parameters.Clear();
                //cmd.Parameters.AddWithValue("@lang_id", lang_id);

                //if (ds.Tables["chk_sort"] != null)
                //{
                //    ds.Tables["chk_sort"].Clear();
                //}

                //SqlDataAdapter chk_sort_ada = new SqlDataAdapter();
                //chk_sort_ada.SelectCommand = cmd;
                //chk_sort_ada.Fill(ds, "chk_sort");
                //chk_sort_ada = null;
                //if (ds.Tables["chk_sort"].Rows.Count > 0)
                //{
                //    sort = ds.Tables["chk_sort"].Rows[0]["sort"].ToString();
                //}
                //else
                //{
                //    sort = "0";
                //}
                ////===============================================================================//
                if (sort.Trim().Length == 0)
                {
                    sort = "0";
                }
                csql = @"insert into News_Cate(cate_name,cate_desc,sort,status,lang_id) "
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

        #region 消息類別更新 News_Cate_Update
        //更新內容
        public string News_Cate_Update(string cate_id = "", string cate_name = "", string cate_desc = "", string is_show = "", string sort = "", string lang_id = "")
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
                     + "  News_Cate "
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

        #region 消息類別刪除 News_Cate_Del
        public string News_Cate_Del(string cate_id = "")
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
                //刪除消息圖片
                csql = "delete from "
                     + "  IMG "
                     + "where "
                     + "    img_kind = 'NEWS' "
                     + "and img_no in ("
                     + "    select "
                     + "      Convert(nvarchar,a1.id) "
                     + "    from "
                     + "      NEWS a1 "
                     + "    where "
                     + "      a1.cate_id = @cate_id "
                     + "  ) ";

                cmd.CommandText = csql;

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@cate_id", cate_id);

                cmd.ExecuteNonQuery();
                //刪除消息資料
                csql = "delete from "
                     + "  NEWS "
                     + "where "
                     + "  cate_id = @cate_id ";

                cmd.CommandText = csql;

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@cate_id", cate_id);

                cmd.ExecuteNonQuery();

                //刪除消息類別資料
                csql = @"delete from "
                     + "  News_Cate "
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

        #region 消息資料抓取 News_List
        public DataTable News_List(ref string err_msg, string news_id = "", string sort = "", string status = "", string title_query = "", string start_date = "", string end_date = "", string is_index = "", string cate_id = "", string lang_id = "")
        {
            DataTable dt = new DataTable();

            SqlConnection conn = new SqlConnection(CService.conn_string());
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            string[] Array_news_id;
            string[] Array_title_query;
            string[] Array_cate_id;
            string[] Array_lang_id;

            try
            {
                Array_news_id = news_id.Split(',');
                Array_title_query = title_query.Split(',');
                Array_lang_id = lang_id.Split(',');
                Array_cate_id = cate_id.Split(',');

                csql = "select "
                     + "  a1.* "
                     + "from "
                     + "("
                     + "select distinct "
                     + "  a1.id, a1.n_title, convert(nvarchar(10),a1.n_date,23) as n_date, a1.n_url, a1.n_desc, a1.n_memo "
                     + ", a1.is_index, a1.sort, a1.status,a1.lang_id,a2.lang_name,a1.cate_id, a3.cate_name "
                     + "from "
                     + "   news a1 "
                     + "LEFT JOIN LANG a2 ON a1.LANG_ID = a2.LANG_ID "
                     + "LEFT JOIN NEWS_CATE a3 ON a1.cate_id = a3.id "
                     + "where "
                     + "  a1.status <> 'D' ";

                if (status.Trim().Length > 0)
                {
                    csql = csql + " and a1.status = @status ";
                }

                if (news_id.Trim().Length > 0)
                {
                    csql = csql + " and a1.id in (";
                    for (int i = 0; i < Array_news_id.Length; i++)
                    {
                        if (i > 0)
                        {
                            csql = csql + ",";
                        }
                        csql = csql + "@str_news_id" + i.ToString();
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
                        csql = csql + " a1.n_title like @str_title_query" + i.ToString() + " ";
                    }
                    csql = csql + ") ";
                }

                if (start_date.Trim().Length > 0)
                {

                    csql = csql + "and a1.n_date >= @start_date ";
                }

                if (end_date.Trim().Length > 0)
                {

                    csql = csql + "and a1.n_date <= @end_date ";
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
                    csql = csql + " order by a1.n_date desc ";
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

                if (news_id.Trim().Length > 0)
                {
                    for (int i = 0; i < Array_news_id.Length; i++)
                    {
                        cmd.Parameters.AddWithValue("@str_news_id" + i.ToString(), Array_news_id[i]);
                    }
                }

                if (title_query.Trim().Length > 0)
                {
                    for (int i = 0; i < Array_title_query.Length; i++)
                    {
                        cmd.Parameters.AddWithValue("@str_title_query" + i.ToString(), "%" + Array_title_query[i] + "%");
                    }
                }

                if (is_index.Trim().Length > 0)
                {
                    cmd.Parameters.AddWithValue("@is_index", is_index);
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
                //--------------------------------------------------------------//

                if (ds.Tables["news"] != null)
                {
                    ds.Tables["news"].Clear();
                }

                SqlDataAdapter news_ada = new SqlDataAdapter();
                news_ada.SelectCommand = cmd;
                news_ada.Fill(ds, "news");
                news_ada = null;

                dt = ds.Tables["news"];

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

        #region 消息資料新增 News_Insert
        public string News_Insert(string n_title = "", string n_date = "", string n_desc = "", string is_show = "", string is_index = "", string sort = "", string n_memo = "", string lang_id = "", string cate_id = "", string img_no = "")
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
                ////========抓取sort==============================================================//
                //csql = "select "
                //     + "   (max(sort) + 1) as sort "
                //     + "from "
                //     + "   News "
                //     + "where "
                //     + "    cate_id = @cate_id "
                //     + "and lang_id = @lang_id ";

                //cmd.CommandText = csql;

                //////讓ADO.NET自行判斷型別轉換
                //cmd.Parameters.Clear();
                //cmd.Parameters.AddWithValue("@cate_id", cate_id);
                //cmd.Parameters.AddWithValue("@lang_id", lang_id);

                //if (ds.Tables["chk_sort"] != null)
                //{
                //    ds.Tables["chk_sort"].Clear();
                //}

                //SqlDataAdapter chk_sort_ada = new SqlDataAdapter();
                //chk_sort_ada.SelectCommand = cmd;
                //chk_sort_ada.Fill(ds, "chk_sort");
                //chk_sort_ada = null;
                //if (ds.Tables["chk_sort"].Rows.Count > 0)
                //{
                //    sort = ds.Tables["chk_sort"].Rows[0]["sort"].ToString();
                //}
                //else
                //{
                //    sort = "0";
                //}
                ////===============================================================================//
                if (sort.Trim().Length == 0)
                {
                    sort = "0";
                }
                csql = @"insert into News(n_title,n_date,n_desc,is_index,sort,status,n_memo,lang_id,cate_id) "
                     + "values(@n_title,@n_date,@n_desc,@is_index,@sort,@is_show,@n_memo,@lang_id,@cate_id)";

                cmd.CommandText = csql;

                ////讓ADO.NET自行判斷型別轉換
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@n_title", n_title);
                cmd.Parameters.AddWithValue("@n_date", n_date);
                cmd.Parameters.AddWithValue("@n_desc", n_desc);
                cmd.Parameters.AddWithValue("@is_index", is_index);
                cmd.Parameters.AddWithValue("@sort", sort);
                cmd.Parameters.AddWithValue("@is_show", is_show);
                cmd.Parameters.AddWithValue("@n_memo", n_memo);
                cmd.Parameters.AddWithValue("@lang_id", lang_id);
                cmd.Parameters.AddWithValue("@cate_id", cate_id);

                cmd.ExecuteNonQuery();

                //抓取序號
                csql = "select "
                     + "  * "
                     + "from "
                     + "  News "
                     + "where "
                     + "    n_title = @n_title "
                     + "and n_date = @n_date "
                     + "and n_desc = @n_desc "
                     + "and is_index = @is_index "
                     + "and sort = @sort "
                     + "and status = @is_show "
                     + "and n_memo = @n_memo "
                     + "and lang_id = @lang_id "
                     + "and cate_id = @cate_id ";

                cmd.CommandText = csql;

                ////讓ADO.NET自行判斷型別轉換
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@n_title", n_title);
                cmd.Parameters.AddWithValue("@n_date", n_date);
                cmd.Parameters.AddWithValue("@n_desc", n_desc);
                cmd.Parameters.AddWithValue("@is_index", is_index);
                cmd.Parameters.AddWithValue("@sort", sort);
                cmd.Parameters.AddWithValue("@is_show", is_show);
                cmd.Parameters.AddWithValue("@n_memo", n_memo);
                cmd.Parameters.AddWithValue("@lang_id", lang_id);
                cmd.Parameters.AddWithValue("@cate_id", cate_id);

                if (ds.Tables["chk_news"] != null)
                {
                    ds.Tables["chk_news"].Clear();
                }

                SqlDataAdapter chk_ada = new SqlDataAdapter();
                chk_ada.SelectCommand = cmd;
                chk_ada.Fill(ds, "chk_news");
                chk_ada = null;

                if (ds.Tables["chk_news"].Rows.Count > 0)
                {
                    id = ds.Tables["chk_news"].Rows[0]["id"].ToString();
                    if (img_no.Trim().Length > 0)
                    {
                        csql = @"UPDATE "
                             + " IMG "
                             + "SET "
                             + "  IMG_NO = @id "
                             + "WHERE "
                             + "    IMG_KIND = 'News' "
                             + "AND IMG_NO = @img_no ";
                        cmd.CommandText = csql;

                        ////讓ADO.NET自行判斷型別轉換
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@img_no", img_no);

                        cmd.ExecuteNonQuery();
                    }
                }

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

        #region 消息資料更新 News_Update
        //更新內容
        public string News_Update(string n_id = "", string n_title = "", string n_date = "", string n_desc = "", string is_show = "", string is_index = "", string sort = "", string n_memo = "", string lang_id = "", string cate_id = "")
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
                     + "  news "
                     + "set "
                     + "  n_title = @n_title "
                     + ", n_date = @n_date "
                     + ", n_desc = @n_desc "
                     + ", status = @is_show "
                     + ", is_index = @is_index "
                     + ", sort = @sort "
                     + ", n_memo = @n_memo "
                     + ", lang_id = @lang_id "
                     + ", cate_id = @cate_id "
                     + ", UPD_ID = 'System' "
                     + ", UPD_DT = getdate() "
                     + "where "
                     + "  id = @n_id ";

                cmd.CommandText = csql;

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@n_id", n_id);
                cmd.Parameters.AddWithValue("@n_title", n_title);
                cmd.Parameters.AddWithValue("@n_date", n_date);
                cmd.Parameters.AddWithValue("@n_desc", n_desc);
                cmd.Parameters.AddWithValue("@is_index", is_index);
                cmd.Parameters.AddWithValue("@sort", sort);
                cmd.Parameters.AddWithValue("@is_show", is_show);
                cmd.Parameters.AddWithValue("@n_memo", n_memo);
                cmd.Parameters.AddWithValue("@lang_id", lang_id);
                cmd.Parameters.AddWithValue("@cate_id", cate_id);

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

        #region 消息資料刪除 News_Del
        public string News_Del(string n_id = "")
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
                //刪除圖片
                csql = @"delete from IMG WHERE IMG_NO = @n_id AND IMG_KIND='News' ";
                cmd.CommandText = csql;

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@n_id", n_id);

                cmd.ExecuteNonQuery();

                //刪除資料
                csql = @"delete from "
                     + "  news "
                     + "where "
                     + "  id = @n_id ";

                cmd.CommandText = csql;

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@n_id", n_id);

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
    }
}