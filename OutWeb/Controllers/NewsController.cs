using OutWeb.Models.FrontEnd.NewsFrontEndModels;
using OutWeb.Modules.FontEnd;
using System.Collections.Generic;
using System.Web.Mvc;

namespace OutWeb.Controllers
{
    public class NewsController : Controller
    {
        public NewsController()
        {
            ViewBag.IsFirstPage = false;
        }

        public ActionResult Index()
        {
            return RedirectToAction("Featured");
        }

        // 最新訊息
        public ActionResult Featured()
        {
            return View();
        }

        // 活動寫真 - 最新活動
        public ActionResult LatestEvent()
        {
            return View();
        }

        // 活動寫真 - 歷史活動列表
        public ActionResult EventHistoryList()
        {
            return View();
        }

        // 活動寫真 - 歷史活動內容
        public ActionResult EventHistoryContent()
        {
            return View();
        }

        // 活動寫真 - 各州活動分類
        public ActionResult EventStatesCategory()
        {
            return View();
        }

        // 活動寫真 - 各州活動列表
        public ActionResult EventStatesList()
        {
            return View();
        }

        // 活動寫真 - 各州活動內容
        public ActionResult EventStatesContent()
        {
            return View();
        }

        // 新聞公告聲明 - 最新消息
        public ActionResult LatestNews()
        {
            return View();
        }

        // 新聞公告聲明 - 列表
        public ActionResult NewsList()
        {
            return View();
        }

        // 新聞公告聲明 - 內容
        public ActionResult NewsContent()
        {
            return View();
        }

        // 焦點專欄 - 分類
        public ActionResult FocusCategory()
        {
            return View();
        }

        // 焦點專欄 - 列表
        public ActionResult FocusList()
        {
            return View();
        }

        // 焦點專欄 - 內容
        public ActionResult FocusContent()
        {
            return View();
        }
    }
}