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
    public class AboutUsController : Controller
    {
        //== Class 建立 =========================================//
        DBService DB = new DBService();
        //News CNews = new News();
        Language Clang = new Language();
        AboutUs CAboutUs = new AboutUs();
        //JoinUs CJoinUs = new JoinUs();
        Edu CEdu = new Edu();
        //Focus CFocus = new Focus();
        //=== 變數設定  =========================================//
        string Img_Path = "~/Images";
        string IsDebug = WebConfigurationManager.AppSettings["Debug"].ToString();
        //=== Log 記錄 =========================================//
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        //=====================================================//

        // GET: AboutUs
        public AboutUsController()
        {
            ViewBag.IsFirstPage = false;
        }

        public ActionResult Index()
        {

            return RedirectToAction("TCG");
        }

        // 台灣民政府由來
        public ActionResult TCG(string id = "")
        {
            //抓取資料
            string cate_id = "1";
            DataTable dt;
            DataTable d_detail;
            DataTable dt1;
            string err_msg = "";
            string lang_id = "zh-tw";
            dt1 = CAboutUs.List(ref err_msg, "", "sort desc", "Y", "", cate_id, lang_id);
            dt = dt1.Copy();
            if(dt1.Rows.Count > 0)
            {
                if(id == "")
                {
                    id = dt1.Rows[0]["id"].ToString();
                }
            }

            dt1 = CAboutUs.List(ref err_msg, id, "sort desc", "Y", "", cate_id, lang_id);
            d_detail = dt1.Copy();

            ViewData["dt"] = dt;
            ViewData["d_detail"] = d_detail;
            ViewData["id"] = id;
            return View();
        }

        // 關於我們 - 主張與立場
        public ActionResult Position()
        {
            //抓取資料
            string cate_id = "2";
            DataTable dt;
            string err_msg = "";
            string lang_id = "zh-tw";
            dt = CAboutUs.List(ref err_msg, "", "sort desc", "Y", "", cate_id, lang_id);

            ViewData["dt"] = dt;
            return View();
        }

        // 關於我們 - 台灣地位的聲明
        public ActionResult Statement()
        {
            //抓取資料
            string cate_id = "3";
            DataTable dt;
            string err_msg = "";
            string lang_id = "zh-tw";
            dt = CAboutUs.List(ref err_msg, "", "sort desc", "Y", "", cate_id, lang_id);

            ViewData["dt"] = dt;
            return View();
        }

        // 關於我們 - 法律依據
        public ActionResult Law()
        {
            //抓取資料
            string cate_id = "4,5,6";
            DataTable dt;
            DataTable d_cate;
            string err_msg = "";
            string lang_id = "zh-tw";
            d_cate = CAboutUs.Cate_List(ref err_msg, cate_id, "sort", "Y", "", lang_id);
            dt = CAboutUs.List(ref err_msg, "", "sort desc", "Y", "", cate_id, lang_id);

            ViewData["d_cate"] = d_cate;
            ViewData["dt"] = dt;

            return View();
        }

        // 教育專欄 - 分類
        public ActionResult EducationCategory()
        {
            //抓取分類
            DataTable dt;
            string err_msg = "";
            string lang_id = "zh-tw";
            dt = CEdu.Cate_List(ref err_msg, "", "sort desc", "Y", "", lang_id);

            ViewData["dt"] = dt;

            return View();
        }

        // 教育專欄 - 列表
        public ActionResult EducationList(string cate_id = "",int page = 1)
        {
            DataTable dt;
            DataTable d_cate;
            string err_msg = "";
            string lang_id = "zh-tw";
            
            d_cate = CEdu.Cate_List(ref err_msg, cate_id, "sort desc", "Y", "", lang_id);
            dt = CEdu.List(ref err_msg, "", "sort desc", "Y", "", "", "", cate_id, lang_id);

            ViewData["dt"] = dt;
            ViewData["d_cate"] = d_cate;
            ViewData["page"] = page;
            return View();
        }

        // 教育專欄 - 內容
        public ActionResult EducationContent(string id="")
        {
            DataTable dt;
            string err_msg = "";
            string lang_id = "zh-tw";
            dt = CEdu.List(ref err_msg, id, "sort desc", "Y", "", "", "", "", lang_id);

            ViewData["dt"] = dt;
            return View();
        }

        // 行政區域圖&組織架構
        public ActionResult Organization()
        {
            //抓取資料
            string cate_id = "7,8,9";
            DataTable dt;
            DataTable d_cate;
            string err_msg = "";
            string lang_id = "zh-tw";
            d_cate = CAboutUs.Cate_List(ref err_msg, cate_id, "sort", "Y", "", lang_id);
            dt = CAboutUs.List(ref err_msg, "", "sort desc", "Y", "", cate_id, lang_id);

            ViewData["d_cate"] = d_cate;
            ViewData["dt"] = dt;

            return View();
        }
    }
}