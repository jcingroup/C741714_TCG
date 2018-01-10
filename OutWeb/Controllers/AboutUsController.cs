using OutWeb.Enums;
using OutWeb.Models.Manage.AgentModels;
using OutWeb.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OutWeb.Controllers
{
    public class AboutUsController : Controller
    {
        // GET: AboutUs
        public AboutUsController()
        {
            ViewBag.IsFirstPage = false;
        }

        public ActionResult Index()
        {
            return RedirectToAction("TCG");
        }

        // 什麼是台灣民政府
        public ActionResult TCG()
        {
            return View();
        }

        // 關於我們 - 主張與立場
        public ActionResult Position()
        {
            return View();
        }

        // 關於我們 - 台灣地位的聲明
        public ActionResult Statement()
        {
            return View();
        }

        // 關於我們 - 法律依據
        public ActionResult Law()
        {
            return View();
        }

        // 教育專欄 - 分類
        public ActionResult EducationCategory()
        {
            return View();
        }

        // 教育專欄 - 列表
        public ActionResult EducationList()
        {
            return View();
        }

        // 教育專欄 - 內容
        public ActionResult EducationContent()
        {
            return View();
        }

        // 行政區域圖&組織架構
        public ActionResult Organization()
        {
            return View();
        }
    }
}