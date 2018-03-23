using OutWeb.Models.FrontModels.News.AnnouncementLatest;
using OutWeb.Models.FrontModels.News.EventLatestModels;
using OutWeb.Models.FrontModels.News.EventStatesModels;
using OutWeb.Models.FrontModels.News.FocusNewsModels;

/*Json.NET相關的命名空間*/

using OutWeb.Repositories;
using OutWeb.Service;
using System;
using System.Web.Mvc;
using System.Linq;
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

            EventLatestRepository repo = new EventLatestRepository();
            EventLatestResult mdoel = repo.GetList(filter);

            return View(mdoel);
        }

        // 最新訊息 - 活動寫真 - 最新活動內容
        public ActionResult EventLatestContent(int? ID, int? pagingID)
        {
            if (!ID.HasValue)
                return RedirectToAction("EventLatest");
            string langCd = string.Empty;
            EventLatestRepository repo = new EventLatestRepository();
            EventContent model = repo.GetContentByID((int)ID, langCd);
            if (pagingID != null)
            {
                model.PagingID = (int)pagingID;
                var pagFirst = model.Data.PagingList.Where(s => s.ID == (int)pagingID).FirstOrDefault();
                if (pagFirst == null)
                    return RedirectToAction("EventLatest");
                pagFirst.Current = "current";
            }
            else
            {
                if (model.Data.PagingList.Count > 0)
                    model.Data.PagingList.First().Current = "current";
            }
            return View(model);
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
        public ActionResult EventStatesContent(int? statesTypeID, int? ID, int? pagingID)
        {
            if (!statesTypeID.HasValue || !ID.HasValue)
                return RedirectToAction("EventStatesList");
            string langCd = string.Empty;
            EventStatesRepository repo = new EventStatesRepository();
            EventStatesContent model = repo.GetContentByID((int)statesTypeID, (int)ID, langCd);
            if (pagingID != null)
            {
                model.PagingID = (int)pagingID;
                var pagFirst = model.Data.PagingList.Where(s => s.ID == (int)pagingID).FirstOrDefault();
                if (pagFirst == null)
                    return RedirectToAction("EventLatest");
                pagFirst.Current = "current";
            }
            else
            {
                if (model.Data.PagingList.Count > 0)
                    model.Data.PagingList.First().Current = "current";
            }
            return View(model);
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
        public ActionResult AnnouncementList(int? typeID, int? page, string langCode = "zh-tw")
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

            TempData["CateInfo"] = repo.GetNewsCate(langCode);
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
            return View(mdoel);
        }

        // 焦點專欄 - 內容
        public ActionResult FocusContent(int? focusTypeID, int? ID, int? pagingID)
        {
            if (!focusTypeID.HasValue || !ID.HasValue)
                return RedirectToAction("EventLatest");
            string langCd = string.Empty;
            FocusRepository repo = new FocusRepository();
            FocusNewsContent model = repo.GetContentByID((int)focusTypeID, (int)ID, langCd);
            if (pagingID != null)
            {
                model.PagingID = (int)pagingID;
                var pagFirst = model.Data.PagingList.Where(s => s.ID == (int)pagingID).FirstOrDefault();
                if (pagFirst == null)
                    return RedirectToAction("FocusList", new { focusTypeID });
                pagFirst.Current = "current";
            }
            else
            {
                if (model.Data.PagingList.Count > 0)
                    model.Data.PagingList.First().Current = "current";
            }
            return View(model);
        }
    }
}