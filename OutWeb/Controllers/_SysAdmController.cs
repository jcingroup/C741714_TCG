using OutWeb.Authorize;
using OutWeb.Entities;
using OutWeb.Enums;
using OutWeb.Exceptions;
using OutWeb.Models.Manage.AgentModels;
using OutWeb.Models.Manage.ImgModels;
using OutWeb.Models.Manage.ManageNewsModels;
using OutWeb.Models.Manage.ProductKindModels;
using OutWeb.Models.Manage.ProductModels;
using OutWeb.Models.Manage.WorksModels;
using OutWeb.Modules.Manage;
using OutWeb.Repositories;
using OutWeb.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace OutWeb.Controllers
{
    [Auth]
    [ErrorHandler]
    public class _SysAdmController : Controller
    {

        public _SysAdmController()
        {
            ViewBag.IsFirstPage = false;
        }


        #region 單一網頁編輯
        public ActionResult AboutUs()
        {
            return View();
        }
        public ActionResult JoinUs()
        {
            return View();
        }
        #endregion 單一網頁編輯


        #region 教育專欄 分類
        public ActionResult EduKindList()
        {
            return View();
        }
        #endregion 教育專欄 分類


        #region 教育專欄
        public ActionResult EduDataList()
        {
            return View();
        }
        public ActionResult EduDataAdd()
        {
            return View();
        }
        public ActionResult EduDataEdit()
        {
            return View();
        }
        #endregion 教育專欄


        #region 法理學院 影片管理
        public ActionResult SchoolVideo()
        {
            return View();
        }
        #endregion 法理學院


        #region 法理學院 歷屆合照
        public ActionResult SchoolDataList()
        {
            return View();
        }
        public ActionResult SchoolDataAdd()
        {
            return View();
        }
        public ActionResult SchoolDataEdit()
        {
            return View();
        }
        #endregion 法理學院 歷屆合照


        #region 活動寫真 歷史活動
        public ActionResult EventHistoryList()
        {
            return View();
        }
        public ActionResult EventHistoryAdd()
        {
            return View();
        }
        public ActionResult EventHistoryEdit()
        {
            return View();
        }
        #endregion 活動寫真 歷史活動


        #region 活動寫真 各州活動
        public ActionResult EventStatesList()
        {
            return View();
        }
        public ActionResult EventStatesAdd()
        {
            return View();
        }
        public ActionResult EventStatesEdit()
        {
            return View();
        }
        #endregion 活動寫真 各州活動


        #region 活動寫真 各州影片
        public ActionResult StatesVideoList()
        {
            return View();
        }
        #endregion 活動寫真 各州影片


        #region 新聞公告聲明
        public ActionResult NewsList()
        {
            return View();
        }
        public ActionResult NewsAdd()
        {
            return View();
        }
        public ActionResult NewsEdit()
        {
            return View();
        }
        #endregion 新聞公告聲明


        #region 焦點專欄 分類
        public ActionResult FocusKindList()
        {
            return View();
        }
        #endregion 焦點專欄 分類


        #region 焦點專欄
        public ActionResult FocusDataList()
        {
            return View();
        }
        public ActionResult FocusDataAdd()
        {
            return View();
        }
        public ActionResult FocusDataEdit()
        {
            return View();
        }
        #endregion 焦點專欄


        #region 修改密碼

        /// 管理員密碼變更
        [HttpGet]
        public ActionResult ChangePW()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChangePW(FormCollection form)
        {
            SignInModule signInModule = new SignInModule();
            try
            {
                signInModule.ChangePassword(form);
                ViewBag.Message = "success";
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
            }
            return View();
        }
        #endregion
    }
}