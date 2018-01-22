using OutWeb.Enums;
using OutWeb.Service;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace OutWeb.Controllers
{
    public class JoinUsController : Controller
    {
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
            return View();
        }

        // 地方服務處諮詢
        public ActionResult Consult()
        {
            return View();
        }

        // 參加台灣法理學院 - 課堂 LIVE 直播
        public ActionResult Live()
        {
            return View();
        }

        // 參加台灣法理學院 - 開課時間表
        public ActionResult Schedule()
        {
            return View();
        }

        // 參加台灣法理學院 - 歷屆合照
        public ActionResult Gallery()
        {
            return View();
        }
    }
}