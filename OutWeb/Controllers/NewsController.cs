using OutWeb.Models.FrontModels.News.EventLatestModels;

/*Json.NET相關的命名空間*/

using OutWeb.Repositories;
using OutWeb.Service;
using System;
using System.Data;
using System.Web.Mvc;

namespace OutWeb.Controllers
{
    public class NewsController : Controller
    {
        //== Class 建立 =========================================//
        private DBService DB = new DBService();

        private News CNews = new News();
        private Language Clang = new Language();

        //AboutUs CAboutUs = new AboutUs();
        //JoinUs CJoinUs = new JoinUs();
        //Edu CEdu = new Edu();
        private Focus CFocus = new Focus();

        //=== 變數設定  =========================================//
        private String Img_Path = "~/Images";

        //=== Log 記錄 =========================================//
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        //=====================================================//

        public NewsController()
        {
            ViewBag.IsFirstPage = false;
        }

        public ActionResult Index()
        {
            return RedirectToAction("Latest");
        }

        // 最新訊息
        public ActionResult Latest()
        {
            return View();
        }

        // 最新訊息 - 活動寫真 - 最新活動
        public ActionResult EventLatest(int? page)
        {
            EventLatestListFilter filter = new EventLatestListFilter()
            {
                CurrentPage = page ?? 1,
            };

            NewEventLatestRepository repo = new NewEventLatestRepository();
            EventLatestResult mdoel = repo.GetList(filter);

            return View(mdoel);
        }

        // 最新訊息 - 活動寫真 - 最新活動內容
        public ActionResult EventLatestContent(int ID)
        {
            string langCd = string.Empty;
            NewEventLatestRepository repo = new NewEventLatestRepository();
            EventContent mdoel = repo.GetContentByID(ID, langCd);
            return View(mdoel);
        }

        // 最新訊息 - 活動寫真 - 各州活動分類
        public ActionResult EventStatesCategory()
        {
            return View();
        }

        // 最新訊息 - 活動寫真 - 各州活動列表
        public ActionResult EventStatesList()
        {
            return View();
        }

        // 最新訊息 - 活動寫真 - 各州活動內容
        public ActionResult EventStatesContent()
        {
            return View();
        }

        // 最新訊息 - 新聞 公告 聲明 - 最新消息
        public ActionResult AnnouncementLatest()
        {
            return View();
        }

        // 最新訊息 - 新聞 公告 聲明 列表
        public ActionResult AnnouncementList()
        {
            return View();
        }

        // 最新訊息 - 新聞 公告 聲明 內容
        public ActionResult AnnouncementContent()
        {
            return View();
        }

        // 焦點專欄 - 分類
        public ActionResult FocusCategory()
        {
            //抓取分類
            DataTable dt;
            string err_msg = "";
            string lang_id = "zh-tw";
            dt = CFocus.Cate_List(ref err_msg, "", "sort desc", "Y", "", lang_id);

            ViewData["dt"] = dt;

            return View();
        }

        // 焦點專欄 - 列表
        public ActionResult FocusList(string cate_id = "", int page = 1)
        {
            DataTable dt;
            DataTable d_cate;
            string err_msg = "";
            string lang_id = "zh-tw";

            d_cate = CFocus.Cate_List(ref err_msg, cate_id, "sort desc", "Y", "", lang_id);
            dt = CFocus.List(ref err_msg, "", "sort desc", "Y", "", "", "", "", cate_id, lang_id);

            ViewData["dt"] = dt;
            ViewData["d_cate"] = d_cate;
            ViewData["page"] = page;

            return View();
        }

        // 焦點專欄 - 內容
        public ActionResult FocusContent(string id = "")
        {
            DataTable dt;
            string err_msg = "";
            string lang_id = "zh-tw";
            dt = CFocus.List(ref err_msg, id, "sort desc", "Y", "", "", "", "", "", lang_id);

            ViewData["dt"] = dt;

            return View();
        }
    }
}