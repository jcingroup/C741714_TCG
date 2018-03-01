using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace OutWeb.Service
{
    public class Activity
    {
        string conn_str = WebConfigurationManager.ConnectionStrings["conn_string"].ConnectionString.ToString();
        string csql = "";
        string cate_dbf_name = "ACTIVITY_CATE";
        string dbf_name = "ACTIVITY";
        string img_kind = "Activity";

        DataSet ds = new DataSet();
        Service CService = new Service();
        //Log 記錄
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

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
            string[] Array_lang_id;
            //string[] Array_cate_id;

            try
            {
                Array_id = id.Split(',');
                Array_title_query = title_query.Split(',');
                Array_lang_id = lang_id.Split(',');
                //Array_cate_id = cate_id.Split(',');

                csql = "select "
                     + "  a1.* "
                     + "from "
                     + "("
                     + "select distinct "
                     + "  a1.id, a1.c_title, a1.c_desc, a1.sort, a1.status "
                     + ", a1.lang_id, a2.lang_name "
                     //+ ", a1.cate_id, a3.cate_name "
                     + ", convert(nvarchar(10),a1.c_date,23) as c_date, a1.is_index "
                     + ", a4.img_file, isnull(a5.img_count,0) as img_count "
                     + "from "
                     + "   " + dbf_name + " a1 "
                     + "LEFT JOIN LANG a2 ON a1.LANG_ID = a2.LANG_ID "
                     //+ "LEFT JOIN " + cate_dbf_name + " a3 ON a1.cate_id = a3.id "
                     + "LEFT JOIN IMG a4 ON Convert(nvarchar(50),a1.id) = a4.img_no and a4.is_index = 'Y' and img_kind = '" + img_kind + "' "
                     + "LEFT JOIN ( "
                     + " Select "
                     + "    img_no, count(id) as img_count "
                     + " From "
                     + "    IMG "
                     + " Where "
                     + "    IMG_KIND = '" + img_kind + "' "
                     + " Group By "
                     + "    img_no "
                     + ") a5 ON Convert(nvarchar(50),a1.id) = a5.img_no "
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

                //if (cate_id.Trim().Length > 0)
                //{
                //    csql = csql + " and a1.cate_id in (";
                //    for (int i = 0; i < Array_cate_id.Length; i++)
                //    {
                //        if (i > 0)
                //        {
                //            csql = csql + ",";
                //        }
                //        csql = csql + "@str_cate_id" + i.ToString();
                //    }
                //    csql = csql + ") ";
                //}

                csql = csql + ")a1 ";

                if (sort.Trim().Length > 0)
                {
                    csql = csql + " order by " + sort + " ";
                }
                else
                {
                    //csql = csql + " order by a1.cate_id, a1.sort desc ";
                    csql = csql + " order by a1.sort desc ";
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

                //if (cate_id.Trim().Length > 0)
                //{
                //    for (int i = 0; i < Array_cate_id.Length; i++)
                //    {
                //        cmd.Parameters.AddWithValue("@str_cate_id" + i.ToString(), Array_cate_id[i]);
                //    }
                //}

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
                logger.Error(ex.Message);
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
                     //+ "    cate_id = @cate_id and "
                     + " lang_id = @lang_id ";

                cmd.CommandText = csql;

                ////讓ADO.NET自行判斷型別轉換
                cmd.Parameters.Clear();
                //cmd.Parameters.AddWithValue("@cate_id", cate_id);
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

                //csql = @"insert into " + dbf_name + "(c_title,c_date,c_desc,is_index,sort,status,lang_id,cate_id) "
                //     + "values(@c_title,@c_date,@c_desc,@is_index,@sort,@is_show,@lang_id,@cate_id)";

                csql = @"insert into " + dbf_name + "(c_title,c_date,c_desc,is_index,sort,status,lang_id) "
                     + "values(@c_title,@c_date,@c_desc,@is_index,@sort,@is_show,@lang_id)";

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
                //cmd.Parameters.AddWithValue("@cate_id", cate_id);

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
                     //+ "and cate_id = @cate_id "
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
                //cmd.Parameters.AddWithValue("@cate_id", cate_id);

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
                        //IMG
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

                        //URL
                        csql = @"UPDATE "
                             + " URL "
                             + "SET "
                             + "  URL_NO = @id "
                             + "WHERE "
                             + "    URL_KIND = '" + img_kind + "' "
                             + "AND URL_NO = @img_no ";
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
                string err_msg = CService.rtn_errmsg(ex.Message, ex.Source, ex.StackTrace);
                logger.Error(err_msg);
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
                     //+ ", cate_id = @cate_id "
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
                //cmd.Parameters.AddWithValue("@cate_id", cate_id);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                c_msg = ex.Message;
                logger.Error(ex.Message);
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
                //======== 刪除URL ====================//
                csql = @"delete from URL SET URL_KIND='" + img_kind + "' WHERE URL_NO = @id ";
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
                logger.Error(ex.Message);
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
    }
}