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

namespace OutWeb.Controllers
{
    public class JoinUsController : Controller
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
        //=== Log 記錄 =========================================//
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        //=====================================================//


        // GET: JoinUs
        public JoinUsController()
        {
            ViewBag.IsFirstPage = false;
        }

        public ActionResult Index()
        {
            return RedirectToAction("Apply");
        }

        // 申請民政府身分證
        public ActionResult Apply()
        {
            //抓取資料
            string cate_id = "1";
            DataTable dt;
            string err_msg = "";
            string lang_id = "zh-tw";
            dt = CJoinUs.List(ref err_msg, "", "sort desc", "Y", "", cate_id, lang_id);

            ViewData["dt"] = dt;
            return View();
        }

        // 地方服務處諮詢
        public ActionResult Consult()
        {
            //抓取資料
            string cate_id = "2";
            DataTable dt;
            string err_msg = "";
            string lang_id = "zh-tw";
            dt = CJoinUs.List(ref err_msg, "", "sort desc", "Y", "", cate_id, lang_id);

            ViewData["dt"] = dt;
            return View();
        }

        // 參加台灣法理學院 - 課堂 LIVE 直播
        public ActionResult Live()
        {
            DataTable dt;
            string err_msg = "";
            string lang_id = "zh-tw";
            dt = Cschool.Video_List(ref err_msg, "", "sort desc", "Y", "", lang_id);
            ViewData["dt"] = dt;
            return View();
        }

        // 參加台灣法理學院 - 開課時間表
        public ActionResult Schedule()
        {
            //抓取資料
            string cate_id = "3";
            DataTable dt;
            string err_msg = "";
            string lang_id = "zh-tw";
            dt = CJoinUs.List(ref err_msg, "", "sort desc", "Y", "", cate_id, lang_id);

            ViewData["dt"] = dt;
            return View();
        }

        // 參加台灣法理學院 - 歷屆合照
        public ActionResult Gallery()
        {
            DataTable dt;
            string err_msg = "";
            string lang_id = "zh-tw";
            dt = Cschool.List(ref err_msg, "", " sort desc , cate_id desc ", "Y", "", "", "", "", lang_id);
            ViewData["dt"] = dt;
            return View();
        }
    }
}