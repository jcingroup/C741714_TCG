using OutWeb.Enums;
using OutWeb.Models.FrontEnd.NewsFrontEndModels;
using OutWeb.Models.Manage.ManageNewsModels;
using OutWeb.Modules.FontEnd;
using OutWeb.Modules.Manage;
using OutWeb.Service;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace OutWeb.Controllers
{
    public class HomeController : Controller
    {
        FontEndModule m_module = new FontEndModule();
        FontEndModule Module { get { return this.m_module; } set { this.m_module = value; } }
        public HomeController()
        {
            ViewBag.IsFirstPage = false;
        }

        // all 靜態
        public ActionResult Index()
        {
            ViewBag.IsFirstPage = true;
            List<NewsFrontEndDataModel> model = this.Module.GetNewsListFrontEnd(3);
            return View(model);
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