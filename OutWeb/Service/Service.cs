using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Text;
using System.Web.Mvc;

namespace OutWeb.Service
{
    public class Service
    {
        string conn_str = WebConfigurationManager.ConnectionStrings["conn_string"].ConnectionString.ToString();
        string local_conn_str = WebConfigurationManager.ConnectionStrings["local_conn_string"].ConnectionString.ToString();
        string IsDebug = WebConfigurationManager.AppSettings["Debug"].ToString();
        string IsLocal = WebConfigurationManager.AppSettings["Local"].ToString();
        string LogToFile = WebConfigurationManager.AppSettings["LogToFile"].ToString();
        string LogToDB = WebConfigurationManager.AppSettings["LogToDB"].ToString();
        string csql = "";
        DataSet ds = new DataSet();
        //Service CService = new Service();
        //Log 記錄
        private NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        #region Mail
        public DataTable Mail(string From, string From_Name, string M_To, string M_CC, string subject, string body)
        {
            string err_msg = "";
            string c_msg = "";
            string status = "";
            string[] c_To;
            string[] c_CC;

            string str_Host = "";
            string str_Account = "";
            string str_Pwd = "";
            string str_Port = "";
            string str_SSL = "";
            int int_Port = 0;



            DataTable dt = new DataTable();
            DataRow dw;

            dt.Columns.Add("status", System.Type.GetType("System.String"));
            dt.Columns.Add("c_msg", System.Type.GetType("System.String"));
            dt.Columns.Add("err_msg", System.Type.GetType("System.String"));

            status = "N";

            SqlConnection conn = new SqlConnection(conn_string());
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            //抓取Smtp Server 設定
            try
            {
                csql = "SELECT ms_id, _BD_ID, _BD_DT, _UPD_ID, _UPD_DT, Host, Port, Account, Pwd, SSL FROM Mail_Server";
                cmd.CommandText = csql;
                if (ds.Tables["m_server"] != null)
                {
                    ds.Tables["m_server"].Clear();
                }

                SqlDataAdapter news_ada = new SqlDataAdapter();
                news_ada.SelectCommand = cmd;
                news_ada.Fill(ds, "m_server");
                news_ada = null;

                if (ds.Tables["m_server"].Rows.Count > 0)
                {
                    str_Host = ds.Tables["m_server"].Rows[0]["Host"].ToString();
                    str_Account = ds.Tables["m_server"].Rows[0]["Account"].ToString();
                    str_Pwd = ds.Tables["m_server"].Rows[0]["Pwd"].ToString();
                    str_Port = ds.Tables["m_server"].Rows[0]["Port"].ToString();
                    str_SSL = ds.Tables["m_server"].Rows[0]["SSL"].ToString();
                    int_Port = Convert.ToInt32(str_Port);
                    //=====================================//
                    From = str_Account;
                }
                else
                {
                    str_Host = "";
                    str_Account = "";
                    str_Pwd = "";
                    str_Port = "";
                    str_SSL = "";
                    int_Port = 0;
                }
            }
            catch (Exception ex)
            {
                err_msg = ex.Message;
                //logger.Error(rtn_errmsg(ex));
                msg_write("Error", ex.Message, ex.StackTrace, System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "," + System.Reflection.MethodBase.GetCurrentMethod().Name);
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


            try
            {


                System.Net.Mail.MailMessage em = new System.Net.Mail.MailMessage();
                //寄件者
                em.From = new System.Net.Mail.MailAddress(From, From_Name, System.Text.Encoding.UTF8);
                //收件者
                if (M_To.Trim().Length > 0)
                {
                    c_To = M_To.Split(',');
                    for (int i = 0; i < c_To.Length; i++)
                    {
                        //em.To.Add(new System.Net.Mail.MailAddress("收件信箱"));
                        em.To.Add(new System.Net.Mail.MailAddress(c_To[i]));
                    }
                }

                //副本
                if (M_CC.Trim().Length > 0)
                {
                    c_CC = M_CC.Split(',');
                    for (int i = 0; i < c_CC.Length; i++)
                    {
                        //em.To.Add(new System.Net.Mail.MailAddress("收件信箱"));
                        em.CC.Add(new System.Net.Mail.MailAddress(c_CC[i]));
                    }
                }

                //em.Bcc.Add(new System.Net.Mail.MailAddress("gary661116@gmail.com"));
                //em.Bcc.Add(new System.Net.Mail.MailAddress("boy0039@gmail.com"));
                //boy0039@gmail.com
                //主題
                em.Subject = subject;
                em.SubjectEncoding = System.Text.Encoding.UTF8;
                //內容
                em.Body = body;
                em.BodyEncoding = System.Text.Encoding.UTF8;
                em.IsBodyHtml = true;	  //信件內容是否使用HTML格式
                //----------------------------------------------------------------------------//
                //Mail Server 設定
                System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                //=== Google 設定 ===//
                //登入帳號認證
                //smtp.Credentials = new System.Net.NetworkCredential("帳號", "密碼");
                //使用587 Port - google要設定
                //smtp.Port = 587;
                //smtp.EnableSsl = true;   //啟動SSL 
                //end of google設定
                //smtp.Host = "smtp.gmail.com";   //SMTP伺服器
                //=================//
                //=== 一般設定 ====//
                //smtp.Host = "msa.hinet.net";   //SMTP伺服器
                //===============//
                //=== IsCom (Office365 設定) == //
                //====登入帳號認證設定 == //
                smtp.UseDefaultCredentials = false;
                //smtp.Credentials = new System.Net.NetworkCredential("biztest@azuretestsandbox.onmicrosoft.com", "Suha7367");
                smtp.Credentials = new System.Net.NetworkCredential(str_Account, str_Pwd);
                //=====================//
                //=====Port 設定=====//
                //使用587 Port - Office365 要設定
                //smtp.Port = 587;
                smtp.Port = int_Port;
                //====================//
                //==== SSL 設定 =======//
                //smtp.EnableSsl = true;   //啟動SSL 
                //啟動SSL 
                if (str_SSL == "Y")
                {
                    smtp.EnableSsl = true;
                }
                else
                {
                    smtp.EnableSsl = false;
                }
                //=====================//
                //end of Office365設定
                //===== Host 設定 ======//
                //smtp.Host = "smtp.office365.com";   //SMTP伺服器
                smtp.Host = str_Host;   //SMTP伺服器
                //=====================//
                //----------------------------------------------------------------------------//
                smtp.Send(em);            //寄出
                status = "Y";
            }
            catch (Exception ex)
            {
                status = "N";
                err_msg = ex.Message;
                //logger.Error(rtn_errmsg(ex));
                msg_write("Error", ex.Message, ex.StackTrace, System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "," + System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            finally
            {
                dw = dt.NewRow();
                dw["status"] = status;
                dw["c_msg"] = c_msg;
                dw["err_msg"] = err_msg;
                dt.Rows.Add(dw);
            }
            return dt;
        }
        #endregion

        #region Return Error_Msg

        public string rtn_errmsg(string err_source, string err_message, string err_stacktrace)
        //public string rtn_errmsg(Exception ex)
        {
            string err_msg = "";
            //string LineNo = ""; //錯誤行號
            ////string LineNo = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber().ToString(); //錯誤行號
            //System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(1, true);
            //string currentName = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetMethod().Name;  //Method
            //string callName = new System.Diagnostics.StackTrace(ex, true).GetFrame(1).GetMethod().Name;     //FileName

            //string err_message = ex.Message;
            //string err_stacktrace = ex.StackTrace;
            //string err_hashcode = ex.GetType().GetHashCode().ToString();
            //string err_fullname = ex.GetType().Name;
            //string err_target_site = ex.TargetSite.ToString();
            //string err_source = ex.Source.ToString();
            //string space_name = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Namespace;
            //string space_fullname = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName;
            //string now_name = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name; //當前類名
            //string now_method = System.Reflection.MethodBase.GetCurrentMethod().Name; //當前方法名
            //string[] err_code;
            //string Loca = "";
            //string cMethod = "";

            //err_code = err_stacktrace.Split('於');
            //cMethod = err_code[err_code.Length - 2];
            //Loca = err_code[err_code.Length - 1];
            //LineNo = ex.StackTrace.Substring(Loca.IndexOf("行") + 1, Loca.Length-(Loca.IndexOf("行") + 1));

            /*
            err_msg = "======================================================================\r\n"
                    + "    發生時間：" + DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "\r\n"
                    + "    檔案名稱：" + sys_name + "\r\n"
                    + "    錯誤位置：" + err_source + "\r\n"
                    + "    錯誤內容：" + err_message + "\r\n"
                    + "    錯誤資訊：" + err_stacktrace + "\r\n";
            */
            err_msg = "\r\n"
                    + "    錯誤位置：" + err_source + "\r\n"
                    //+ "    錯誤行數：" + LineNo + "\r\n"
                    //+ "    HashCode：" + err_hashcode + "\r\n"
                    //+ "    Target：" + err_target_site + "\r\n"
                    + "    錯誤內容：" + err_message + "\r\n"
                    + "    錯誤資訊：" + err_stacktrace;
            return err_msg;
        }
        #endregion Return Error_Msg

        #region Unescape
        public string UnEscape(string str)
        {
            StringBuilder sb = new StringBuilder();
            int len = str.Length;
            int i = 0;
            while (i != len)
            {
                if (Uri.IsHexEncoding(str, i))
                    sb.Append(Uri.HexUnescape(str, ref i));
                else
                    sb.Append(str[i++]);
            }
            return sb.ToString();
        }
        #endregion

        #region 連結字串取得 conn_string
        //連結字串取得
        public string conn_string()
        {
            string connstr = "";
            if (IsLocal == "On")
            {
                connstr = local_conn_str;
            }
            else
            {
                connstr = conn_str;
            }
            return connstr;
        }
        #endregion 連結字串取得 conn_string

        public void msg_write(string msg_kind = "", string msg_content = "", string msg_desc = "", string msg_loca_a = "", string msg_loca_b = "")
        {
            SqlConnection conn = new SqlConnection(conn_string());
            SqlCommand cmd = new SqlCommand();
            string userid = "";
            string msg_loca = "";

            msg_loca = msg_loca_a + "," + msg_loca_b;

            //寫入檔案
            if (LogToFile == "Y")
            {
                switch (msg_kind)
                {
                    case "Error":
                        logger.Error(rtn_errmsg(msg_loca, msg_content, msg_desc));
                        break;
                    case "Debug":
                        logger.Debug(msg_content);
                        break;
                }
            }


            //寫入資料庫
            if (LogToDB == "Y")
            {
                try
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    cmd.Connection = conn;

                    //===============================================================================//
                    //BD_ID, BD_DT, UPD_ID, UPD_DT, MSG_CONTENT, MSG_DESC, MSG_LOCA
                    csql = @"insert into MSG(MSG_CONTENT, MSG_DESC, MSG_LOCA, MSG_KIND) "
                         + "values(@msg_content,@msg_desc,@msg_loca,@msg_kind)";

                    cmd.CommandText = csql;

                    ////讓ADO.NET自行判斷型別轉換
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@msg_content", msg_content);
                    cmd.Parameters.AddWithValue("@msg_desc", msg_desc);
                    cmd.Parameters.AddWithValue("@msg_loca", msg_loca);
                    cmd.Parameters.AddWithValue("@msg_kind", msg_kind);

                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    string errmsg = "";
                    logger.Error(rtn_errmsg(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName + "," + System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace));
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
            }
        }
    }


}