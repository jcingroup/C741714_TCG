using OutWeb.Models.FrontEnd.NewsFrontEndModels;
using OutWeb.Modules.FontEnd;
using System.Collections.Generic;
using System.Web.Mvc;

namespace OutWeb.Controllers
{
    public class NewsController : Controller
    {
        FontEndModule m_module = new FontEndModule();
        FontEndModule Module { get { return this.m_module; } set { this.m_module = value; } }
        public NewsController()
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
            List<NewsFrontEndDataModel> model = this.Module.GetNewsListFrontEnd(0);
            return View(model);
        }

        // 內容
        public ActionResult Content(int? ID)
        {
            if (ID == null)
                return RedirectToAction("List");
            NewsFrontEndDetalisDataModel model = this.Module.GetNewsByIDFrontEnd((int)ID);
            return View(model);
        }
    }
}