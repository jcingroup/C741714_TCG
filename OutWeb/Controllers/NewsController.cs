using System.Collections.Generic;
using System.Web.Mvc;

namespace OutWeb.Controllers
{
    public class NewsController : Controller
    {

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
            //List<NewsFrontEndDataModel> model = this.Module.GetNewsListFrontEnd(0);
            return View();
        }

        // 內容
        public ActionResult Content(int? ID)
        {
            if (ID == null)
                return RedirectToAction("List");
            //NewsFrontEndDetalisDataModel model = this.Module.GetNewsByIDFrontEnd((int)ID);
            return View();
        }
    }
}