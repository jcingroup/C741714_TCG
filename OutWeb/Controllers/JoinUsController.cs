using System.Linq;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using OutWeb.Service;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Specialized;
/*Json.NET相關的命名空間*/
using Newtonsoft.Json;
using System.Web.Configuration;

namespace OutWeb.Controllers
{
    public class JoinUsController : LanguageController
    {
        //== Class 建立 =========================================//
        DBService DB = new DBService();
        //News CNews = new News();
        Language Clang = new Language();
        //AboutUs CAboutUs = new AboutUs();
        JoinUs CJoinUs = new JoinUs();
        //Edu CEdu = new Edu();
        //Focus CFocus = new Focus();
        School Cschool = new School();
        //=== 變數設定  =========================================//
        String Img_Path = "~/Images";
        string IsDebug = WebConfigurationManager.AppSettings["Debug"].ToString();
        //=== Log 記錄 =========================================//
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        //=====================================================//


        // GET: JoinUs
        public JoinUsController()
        {
            ViewBag.IsFirstPage = false;
        }

        public ActionResult Index(string langCode = "")
        {
            return RedirectToAction("Apply", new { langCode });
        }

        // 申請民政府身分證
        public ActionResult Apply(string id = "")
        {
            //抓取資料
            string cate_id = "1,4,5";
            DataTable dt;
            DataTable d_detail;
            DataTable dt1;
            DataTable d_log;
            string err_msg = "";
            //======語系取得========
            string lang_id = GetLang();
            //======================
            dt1 = CJoinUs.List(ref err_msg, "", "sort desc", "Y", "", cate_id, lang_id);
            dt = dt1.Copy();
            if (dt1.Rows.Count > 0)
            {
                if (id == "")
                {
                    id = dt1.Rows[0]["id"].ToString();
                }
            }

            dt1 = CJoinUs.List(ref err_msg, id, "sort desc", "Y", "", cate_id, lang_id);
            d_detail = dt1.Copy();

            ViewData["dt"] = dt;
            ViewData["d_detail"] = d_detail;
            ViewData["id"] = id;
            //=瀏覽記錄====================
            DB.Log_Insert(id, "JoinUs");
            d_log = DB.Log_List(ref err_msg, id, "JoinUs");
            ViewData["log_count"] = d_log.Rows.Count.ToString();
            //============================
            return View();
        }

        // 地方服務處諮詢
        public ActionResult Consult(string id = "")
        {
            //抓取資料
            string cate_id = "2,6,7";
            DataTable dt;
            DataTable d_detail;
            DataTable dt1;
            DataTable d_log;
            string err_msg = "";
            //======語系取得========
            string lang_id = GetLang();
            //======================
            dt1 = CJoinUs.List(ref err_msg, "", "sort desc", "Y", "", cate_id, lang_id);
            dt = dt1.Copy();
            if (dt1.Rows.Count > 0)
            {
                if (id == "")
                {
                    id = dt1.Rows[0]["id"].ToString();
                }
            }

            dt1 = CJoinUs.List(ref err_msg, id, "sort desc", "Y", "", cate_id, lang_id);
            d_detail = dt1.Copy();

            ViewData["dt"] = dt;
            ViewData["d_detail"] = d_detail;
            ViewData["id"] = id;
            //=瀏覽記錄====================
            DB.Log_Insert(id, "JoinUs");
            d_log = DB.Log_List(ref err_msg, id, "JoinUs");
            ViewData["log_count"] = d_log.Rows.Count.ToString();
            //============================
            return View();
        }

        // 參加台灣法理學院 - 課堂 LIVE 直播
        public ActionResult Live()
        {
            DataTable dt;
            DataTable d_log;
            string err_msg = "";
            //======語系取得========
            string lang_id = GetLang();
            //======================
            dt = Cschool.Video_List(ref err_msg, "", "sort desc", "Y", "", lang_id);
            ViewData["dt"] = dt;
            //=瀏覽記錄====================
            DB.Log_Insert("1", "Vedio");
            d_log = DB.Log_List(ref err_msg, "1", "Vedio");
            ViewData["log_count"] = d_log.Rows.Count.ToString();
            //============================
            return View();
        }

        // 參加台灣法理學院 - 開課時間表
        public ActionResult Schedule()
        {
            //抓取資料
            string cate_id = "3";
            DataTable dt;
            DataTable d_log;
            string log_count = "";
            string err_msg = "";
            //======語系取得========
            string lang_id = GetLang();
            //======================
            dt = CJoinUs.List(ref err_msg, "", "sort desc", "Y", "", cate_id, lang_id);
            //=瀏覽記錄====================
            DB.Log_Insert("1","Schedule");
            d_log = DB.Log_List(ref err_msg, "1", "Schedule");
            ViewData["log_count"] = d_log.Rows.Count.ToString();
            //============================

            ViewData["dt"] = dt;
            return View();
        }

        // 參加台灣法理學院 - 歷屆合照
        public ActionResult Gallery()
        {
            DataTable dt;
            string err_msg = "";
            //======語系取得========
            string lang_id = GetLang();
            //======================
            dt = Cschool.List(ref err_msg, "", " sort desc,cate_id desc ", "Y", "", "", "", "", lang_id); //排序大到小、資料建檔日期新到舊
            ViewData["dt"] = dt;
            return View();
        }
    }
}