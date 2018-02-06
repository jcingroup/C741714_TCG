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

        // 公司簡介
        public ActionResult AboutUs()
        {
            return View();
        }

        // 聯絡我們
        public ActionResult ContactUs()
        {
            return View();
        }
    }
}