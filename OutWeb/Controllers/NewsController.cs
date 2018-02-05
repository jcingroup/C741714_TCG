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
            return RedirectToAction("Latest");
        }

        // 最新訊息
        public ActionResult Latest()
        {
            return View();
        }

        // 最新訊息 - 活動寫真 - 最新活動
        public ActionResult EventLatest()
        {
            return View();
        }
        // 最新訊息 - 活動寫真 - 最新活動內容
        public ActionResult EventLatestContent()
        {
            return View();
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