﻿using OutWeb.Enums;
using OutWeb.Models.FrontModels.SearchSiteModels;
using OutWeb.Modules.FrontEnd;
using OutWeb.Service;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace OutWeb.Controllers
{
    public class HomeController : LanguageController
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
        public ActionResult Search(string str, int? page)
        {
            //======語系取得========
            string lang_id = GetLang();
            //======================

            SearchListViewModel model = new SearchListViewModel();
            model.Filter.CurrentPage = page ?? 1;
            model.Filter.QueryString = str;
            model.Filter.LangCode = lang_id;
            SearchRepository repo = new SearchRepository();
            model.Result = repo.SearchSite(model.Filter);
            return View(model);
        }
    }
}