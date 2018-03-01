using OutWeb.Enums;
using OutWeb.Service;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace OutWeb.Controllers
{
    public class HomeController : Controller
    {
        public HomeController()
        {
            ViewBag.IsFirstPage = false;
        }

        // all 靜態
        public ActionResult Index()
        {
            ViewBag.IsFirstPage = true;

            return View();
        }

        // 全文檢索
        public ActionResult Search()
        {
            return View();
        }
    }
}