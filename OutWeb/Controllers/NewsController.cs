using OutWeb.Models.FrontModels.News.AnnouncementLatest;
using OutWeb.Models.FrontModels.News.EventLatestModels;
using OutWeb.Models.FrontModels.News.EventStatesModels;
using OutWeb.Models.FrontModels.News.FocusNewsModels;

/*Json.NET相關的命名空間*/

using OutWeb.Repositories;
using OutWeb.Service;
using System;
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
            string langCode = string.Empty;
            LatestRepository repo = new LatestRepository();
            var model = repo.GetLatestList(langCode);
            return View(model);
        }

        // 最新訊息 - 活動寫真 - 最新（中央）活動
        public ActionResult EventLatest(int? page, string langCode)
        {
            EventLatestListFilter filter = new EventLatestListFilter()
            {
                CurrentPage = page ?? 1,
                LangCode = langCode
            };

            NewEventLatestRepository repo = new NewEventLatestRepository();
            EventLatestResult mdoel = repo.GetList(filter);

            return View(mdoel);
        }

        // 最新訊息 - 活動寫真 - 最新活動內容
        public ActionResult EventLatestContent(int? ID)
        {
            if (!ID.HasValue)
                return RedirectToAction("EventLatest");
            string langCd = string.Empty;
            NewEventLatestRepository repo = new NewEventLatestRepository();
            EventContent mdoel = repo.GetContentByID((int)ID, langCd);
            return View(mdoel);
        }

        // 最新訊息 - 活動寫真 - 各州活動分類
        public ActionResult EventStatesCategory()
        {
            string langCd = string.Empty;
            EventStatesRepository repo = new EventStatesRepository();
            var mdoel = repo.GetStatesCate(langCd);

            return View(mdoel);
        }

        // 最新訊息 - 活動寫真 - 各州活動列表
        public ActionResult EventStatesList(int? statesTypeID, int? page, string langCode)
        {
            if (!statesTypeID.HasValue)
                return RedirectToAction("EventLatest");
            EventStatesListFilter filter = new EventStatesListFilter()
            {
                CurrentPage = page ?? 1,
                LangCode = langCode
            };

            statesTypeID = statesTypeID ?? 1;

            EventStatesRepository repo = new EventStatesRepository();
            EventStatesResult mdoel = repo.GetList((int)statesTypeID, filter);
            mdoel.StatesTypeID = (int)statesTypeID;
            TempData["StateInfo"] = repo.GetStatesCateByID((int)statesTypeID, langCode);
            return View(mdoel);
        }

        // 最新訊息 - 活動寫真 - 各州活動內容
        public ActionResult EventStatesContent(int? statesTypeID, int? ID)
        {
            if (!statesTypeID.HasValue || !ID.HasValue)
                return RedirectToAction("EventStatesList");
            string langCd = string.Empty;
            EventStatesRepository repo = new EventStatesRepository();
            EventStatesContent mdoel = repo.GetContentByID((int)statesTypeID, (int)ID, langCd);
            return View(mdoel);
        }

        // 最新訊息 - 新聞 公告 聲明 - 最新消息
        public ActionResult AnnouncementLatest(int? page, string langCode)
        {
            AnnouncementLatestFilter filter = new AnnouncementLatestFilter()
            {
                CurrentPage = page ?? 1,
                LangCode = langCode
            };

            AnnouncementLatestRepository repo = new AnnouncementLatestRepository();
            AnnouncementLatestResult mdoel = repo.GetList(filter);

            return View(mdoel);
        }

        // 最新訊息 - 新聞 公告 聲明 列表
        public ActionResult AnnouncementList(int? typeID, int? page, string langCode)
        {
            if (!typeID.HasValue)
                return RedirectToAction("AnnouncementLatest");
            AnnouncementLatestFilter filter = new AnnouncementLatestFilter()
            {
                CurrentPage = page ?? 1,
                TypeID = typeID,
                LangCode = langCode
            };
            AnnouncementLatestRepository repo = new AnnouncementLatestRepository();
            AnnouncementLatestResult mdoel = repo.GetList(filter);
            mdoel.TypeID = (int)filter.TypeID;
            return View(mdoel);
        }

        // 最新訊息 - 新聞 公告 聲明 內容
        public ActionResult AnnouncementContent(int? ID, int? typeID)
        {
            if (!ID.HasValue || !typeID.HasValue)
                return RedirectToAction("AnnouncementLatest");
            string langCd = string.Empty;
            AnnouncementLatestRepository repo = new AnnouncementLatestRepository();
            AnnouncementLatestContent mdoel = repo.GetContentByID((int)ID, (int)typeID, langCd);
            return View(mdoel);
        }

        // 焦點專欄 - 分類
        //public ActionResult FocusCategory()
        //{
        //    //抓取分類
        //    DataTable dt;
        //    string err_msg = "";
        //    string lang_id = "zh-tw";
        //    dt = CFocus.Cate_List(ref err_msg, "", "sort desc", "Y", "", lang_id);

        //    ViewData["dt"] = dt;

        //    return View();
        //}

        //// 焦點專欄 - 列表
        //public ActionResult FocusList(string cate_id = "", int page = 1)
        //{
        //    DataTable dt;
        //    DataTable d_cate;
        //    string err_msg = "";
        //    string lang_id = "zh-tw";

        //    d_cate = CFocus.Cate_List(ref err_msg, cate_id, "sort desc", "Y", "", lang_id);
        //    dt = CFocus.List(ref err_msg, "", "sort desc", "Y", "", "", "", "", cate_id, lang_id);

        //    ViewData["dt"] = dt;
        //    ViewData["d_cate"] = d_cate;
        //    ViewData["page"] = page;

        //    return View();
        //}

        //// 焦點專欄 - 內容
        //public ActionResult FocusContent(string id = "")
        //{
        //    DataTable dt;
        //    string err_msg = "";
        //    string lang_id = "zh-tw";
        //    dt = CFocus.List(ref err_msg, id, "sort desc", "Y", "", "", "", "", "", lang_id);

        //    ViewData["dt"] = dt;

        //    return View();
        //}

        public ActionResult FocusCategory()
        {
            string langCd = string.Empty;
            FocusRepository repo = new FocusRepository();
            var mdoel = repo.GetFocusCate(langCd);

            return View(mdoel);
        }

        // 焦點專欄 - 列表
        public ActionResult FocusList(int? focusTypeID, int? page, string langCode)
        {
            if (!focusTypeID.HasValue)
                return RedirectToAction("EventLatest");
            FocusNewsListFilter filter = new FocusNewsListFilter()
            {
                CurrentPage = page ?? 1,
                LangCode = langCode
            };

            focusTypeID = focusTypeID ?? 1;

            FocusRepository repo = new FocusRepository();
            FocusNewsResult mdoel = repo.GetList((int)focusTypeID, filter);
            TempData["FocusInfo"] = repo.GetFocusCateByID((int)focusTypeID, langCode);
            return View(mdoel);
        }

        // 焦點專欄 - 內容
        public ActionResult FocusContent(int? focusTypeID, int? ID)
        {
            if (!focusTypeID.HasValue || !ID.HasValue)
                return RedirectToAction("FocusList");
            string langCd = string.Empty;
            FocusRepository repo = new FocusRepository();
            FocusNewsContent mdoel = repo.GetContentByID((int)focusTypeID, (int)ID, langCd);
            return View(mdoel);
        }
    }
}