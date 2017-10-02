using OutWeb.Models.FrontEnd.ProductFrontEndModels;
using OutWeb.Modules.FontEnd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OutWeb.Controllers
{
    public class ProductsController : Controller
    {
        FontEndModule m_module = new FontEndModule();
        FontEndModule Module { get { return this.m_module; } set { this.m_module = value; } }
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
            List<ProductFrontEndDataModel> model = this.Module.GetProductListFrontEnd();
            return View(model);
        }

        // 內容
        public ActionResult Content(int? ID)
        {
            if (ID == null)
                return RedirectToAction("List");
            ProductFrontEndDataModel model = this.Module.GetProductByIDFrontEnd((int)ID);
            return View(model);
        }
    }
}