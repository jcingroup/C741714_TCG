using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OutWeb.Controllers
{
    public class ProductsController : Controller
    {
        // GET: Products
        public ProductsController()
        {
            ViewBag.IsFirstPage = false;
        }

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        // 套程式-最新消息
        // 列表
        public ActionResult List()
        {
            return View();
        }

        // 內容
        public ActionResult Content(int? ID)
        {
            if (ID == null)
                return RedirectToAction("List");
            return View();
        }
    }
}