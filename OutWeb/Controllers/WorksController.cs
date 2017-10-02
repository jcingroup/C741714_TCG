using OutWeb.Models.FrontEnd.WorksFrontEndModels;
using OutWeb.Modules.FontEnd;
using System.Collections.Generic;
using System.Web.Mvc;

namespace OutWeb.Controllers
{
    public class WorksController : Controller
    {
        FontEndModule m_module = new FontEndModule();
        FontEndModule Module { get { return this.m_module; } set { this.m_module = value; } }
        public WorksController()
        {
            ViewBag.IsFirstPage = false;
        }

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        // 套程式-案例分享
        // 列表
        public ActionResult List()
        {
            List<WorksFrontEndDataModel> model = this.Module.GetWorksListFrontEnd();
            return View(model);
        }

        // 內容
        public ActionResult Content(int? ID)
        {
            if (ID == null)
                return RedirectToAction("List");
            WorksFrontEndDataModel model = this.Module.GetWorksByIDFrontEnd((int)ID);
            return View(model);
        }
    }
}