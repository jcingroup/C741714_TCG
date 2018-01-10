using OutWeb.Models.FrontEnd.ProductFrontEndModels;
using OutWeb.Modules.FontEnd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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

        // 參加台灣法理學院
        public ActionResult School()
        {
            return View();
        }
    }
}