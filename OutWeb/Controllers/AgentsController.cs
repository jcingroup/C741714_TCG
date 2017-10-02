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
    public class AgentsController : Controller
    {
        // GET: Agents
        public AgentsController()
        {
            ViewBag.IsFirstPage = false;
        }

        public ActionResult Index()
        {
            return RedirectToAction("Agents");
        }

        // 套程式-代理商
        public ActionResult Agents()
        {
            ListModuleService module = ListFactoryService.Create(ListMethodType.AGENT);
            AgentDataModel model = (module.DoGetList<object>(null, Language.NotSet) as AgentDataModel);
            return View(model);
        }
    }
}