using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OutWeb.Service;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Specialized;
/*Json.NET相關的命名空間*/
using Newtonsoft.Json;

namespace OutWeb.Controllers
{
    public class _SysAdmController : Controller
    {
        DBService DB = new DBService();
        News CNews = new News();
        Language Clang = new Language();
        String Img_Path = "~/Images";

        //Log 記錄
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public _SysAdmController()
        {
            ViewBag.IsFirstPage = false;
        }

        // GET: Manage
        // 後台首頁-導向Login
        public ActionResult Index()
        {
            if (Convert.ToString(Session["IsLogined"]) == "Y")
            {
                return RedirectToAction("NewsList");
            }
            else
            {
                return View("Login");
            }
        }

        #region 登入 Login
        // 登入頁
        public ActionResult Login()
        {
            return View();
        }
        #endregion

        #region 登入檢查 Login_Chk
        //登入檢查
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login_Chk(string account, string pwd, string ValidCode)
        {
            DataTable user_info;
            string cmsg = "";
            string err_msg = "";
            try
            {
                if (string.IsNullOrWhiteSpace(account))
                {
                    if (cmsg.Trim().Length > 0)
                    {
                        cmsg = cmsg + "\n";
                    }
                    cmsg = cmsg + "請輸入帳號";
                }

                if (string.IsNullOrWhiteSpace(pwd))
                {
                    if (cmsg.Trim().Length > 0)
                    {
                        cmsg = cmsg + "\n";
                    }
                    cmsg = cmsg + "請輸入密碼";
                }

                if (cmsg.Trim().Length == 0)
                {
                    //比對驗證碼
                    if (ValidCode != Session["ValidateCode"].ToString())
                    {
                        if (cmsg.Trim().Length > 0)
                        {
                            cmsg = cmsg + "\n";
                        }
                        cmsg = cmsg + "驗證碼不正確";
                    }
                    else
                    {
                        //抓取user資料
                        user_info = DB.User_Info(ref err_msg, account);
                        //驗證使用者有無資料
                        if (user_info.Rows.Count == 0)
                        {
                            if (cmsg.Trim().Length > 0)
                            {
                                cmsg = cmsg + "\n";
                            }
                            cmsg = cmsg + "無此帳號，請再確認。";
                        }
                        else
                        {
                            if (user_info.Rows[0]["status"].ToString() == "N")
                            {
                                if (cmsg.Trim().Length > 0)
                                {
                                    cmsg = cmsg + "\n";
                                }
                                cmsg = cmsg + "此帳號停用，請再確認。";
                            }
                            else
                            {
                                if (pwd == user_info.Rows[0]["signin_pwd"].ToString())
                                {
                                    //輸入值
                                    Session["IsLogined"] = "Y";
                                    Session["Account"] = user_info.Rows[0]["signin_id"].ToString();
                                    Session["usr_id"] = user_info.Rows[0]["id"].ToString();
                                    Session["usr_name"] = user_info.Rows[0]["usr_name"].ToString();
                                    Session["usr_ename"] = user_info.Rows[0]["usr_ename"].ToString();
                                    Session["usr_rank"] = user_info.Rows[0]["usr_rank"].ToString();

                                    //輸入登入時間
                                    DB.User_Signin(user_info.Rows[0]["id"].ToString());
                                }
                                else
                                {
                                    if (cmsg.Trim().Length > 0)
                                    {
                                        cmsg = cmsg + "\n";
                                    }
                                    cmsg = cmsg + "密碼錯誤，請重新輸入。";
                                }
                            }
                        }
                    }
                }

                if (cmsg.Trim().Length > 0)
                {
                    TempData["message"] = cmsg;
                    return View("Login");
                }
                else
                {
                    return RedirectToAction("NewsList");
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return View("Login");
            }
        }
        #endregion

        #region 後台登出 Logout
        //後台登出
        public ActionResult Logout()
        {
            //清除 Session();
            Session.Remove("IsLogined");
            Session.Remove("Account");
            Session.Remove("usr_id");
            Session.Remove("usr_name");
            Session.Remove("usr_ename");
            Session.Remove("usr_rank");

            return Redirect(Url.Content("~/_SysAdm"));
        }
        #endregion

        #region 修改密碼 ChangePW
        // 修改密碼
        public ActionResult ChangePW()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChangePW(string now_pwd, string new_pwd, string chk_new_pwd)
        {
            string Account = Convert.ToString(Session["Account"]);
            string id = Convert.ToString(Session["usr_id"]);
            string cmsg = "";
            string err_msg = "";
            DataTable user_info;
            //抓取資料
            user_info = DB.User_Info(ref err_msg, Account);
            //檢查登入密碼是否正確
            if (now_pwd == user_info.Rows[0]["pwd"].ToString())
            {
                DB.User_Update(id, new_pwd);
            }
            else
            {
                if (cmsg.Trim().Length > 0)
                {
                    cmsg = cmsg + "\n";
                }
                cmsg = cmsg + "密碼錯誤，請重新輸入。";
            }

            if (cmsg.Trim().Length == 0)
            {
                cmsg = "密碼變更成功，下次登入請輸入新密碼!!!";
            }

            TempData["message"] = cmsg;
            return View();

        }
        #endregion

        #region 驗證碼 GetValidateCode
        //設置將生成的驗證碼存入Session，並輸出驗證碼圖片
        [AllowAnonymous]
        public ActionResult GetValidateCode()
        {
            ValidateCode vCode = new ValidateCode();
            string code = vCode.CreateValidateCode(5);
            Session["ValidateCode"] = code;
            byte[] bytes = vCode.CreateValidateGraphic(code);
            return File(bytes, @"image/jpeg");
        }
        #endregion

        #region ckeditor上傳圖片
        /// <summary>
        /// ckeditor上傳圖片
        /// </summary>
        /// <param name="upload">預設參數叫upload</param>
        /// <param name="CKEditorFuncNum"></param>
        /// <param name="CKEditor"></param>
        /// <param name="langCode"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadPicture(HttpPostedFileBase upload, string CKEditorFuncNum, string CKEditor, string langCode)
        {
            string result = "";
            if (upload != null && upload.ContentLength > 0)
            {
                //儲存圖片至Server
                upload.SaveAs(Server.MapPath(Img_Path + "/" + upload.FileName));


                var imageUrl = Url.Content(Img_Path + "/" + upload.FileName);

                var vMessage = string.Empty;

                result = @"<html><body><script>window.parent.CKEDITOR.tools.callFunction(" + CKEditorFuncNum + ", \"" + imageUrl + "\", \"" + vMessage + "\");</script></body></html>";

            }

            return Content(result);
        }
        #endregion

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

        #region 新聞公告聲明類別
        #region 新聞公告聲明類別_陳列 News_Cate_List()
        public ActionResult News_Cate_List(string txt_title_query = "", int page = 1, string txt_sort = "", string txt_a_d = "", string txt_show = "", string txt_lang = "")
        {
            //定義變數
            string c_sort = "";
            string err_msg = "";
            DataTable dt;
            DataTable d_lang;

            //排序設定
            if (txt_sort.Trim().Length > 0)
            {
                c_sort = c_sort + "a1." + txt_sort;
            }
            if (txt_a_d.Trim().Length > 0)
            {
                c_sort = c_sort + " " + txt_a_d;
            }

            //抓取消息類別資料
            dt = CNews.News_Cate_List(ref err_msg, "", c_sort, txt_show, txt_title_query, txt_lang);

            d_lang = Clang.Lang_List(ref err_msg,"");
            //設定傳值
            ViewData["page"] = page;
            ViewData["dt"] = dt;
            ViewData["d_lang"] = d_lang;
            ViewData["txt_title_query"] = txt_title_query;
            ViewData["txt_sort"] = txt_sort;
            ViewData["txt_a_d"] = txt_a_d;
            ViewData["txt_lang"] = txt_lang;

            return View();
        }
        #endregion

        #region 消息類別_新增 News_Cate_Add
        public ActionResult News_Cate_Add()
        {
            string err_msg = "";
            ViewData["action_sty"] = "add";
            DataTable d_lang;
            d_lang = Clang.Lang_List(ref err_msg, "");

            ViewData["d_lang"] = d_lang;
            return View("News_Cate_Data");
        }
        #endregion

        #region 消息類別_修改 News_Cate_Edit
        public ActionResult News_Cate_Edit(string cate_id = "")
        {
            string err_msg = "";
            DataTable dt = DB.Partner_Cate_List(ref err_msg, cate_id);
            DataTable d_lang;
            dt = CNews.News_Cate_List(ref err_msg, cate_id);
            d_lang = Clang.Lang_List(ref err_msg, "");
            ViewData["dt"] = dt;
            ViewData["d_lang"] = d_lang;
            ViewData["action_sty"] = "edit";

            return View("News_Cate_Data");
        }
        #endregion

        #region 消息類別_刪除 News_Cate_Del
        public ActionResult News_Cate_Del(string cate_id = "")
        {
            //OverlookDBService OverlookDB = new OverlookDBService();
            CNews.News_Cate_Del(cate_id);
            return RedirectToAction("News_Cate_List");
        }
        #endregion

        #region 消息類別_儲存 News_Cate_Save
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult News_Cate_Save(string action_sty, string cate_id, string cate_name, string cate_desc, string show, string sort,string lang_id)
        {
            //OverlookDBService OverlookDB = new OverlookDBService();

            switch (action_sty)
            {
                case "add":
                    CNews.News_Cate_Insert(cate_name, cate_desc, show, sort, lang_id);
                    break;
                case "edit":
                    CNews.News_Cate_Update(cate_id, cate_name, cate_desc, show, sort,lang_id);
                    break;
            }

            return RedirectToAction("News_Cate_List");
        }

        #endregion
        #endregion

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


        //#region 修改密碼

        ///// 管理員密碼變更
        //[HttpGet]
        //public ActionResult ChangePW()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult ChangePW(FormCollection form)
        //{
        //    SignInModule signInModule = new SignInModule();
        //    try
        //    {
        //        signInModule.ChangePassword(form);
        //        ViewBag.Message = "success";
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.Message = ex.Message;
        //    }
        //    return View();
        //}
        //#endregion
    }
}