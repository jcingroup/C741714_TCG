﻿using System;
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
        //== Class 建立 =========================================//
        DBService DB = new DBService();
        News CNews = new News();
        Language Clang = new Language();
        AboutUs CAboutUs = new AboutUs();
        JoinUs CJoinUs = new JoinUs();
        Edu CEdu = new Edu();
        Focus CFocus = new Focus();
        School CSchool = new School();
        States CStates = new States();
        //=== 變數設定  =========================================//
        String Img_Path = "~/Images";
        //=== Log 記錄 =========================================//
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        //=====================================================//
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
                return RedirectToAction("News_List");
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
                    return RedirectToAction("News_List");
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

        #region 圖片上傳 Upload
        public ActionResult Upload(string img_no, string img_sta, string img_cate, string img_sty = "", string img_id = "")
        {
            DataTable img_file;
            DataTable chk_file;

            HttpFileCollection hfc = System.Web.HttpContext.Current.Request.Files;
            string imgPath = "";
            string filename = "";
            string file_name = "";
            string file_path = "../Images/";
            string str_return = "";
            string[] files;
            string chk_sty = "";
            string pre_filename = "";
            int files_count = 0;
            //string cmsg = "";
            string err_msg = "";
            string new_filename = "";


            if (hfc.Count > 0)
            {
                file_name = hfc[0].FileName;
                files = file_name.Split('\\');
                files_count = files.Count();
                filename = files[files_count - 1];
                new_filename = img_cate + "_" + img_no + "_" + img_sta + "_" + filename;
                imgPath = file_path + new_filename;
                string PhysicalPath = Server.MapPath(imgPath);
                hfc[0].SaveAs(PhysicalPath);
            }

            //抓取資料
            chk_file = DB.Img_List(ref err_msg, img_no, img_sty, img_cate);

            chk_sty = "add";
            if (img_sta == "S")
            {
                if (chk_file.Rows.Count > 0)
                {
                    pre_filename = file_path + chk_file.Rows[0]["img_file"].ToString();

                    chk_sty = "update";
                }
            }

            switch (chk_sty)
            {
                case "add": //加入到資料庫
                    DB.Img_Insert(img_no, new_filename, img_sty, img_cate);
                    
                    break;
                case "update":
                    DB.Img_Update(img_id, img_no, new_filename, img_sty, img_cate);
                    
                    //刪除原本檔案
                    if (pre_filename == "")
                    {
                        string Pre_Path = Server.MapPath(pre_filename);

                        // Delete a file by using File class static method...
                        if (System.IO.File.Exists(Pre_Path))
                        {
                            // Use a try block to catch IOExceptions, to
                            // handle the case of the file already being
                            // opened by another process.
                            try
                            {
                                System.IO.File.Delete(Pre_Path);
                            }
                            catch (System.IO.IOException e)
                            {
                                str_return = str_return + e.Message;
                            }
                        }
                    }


                    break;
            }

            //抓取資料
            img_file = DB.Img_List(ref err_msg, img_no, img_sty, img_cate);

            str_return = JsonConvert.SerializeObject(img_file, Newtonsoft.Json.Formatting.Indented);

            return Content(str_return);


        }
        #endregion

        #region 圖片刪除 Img_Del
        public ActionResult Img_Del(string img_id, string img_sta, string img_no, string img_cate, string img_sty = "")
        {
            string str_return = "";
            DataTable img_file;
            DataTable chk_file;
            string filename = "";
            string file_path = "../Images/";
            string imgPath = "";
            string err_msg = "";

            //抓取資料
            chk_file = DB.Img_List(ref err_msg, img_no, img_sty, img_cate,img_id);

            filename = "";

            if (chk_file.Rows.Count > 0)
            {
                for (int i = 0; i < chk_file.Rows.Count; i++)
                {
                    if (img_id == chk_file.Rows[i]["id"].ToString())
                    {
                        filename = chk_file.Rows[i]["img_file"].ToString();
                        break;
                    }
                }
            }

            imgPath = file_path + filename;

            string PhysicalPath = Server.MapPath(imgPath);

            // Delete a file by using File class static method...
            if (System.IO.File.Exists(PhysicalPath))
            {
                // Use a try block to catch IOExceptions, to
                // handle the case of the file already being
                // opened by another process.
                try
                {
                    System.IO.File.Delete(PhysicalPath);
                }
                catch (System.IO.IOException e)
                {
                    str_return = str_return + e.Message;
                }
            }

            //刪除資料庫資料
            DB.Img_Delete(img_id);

            //抓取資料
            img_file = DB.Img_List(ref err_msg, img_no, img_sty, img_cate);

            str_return = JsonConvert.SerializeObject(img_file, Newtonsoft.Json.Formatting.Indented);
            return Content(str_return);
        }
        #endregion

        #region 關於我們 AboutUs

        #region 關於我們_類別

        #region 關於我們_類別_陳列 AboutUs_Cate_List
        public ActionResult AboutUs_Cate_List(string txt_title_query = "", int page = 1, string txt_sort = "", string txt_a_d = "", string txt_show = "", string txt_lang = "")
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
            dt = CAboutUs.Cate_List(ref err_msg, "", c_sort, txt_show, txt_title_query, txt_lang);

            d_lang = Clang.Lang_List(ref err_msg, "");
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

        #region 關於我們_類別_新增 AboutUs_Cate_Add
        public ActionResult AboutUs_Cate_Add()
        {
            string err_msg = "";
            ViewData["action_sty"] = "add";
            DataTable d_lang;
            d_lang = Clang.Lang_List(ref err_msg, "");

            ViewData["d_lang"] = d_lang;
            return View("AboutUs_Cate_Data");
        }
        #endregion

        #region 關於我們_類別_修改 AboutUs_Cate_Edit
        public ActionResult AboutUs_Cate_Edit(string cate_id = "")
        {
            string err_msg = "";
            DataTable dt;
            DataTable d_lang;
            dt = CAboutUs.Cate_List(ref err_msg, cate_id);
            d_lang = Clang.Lang_List(ref err_msg, "");
            ViewData["dt"] = dt;
            ViewData["d_lang"] = d_lang;
            ViewData["action_sty"] = "edit";

            return View("AboutUs_Cate_Data");
        }
        #endregion

        #region 關於我們_類別_刪除 AboutUs_Cate_Del
        public ActionResult AboutUs_Cate_Del(string cate_id = "")
        {
            //OverlookDBService OverlookDB = new OverlookDBService();
            CAboutUs.Cate_Del(cate_id);
            return RedirectToAction("AboutUs_Cate_List");
        }
        #endregion

        #region 關於我們_類別_儲存 AboutUs_Cate_Save
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AboutUs_Cate_Save(string action_sty, string cate_id, string cate_name, string cate_desc, string show, string sort, string lang_id)
        {
            //OverlookDBService OverlookDB = new OverlookDBService();

            switch (action_sty)
            {
                case "add":
                    CAboutUs.Cate_Insert(cate_name, cate_desc, show, sort, lang_id);
                    break;
                case "edit":
                    CAboutUs.Cate_Update(cate_id, cate_name, cate_desc, show, sort, lang_id);
                    break;
            }

            return RedirectToAction("AboutUs_Cate_List");
        }

        #endregion

        #endregion

        #region 關於我們_基本資料

        #region 關於我們_陳列 AboutUs_List
        public ActionResult AboutUs_List(string txt_title_query = "", int page = 1, string txt_sort = "", string txt_a_d = "", string txt_show = "", string txt_lang = "", string txt_cate = "")
        {
            //定義變數
            string c_sort = "";
            DataTable dt;
            DataTable d_lang;
            DataTable d_cate;
            string err_msg = "";

            //排序設定
            if (txt_sort.Trim().Length > 0)
            {
                c_sort = c_sort + "a1." + txt_sort;
            }
            if (txt_a_d.Trim().Length > 0)
            {
                c_sort = c_sort + " " + txt_a_d;
            }

            //抓取資料
            dt = CAboutUs.List(ref err_msg, "", c_sort, txt_show, txt_title_query, txt_cate, txt_lang);
            //語系
            d_lang = Clang.Lang_List(ref err_msg, "");
            //類別
            d_cate = CAboutUs.Cate_List(ref err_msg, "", "sort", "Y", "", txt_lang);
            //設定傳值
            ViewData["page"] = page;
            ViewData["dt"] = dt;
            ViewData["d_lang"] = d_lang;
            ViewData["d_cate"] = d_cate;
            ViewData["txt_title_query"] = txt_title_query;
            ViewData["txt_sort"] = txt_sort;
            ViewData["txt_a_d"] = txt_a_d;
            ViewData["txt_lang"] = txt_lang;
            ViewData["txt_cate"] = txt_cate;

            return View();
        }
        #endregion

        #region 關於我們_新增 AboutUs_Add
        public ActionResult AboutUs_Add()
        {
            //定義變數
            string err_msg = "";
            DataTable d_cate;
            DataTable d_lang;
            //DataTable d_img;
            //抓取消息類別資料

            d_lang = Clang.Lang_List(ref err_msg, "");
            d_cate = CAboutUs.Cate_List(ref err_msg, "", "sort", "Y", "", d_lang.Rows[0]["lang_id"].ToString());
            //d_img = DB.Img_List(ref err_msg, "", "", "AboutUs");
            //設定傳值
            ViewData["d_lang"] = d_lang;
            ViewData["d_cate"] = d_cate;
            //ViewData["d_img"] = d_img;
            ViewData["action_sty"] = "add";

            return View("AboutUs_Data");
        }
        #endregion

        #region 關於我們_修改 AboutUs_Edit
        public ActionResult AboutUs_Edit(string id = "")
        {
            string err_msg = "";

            DataTable d_cate;
            DataTable d_lang;
            DataTable dt;
            //DataTable d_img;
            //抓取類別資料
            dt = CAboutUs.List(ref err_msg, id);
            d_lang = Clang.Lang_List(ref err_msg, "");
            d_cate = CAboutUs.Cate_List(ref err_msg, "", "sort", "Y", "", dt.Rows[0]["lang_id"].ToString());
            //d_img = DB.Img_List(ref err_msg, id, "", "AboutUs");
            //設定傳值
            ViewData["dt"] = dt;
            ViewData["d_lang"] = d_lang;
            ViewData["d_cate"] = d_cate;
            //ViewData["d_img"] = d_img;
            ViewData["action_sty"] = "edit";

            return View("AboutUs_Data");
        }
        #endregion

        #region 關於我們_刪除 AboutUs_Del
        public ActionResult AboutUs_Del(string id = "")
        {
            //OverlookDBService OverlookDB = new OverlookDBService();
            CAboutUs.Del(id);
            return RedirectToAction("AboutUs_List");
        }
        #endregion

        #region 關於我們_儲存 AboutUs_Save
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AboutUs_Save(string action_sty, string id, string c_title, string c_desc, string show, string sort, string lang_id, string cate_id, string img_no)
        {
            //OverlookDBService OverlookDB = new OverlookDBService();
            switch (action_sty)
            {
                case "add":
                    CAboutUs.Insert(c_title, c_desc, show, sort, lang_id, cate_id, img_no);
                    break;
                case "edit":
                    CAboutUs.Update(id, c_title, c_desc, show, sort, lang_id, cate_id);
                    break;
            }

            return RedirectToAction("AboutUs_List");
        }

        #endregion

        #endregion 關於我們_基本資料

        #endregion 關於我們 AboutUs

        #region 加入我們 JoinUs

        #region 加入我們_類別

        #region 加入我們_類別_陳列 JoinUs_Cate_List
        public ActionResult JoinUs_Cate_List(string txt_title_query = "", int page = 1, string txt_sort = "", string txt_a_d = "", string txt_show = "", string txt_lang = "")
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
            dt = CJoinUs.Cate_List(ref err_msg, "", c_sort, txt_show, txt_title_query, txt_lang);

            d_lang = Clang.Lang_List(ref err_msg, "");
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

        #region 加入我們_類別_新增 JoinUs_Cate_Add
        public ActionResult JoinUs_Cate_Add()
        {
            string err_msg = "";
            ViewData["action_sty"] = "add";
            DataTable d_lang;
            d_lang = Clang.Lang_List(ref err_msg, "");

            ViewData["d_lang"] = d_lang;
            return View("JoinUs_Cate_Data");
        }
        #endregion

        #region 加入我們_類別_修改 JoinUs_Cate_Edit
        public ActionResult JoinUs_Cate_Edit(string cate_id = "")
        {
            string err_msg = "";
            DataTable dt;
            DataTable d_lang;
            dt = CJoinUs.Cate_List(ref err_msg, cate_id);
            d_lang = Clang.Lang_List(ref err_msg, "");
            ViewData["dt"] = dt;
            ViewData["d_lang"] = d_lang;
            ViewData["action_sty"] = "edit";

            return View("JoinUs_Cate_Data");
        }
        #endregion

        #region 加入我們_類別_刪除 JoinUs_Cate_Del
        public ActionResult JoinUs_Cate_Del(string cate_id = "")
        {
            //OverlookDBService OverlookDB = new OverlookDBService();
            CJoinUs.Cate_Del(cate_id);
            return RedirectToAction("JoinUs_Cate_List");
        }
        #endregion

        #region 加入我們_類別_儲存 JoinUs_Cate_Save
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult JoinUs_Cate_Save(string action_sty, string cate_id, string cate_name, string cate_desc, string show, string sort, string lang_id)
        {
            //OverlookDBService OverlookDB = new OverlookDBService();

            switch (action_sty)
            {
                case "add":
                    CJoinUs.Cate_Insert(cate_name, cate_desc, show, sort, lang_id);
                    break;
                case "edit":
                    CJoinUs.Cate_Update(cate_id, cate_name, cate_desc, show, sort, lang_id);
                    break;
            }

            return RedirectToAction("JoinUs_Cate_List");
        }

        #endregion

        #endregion

        #region 加入我們_基本資料

        #region 加入我們_陳列 JoinUs_List
        public ActionResult JoinUs_List(string txt_title_query = "", int page = 1, string txt_sort = "", string txt_a_d = "", string txt_show = "", string txt_lang = "", string txt_cate = "")
        {
            //定義變數
            string c_sort = "";
            DataTable dt;
            DataTable d_lang;
            DataTable d_cate;
            string err_msg = "";

            //排序設定
            if (txt_sort.Trim().Length > 0)
            {
                c_sort = c_sort + "a1." + txt_sort;
            }
            if (txt_a_d.Trim().Length > 0)
            {
                c_sort = c_sort + " " + txt_a_d;
            }

            //抓取資料
            dt = CJoinUs.List(ref err_msg, "", c_sort, txt_show, txt_title_query, txt_cate, txt_lang);
            //語系
            d_lang = Clang.Lang_List(ref err_msg, "");
            //類別
            d_cate = CJoinUs.Cate_List(ref err_msg, "", "sort", "Y", "", txt_lang);
            //設定傳值
            ViewData["page"] = page;
            ViewData["dt"] = dt;
            ViewData["d_lang"] = d_lang;
            ViewData["d_cate"] = d_cate;
            ViewData["txt_title_query"] = txt_title_query;
            ViewData["txt_sort"] = txt_sort;
            ViewData["txt_a_d"] = txt_a_d;
            ViewData["txt_lang"] = txt_lang;
            ViewData["txt_cate"] = txt_cate;

            return View();
        }
        #endregion

        #region 加入我們_新增 JoinUs_Add
        public ActionResult JoinUs_Add()
        {
            //定義變數
            string err_msg = "";
            DataTable d_cate;
            DataTable d_lang;
            //DataTable d_img;
            //抓取消息類別資料

            d_lang = Clang.Lang_List(ref err_msg, "");
            d_cate = CJoinUs.Cate_List(ref err_msg, "", "sort", "Y", "", d_lang.Rows[0]["lang_id"].ToString());
            //d_img = DB.Img_List(ref err_msg, "", "", "AboutUs");
            //設定傳值
            ViewData["d_lang"] = d_lang;
            ViewData["d_cate"] = d_cate;
            //ViewData["d_img"] = d_img;
            ViewData["action_sty"] = "add";

            return View("JoinUs_Data");
        }
        #endregion

        #region 加入我們_修改 JoinUs_Edit
        public ActionResult JoinUs_Edit(string id = "")
        {
            string err_msg = "";

            DataTable d_cate;
            DataTable d_lang;
            DataTable dt;
            //DataTable d_img;
            //抓取類別資料
            dt = CJoinUs.List(ref err_msg, id);
            d_lang = Clang.Lang_List(ref err_msg, "");
            d_cate = CJoinUs.Cate_List(ref err_msg, "", "sort", "Y", "", dt.Rows[0]["lang_id"].ToString());
            //d_img = DB.Img_List(ref err_msg, id, "", "AboutUs");
            //設定傳值
            ViewData["dt"] = dt;
            ViewData["d_lang"] = d_lang;
            ViewData["d_cate"] = d_cate;
            //ViewData["d_img"] = d_img;
            ViewData["action_sty"] = "edit";

            return View("JoinUs_Data");
        }
        #endregion

        #region 加入我們_刪除 JoinUs_Del
        public ActionResult JoinUs_Del(string id = "")
        {
            //OverlookDBService OverlookDB = new OverlookDBService();
            CJoinUs.Del(id);
            return RedirectToAction("JoinUs_List");
        }
        #endregion

        #region 加入我們_儲存 JoinUs_Save
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult JoinUs_Save(string action_sty, string id, string c_title, string c_desc, string show, string sort, string lang_id, string cate_id, string img_no)
        {
            //OverlookDBService OverlookDB = new OverlookDBService();
            switch (action_sty)
            {
                case "add":
                    CJoinUs.Insert(c_title, c_desc, show, sort, lang_id, cate_id, img_no);
                    break;
                case "edit":
                    CJoinUs.Update(id, c_title, c_desc, show, sort, lang_id, cate_id);
                    break;
            }

            return RedirectToAction("JoinUs_List");
        }

        #endregion

        #endregion 加入我們_基本資料

        #endregion 加入我們 JoinUs

        #region 教育專欄 Edu

        #region 教育專欄_類別

        #region 教育專欄_類別_陳列 Edu_Cate_List
        public ActionResult Edu_Cate_List(string txt_title_query = "", int page = 1, string txt_sort = "", string txt_a_d = "", string txt_show = "", string txt_lang = "")
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

            //抓取類別資料
            dt = CEdu.Cate_List(ref err_msg, "", c_sort, txt_show, txt_title_query, txt_lang);

            d_lang = Clang.Lang_List(ref err_msg, "");
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

        #region 教育專欄_類別_新增 Edu_Cate_Add
        public ActionResult Edu_Cate_Add()
        {
            string err_msg = "";
            ViewData["action_sty"] = "add";
            DataTable d_lang;
            d_lang = Clang.Lang_List(ref err_msg, "");

            ViewData["d_lang"] = d_lang;
            return View("Edu_Cate_Data");
        }
        #endregion

        #region 教育專欄_類別_修改 Edu_Cate_Edit
        public ActionResult Edu_Cate_Edit(string cate_id = "")
        {
            string err_msg = "";
            DataTable dt;
            DataTable d_lang;
            dt = CEdu.Cate_List(ref err_msg, cate_id);
            d_lang = Clang.Lang_List(ref err_msg, "");
            ViewData["dt"] = dt;
            ViewData["d_lang"] = d_lang;
            ViewData["action_sty"] = "edit";

            return View("Edu_Cate_Data");
        }
        #endregion

        #region 教育專欄_類別_刪除 Edu_Cate_Del
        public ActionResult Edu_Cate_Del(string cate_id = "")
        {
            //OverlookDBService OverlookDB = new OverlookDBService();
            CEdu.Cate_Del(cate_id);
            return RedirectToAction("Edu_Cate_List");
        }
        #endregion

        #region 教育專欄_類別_儲存 Edu_Cate_Save
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edu_Cate_Save(string action_sty, string cate_id, string cate_name, string cate_desc, string show, string sort, string lang_id)
        {
            //OverlookDBService OverlookDB = new OverlookDBService();

            switch (action_sty)
            {
                case "add":
                    CEdu.Cate_Insert(cate_name, cate_desc, show, sort, lang_id);
                    break;
                case "edit":
                    CEdu.Cate_Update(cate_id, cate_name, cate_desc, show, sort, lang_id);
                    break;
            }

            return RedirectToAction("Edu_Cate_List");
        }

        #endregion

        #endregion

        #region 教育專欄_基本資料

        #region 教育專欄_陳列 Edu_List
        public ActionResult Edu_List(string txt_title_query = "", int page = 1, string txt_sort = "", string txt_a_d = "", string txt_start_date = "", string txt_end_date = "", string txt_show = "", string txt_lang = "", string txt_cate = "")
        {
            //定義變數
            string c_sort = "";
            DataTable dt;
            DataTable d_lang;
            DataTable d_cate;
            string err_msg = "";

            //排序設定
            if (txt_sort.Trim().Length > 0)
            {
                c_sort = c_sort + "a1." + txt_sort;
            }
            if (txt_a_d.Trim().Length > 0)
            {
                c_sort = c_sort + " " + txt_a_d;
            }

            //抓取資料
            dt = CEdu.List(ref err_msg, "", c_sort, txt_show, txt_title_query, txt_start_date, txt_end_date, txt_cate, txt_lang);
            //語系
            d_lang = Clang.Lang_List(ref err_msg, "");
            //類別
            d_cate = CEdu.Cate_List(ref err_msg, "", "sort", "Y", "", txt_lang);
            //設定傳值
            ViewData["page"] = page;
            ViewData["dt"] = dt;
            ViewData["d_lang"] = d_lang;
            ViewData["d_cate"] = d_cate;
            ViewData["txt_title_query"] = txt_title_query;
            ViewData["txt_sort"] = txt_sort;
            ViewData["txt_a_d"] = txt_a_d;
            ViewData["txt_lang"] = txt_lang;
            ViewData["txt_cate"] = txt_cate;

            return View();
        }
        #endregion

        #region 教育專欄_新增 Edu_Add
        public ActionResult Edu_Add()
        {
            //定義變數
            string err_msg = "";
            DataTable d_cate;
            DataTable d_lang;
            //DataTable d_img;
            //抓取消息類別資料

            d_lang = Clang.Lang_List(ref err_msg, "");
            d_cate = CEdu.Cate_List(ref err_msg, "", "sort", "Y", "", d_lang.Rows[0]["lang_id"].ToString());
            //d_img = DB.Img_List(ref err_msg, "", "", "AboutUs");
            //設定傳值
            ViewData["d_lang"] = d_lang;
            ViewData["d_cate"] = d_cate;
            //ViewData["d_img"] = d_img;
            ViewData["action_sty"] = "add";

            return View("Edu_Data");
        }
        #endregion

        #region 教育專欄_修改 Edu_Edit
        public ActionResult Edu_Edit(string id = "")
        {
            string err_msg = "";

            DataTable d_cate;
            DataTable d_lang;
            DataTable dt;
            //DataTable d_img;
            //抓取類別資料
            dt = CEdu.List(ref err_msg, id);
            d_lang = Clang.Lang_List(ref err_msg, "");
            d_cate = CEdu.Cate_List(ref err_msg, "", "sort", "Y", "", dt.Rows[0]["lang_id"].ToString());
            //d_img = DB.Img_List(ref err_msg, id, "", "AboutUs");
            //設定傳值
            ViewData["dt"] = dt;
            ViewData["d_lang"] = d_lang;
            ViewData["d_cate"] = d_cate;
            //ViewData["d_img"] = d_img;
            ViewData["action_sty"] = "edit";

            return View("Edu_Data");
        }
        #endregion

        #region 教育專欄_刪除 Edu_Del
        public ActionResult Edu_Del(string id = "")
        {
            //OverlookDBService OverlookDB = new OverlookDBService();
            CEdu.Del(id);
            return RedirectToAction("Edu_List");
        }
        #endregion

        #region 教育專欄_儲存 Edu_Save
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edu_Save(string action_sty, string id, string c_title, string c_date, string c_desc, string show, string sort, string lang_id, string cate_id, string img_no)
        {
            //OverlookDBService OverlookDB = new OverlookDBService();
            switch (action_sty)
            {
                case "add":
                    CEdu.Insert(c_title, c_date, c_desc, show, sort, lang_id, cate_id, img_no);
                    break;
                case "edit":
                    CEdu.Update(id, c_title, c_date, c_desc, show, sort, lang_id, cate_id);
                    break;
            }

            return RedirectToAction("Edu_List");
        }

        #endregion

        #endregion 教育專欄_基本資料

        #endregion 教育專欄 Edu

        #region 焦點專欄 Focus

        #region 焦點專欄_類別

        #region 焦點專欄_類別_陳列 Focus_Cate_List
        public ActionResult Focus_Cate_List(string txt_title_query = "", int page = 1, string txt_sort = "", string txt_a_d = "", string txt_show = "", string txt_lang = "")
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

            //抓取類別資料
            dt = CFocus.Cate_List(ref err_msg, "", c_sort, txt_show, txt_title_query, txt_lang);

            d_lang = Clang.Lang_List(ref err_msg, "");
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

        #region 焦點專欄_類別_新增 Focus_Cate_Add
        public ActionResult Focus_Cate_Add()
        {
            string err_msg = "";
            ViewData["action_sty"] = "add";
            DataTable d_lang;
            d_lang = Clang.Lang_List(ref err_msg, "");

            ViewData["d_lang"] = d_lang;
            return View("Focus_Cate_Data");
        }
        #endregion

        #region 焦點專欄_類別_修改 Focus_Cate_Edit
        public ActionResult Focus_Cate_Edit(string cate_id = "")
        {
            string err_msg = "";
            DataTable dt;
            DataTable d_lang;
            dt = CFocus.Cate_List(ref err_msg, cate_id);
            d_lang = Clang.Lang_List(ref err_msg, "");
            ViewData["dt"] = dt;
            ViewData["d_lang"] = d_lang;
            ViewData["action_sty"] = "edit";

            return View("Focus_Cate_Data");
        }
        #endregion

        #region 焦點專欄_類別_刪除 Focus_Cate_Del
        public ActionResult Focus_Cate_Del(string cate_id = "")
        {
            //OverlookDBService OverlookDB = new OverlookDBService();
            CFocus.Cate_Del(cate_id);
            return RedirectToAction("Focus_Cate_List");
        }
        #endregion

        #region 焦點專欄_類別_儲存 Focus_Cate_Save
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Focus_Cate_Save(string action_sty, string cate_id, string cate_name, string cate_desc, string show, string sort, string lang_id)
        {
            //OverlookDBService OverlookDB = new OverlookDBService();

            switch (action_sty)
            {
                case "add":
                    CFocus.Cate_Insert(cate_name, cate_desc, show, sort, lang_id);
                    break;
                case "edit":
                    CFocus.Cate_Update(cate_id, cate_name, cate_desc, show, sort, lang_id);
                    break;
            }

            return RedirectToAction("Focus_Cate_List");
        }

        #endregion

        #endregion

        #region 焦點專欄_基本資料

        #region 焦點專欄_陳列 Focus_List
        public ActionResult Focus_List(string txt_title_query = "", int page = 1, string txt_sort = "", string txt_a_d = "", string txt_start_date = "", string txt_end_date = "", string txt_show = "", string txt_index = "", string txt_lang = "", string txt_cate = "")
        {
            //定義變數
            string c_sort = "";
            DataTable dt;
            DataTable d_lang;
            DataTable d_cate;
            string err_msg = "";

            //排序設定
            if (txt_sort.Trim().Length > 0)
            {
                c_sort = c_sort + "a1." + txt_sort;
            }
            if (txt_a_d.Trim().Length > 0)
            {
                c_sort = c_sort + " " + txt_a_d;
            }

            //抓取資料
            dt = CFocus.List(ref err_msg, "", c_sort, txt_show, txt_title_query, txt_start_date, txt_end_date, txt_index, txt_cate, txt_lang);
            //語系
            d_lang = Clang.Lang_List(ref err_msg, "");
            //類別
            d_cate = CFocus.Cate_List(ref err_msg, "", "sort", "Y", "", txt_lang);
            //設定傳值
            ViewData["page"] = page;
            ViewData["dt"] = dt;
            ViewData["d_lang"] = d_lang;
            ViewData["d_cate"] = d_cate;
            ViewData["txt_title_query"] = txt_title_query;
            ViewData["txt_sort"] = txt_sort;
            ViewData["txt_a_d"] = txt_a_d;
            ViewData["txt_lang"] = txt_lang;
            ViewData["txt_cate"] = txt_cate;
            ViewData["txt_index"] = txt_index;

            return View();
        }
        #endregion

        #region 焦點專欄_新增 Focus_Add
        public ActionResult Focus_Add()
        {
            //定義變數
            string err_msg = "";
            DataTable d_cate;
            DataTable d_lang;
            DataTable d_img;
            //抓取消息類別資料

            d_lang = Clang.Lang_List(ref err_msg, "");
            d_cate = CFocus.Cate_List(ref err_msg, "", "sort", "Y", "", d_lang.Rows[0]["lang_id"].ToString());
            d_img = DB.Img_List(ref err_msg, "", "", "Focus");
            //設定傳值
            ViewData["d_lang"] = d_lang;
            ViewData["d_cate"] = d_cate;
            ViewData["d_img"] = d_img;
            ViewData["action_sty"] = "add";

            return View("Focus_Data");
        }
        #endregion

        #region 焦點專欄_修改 Focus_Edit
        public ActionResult Focus_Edit(string id = "")
        {
            string err_msg = "";

            DataTable d_cate;
            DataTable d_lang;
            DataTable dt;
            DataTable d_img;
            //抓取類別資料
            dt = CFocus.List(ref err_msg, id);
            d_lang = Clang.Lang_List(ref err_msg, "");
            d_cate = CFocus.Cate_List(ref err_msg, "", "sort", "Y", "", dt.Rows[0]["lang_id"].ToString());
            d_img = DB.Img_List(ref err_msg, id, "", "Focus");
            //設定傳值
            ViewData["dt"] = dt;
            ViewData["d_lang"] = d_lang;
            ViewData["d_cate"] = d_cate;
            ViewData["d_img"] = d_img;
            ViewData["action_sty"] = "edit";

            return View("Focus_Data");
        }
        #endregion

        #region 焦點專欄_刪除 Focus_Del
        public ActionResult Focus_Del(string id = "")
        {
            //OverlookDBService OverlookDB = new OverlookDBService();
            CFocus.Del(id);
            return RedirectToAction("Focus_List");
        }
        #endregion

        #region 焦點專欄_儲存 Focus_Save
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Focus_Save(string action_sty, string id, string c_title, string c_date, string c_desc, string show,string hot, string sort, string lang_id, string cate_id, string img_no)
        {
            //OverlookDBService OverlookDB = new OverlookDBService();
            switch (action_sty)
            {
                case "add":
                    CFocus.Insert(c_title, c_date, c_desc, show, hot, sort, lang_id, cate_id, img_no);
                    break;
                case "edit":
                    CFocus.Update(id, c_title, c_date, c_desc, show, hot, sort, lang_id, cate_id);
                    break;
            }

            return RedirectToAction("Focus_List");
        }

        #endregion

        #endregion 焦點專欄_基本資料

        #endregion 焦點專欄 Focus


        #region 法理學院_直播

        #region 直播_陳列 School_Video_List
        public ActionResult School_Video_List(string txt_title_query = "", int page = 1, string txt_sort = "", string txt_a_d = "", string txt_show = "", string txt_lang = "")
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

            //抓取直播資料
            dt = CSchool.Video_List(ref err_msg, "", c_sort, txt_show, txt_title_query, txt_lang);

            d_lang = Clang.Lang_List(ref err_msg, "");
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

        #region 直播_新增 School_Video_Add
        public ActionResult School_Video_Add()
        {
            string err_msg = "";
            ViewData["action_sty"] = "add";
            DataTable d_lang;
            d_lang = Clang.Lang_List(ref err_msg, "");

            ViewData["d_lang"] = d_lang;
            return View("School_Video_Data");
        }
        #endregion

        #region 直播_修改 School_Video_Edit
        public ActionResult School_Video_Edit(string video_id = "")
        {
            string err_msg = "";
            DataTable dt;
            DataTable d_lang;
            dt = CSchool.Video_List(ref err_msg, video_id);
            d_lang = Clang.Lang_List(ref err_msg, "");
            ViewData["dt"] = dt;
            ViewData["d_lang"] = d_lang;
            ViewData["action_sty"] = "edit";

            return View("School_Video_Data");
        }
        #endregion

        #region 直播_刪除 School_Video_Del
        public ActionResult School_Video_Del(string video_id = "")
        {
            //OverlookDBService OverlookDB = new OverlookDBService();
            CSchool.Video_Del(video_id);
            return RedirectToAction("School_Video_List");
        }
        #endregion

        #region 直播_儲存 School_Video_Save
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult School_Video_Save(string action_sty, string video_id, string c_url, string show, string sort, string lang_id)
        {
            //OverlookDBService OverlookDB = new OverlookDBService();

            switch (action_sty)
            {
                case "add":
                    CSchool.Video_Insert(c_url, show, sort, lang_id);
                    break;
                case "edit":
                    CSchool.Video_Update(video_id, c_url, show, sort, lang_id);
                    break;
            }

            return RedirectToAction("School_Video_List");
        }

        #endregion

        #endregion 法理學院_直播

        #region 法理學院_歷屆合照 School
        
        #region 法理學院_歷屆合照_類別 School_Cate

        #region 法理學院_歷屆合照_類別_陳列 School_Cate_List
        public ActionResult School_Cate_List(string txt_title_query = "", int page = 1, string txt_sort = "", string txt_a_d = "", string txt_show = "", string txt_lang = "")
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
            dt = CSchool.Cate_List(ref err_msg, "", c_sort, txt_show, txt_title_query, txt_lang);

            d_lang = Clang.Lang_List(ref err_msg, "");
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

        #region 法理學院_歷屆合照_類別_新增 School_Cate_Add
        public ActionResult School_Cate_Add()
        {
            string err_msg = "";
            ViewData["action_sty"] = "add";
            DataTable d_lang;
            d_lang = Clang.Lang_List(ref err_msg, "");

            ViewData["d_lang"] = d_lang;
            return View("School_Cate_Data");
        }
        #endregion

        #region 法理學院_歷屆合照_類別_修改 School_Cate_Edit
        public ActionResult School_Cate_Edit(string cate_id = "")
        {
            string err_msg = "";
            DataTable dt;
            DataTable d_lang;
            dt = CSchool.Cate_List(ref err_msg, cate_id);
            d_lang = Clang.Lang_List(ref err_msg, "");
            ViewData["dt"] = dt;
            ViewData["d_lang"] = d_lang;
            ViewData["action_sty"] = "edit";

            return View("School_Cate_Data");
        }
        #endregion

        #region 法理學院_歷屆合照_類別_刪除 School_Cate_Del
        public ActionResult School_Cate_Del(string cate_id = "")
        {
            //OverlookDBService OverlookDB = new OverlookDBService();
            CSchool.Cate_Del(cate_id);
            return RedirectToAction("School_Cate_List");
        }
        #endregion

        #region 法理學院_歷屆合照_類別_儲存 School_Cate_Save
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult School_Cate_Save(string action_sty, string cate_id, string cate_name, string cate_desc, string show, string sort, string lang_id)
        {
            //OverlookDBService OverlookDB = new OverlookDBService();

            switch (action_sty)
            {
                case "add":
                    CSchool.Cate_Insert(cate_name, cate_desc, show, sort, lang_id);
                    break;
                case "edit":
                    CSchool.Cate_Update(cate_id, cate_name, cate_desc, show, sort, lang_id);
                    break;
            }

            return RedirectToAction("School_Cate_List");
        }

        #endregion

        #endregion

        #region 法理學院_歷屆合照_基本資料

        #region 法理學院_歷屆合照_陳列 School_List
        public ActionResult School_List(string txt_title_query = "", int page = 1, string txt_sort = "", string txt_a_d = "", string txt_start_date = "", string txt_end_date = "", string txt_show = "", string txt_lang = "", string txt_cate = "")
        {
            //定義變數
            string c_sort = "";
            DataTable dt;
            DataTable d_lang;
            DataTable d_cate;
            string err_msg = "";

            //排序設定
            if (txt_sort.Trim().Length > 0)
            {
                c_sort = c_sort + "a1." + txt_sort;
            }
            if (txt_a_d.Trim().Length > 0)
            {
                c_sort = c_sort + " " + txt_a_d;
            }

            //抓取資料
            dt = CSchool.List(ref err_msg, "", c_sort, txt_show, txt_title_query, txt_start_date, txt_end_date, txt_cate, txt_lang);
            //語系
            d_lang = Clang.Lang_List(ref err_msg, "");
            //類別
            d_cate = CSchool.Cate_List(ref err_msg, "", "sort", "Y", "", txt_lang);
            //設定傳值
            ViewData["page"] = page;
            ViewData["dt"] = dt;
            ViewData["d_lang"] = d_lang;
            ViewData["d_cate"] = d_cate;
            ViewData["txt_title_query"] = txt_title_query;
            ViewData["txt_sort"] = txt_sort;
            ViewData["txt_a_d"] = txt_a_d;
            ViewData["txt_lang"] = txt_lang;
            ViewData["txt_cate"] = txt_cate;

            return View();
        }
        #endregion

        #region 法理學院_歷屆合照_新增 School_Add
        public ActionResult School_Add()
        {
            //定義變數
            string err_msg = "";
            DataTable d_cate;
            DataTable d_lang;
            DataTable d_img;
            //抓取消息類別資料

            d_lang = Clang.Lang_List(ref err_msg, "");
            d_cate = CSchool.Cate_List(ref err_msg, "", "sort", "Y", "", d_lang.Rows[0]["lang_id"].ToString());
            d_img = DB.Img_List(ref err_msg, "", "", "School");
            //設定傳值
            ViewData["d_lang"] = d_lang;
            ViewData["d_cate"] = d_cate;
            ViewData["d_img"] = d_img;
            ViewData["action_sty"] = "add";

            return View("School_Data");
        }
        #endregion

        #region 法理學院_歷屆合照_修改 School_Edit
        public ActionResult School_Edit(string id = "")
        {
            string err_msg = "";

            DataTable d_cate;
            DataTable d_lang;
            DataTable dt;
            DataTable d_img;
            //抓取類別資料
            dt = CSchool.List(ref err_msg, id);
            d_lang = Clang.Lang_List(ref err_msg, "");
            d_cate = CSchool.Cate_List(ref err_msg, "", "sort", "Y", "", dt.Rows[0]["lang_id"].ToString());
            d_img = DB.Img_List(ref err_msg, id, "", "School");
            //設定傳值
            ViewData["dt"] = dt;
            ViewData["d_lang"] = d_lang;
            ViewData["d_cate"] = d_cate;
            ViewData["d_img"] = d_img;
            ViewData["action_sty"] = "edit";

            return View("School_Data");
        }
        #endregion

        #region 法理學院_歷屆合照_刪除 School_Del
        public ActionResult School_Del(string id = "")
        {
            //OverlookDBService OverlookDB = new OverlookDBService();
            CSchool.Del(id);
            return RedirectToAction("School_List");
        }
        #endregion

        #region 法理學院_歷屆合照_儲存 School_Save
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult School_Save(string action_sty, string id, string c_title, string c_date, string c_desc, string show, string sort, string lang_id, string cate_id, string img_no)
        {
            //OverlookDBService OverlookDB = new OverlookDBService();
            switch (action_sty)
            {
                case "add":
                    CSchool.Insert(c_title, c_date, c_desc, show, sort, lang_id, cate_id, img_no);
                    break;
                case "edit":
                    CSchool.Update(id, c_title, c_date, c_desc, show, sort, lang_id, cate_id);
                    break;
            }

            return RedirectToAction("School_List");
        }

        #endregion

        #endregion 法理學院_歷屆合照_基本資料

        #endregion 法理學院_歷屆合照 School
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


        #region 活動寫真 各洲活動 Activity_States

        #region 各洲活動_類別

        #region 各洲活動_類別_陳列 States_Cate_List
        public ActionResult States_Cate_List(string txt_title_query = "", int page = 1, string txt_sort = "", string txt_a_d = "", string txt_show = "", string txt_lang = "")
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
            dt = CStates.Cate_List(ref err_msg, "", c_sort, txt_show, txt_title_query, txt_lang);

            d_lang = Clang.Lang_List(ref err_msg, "");
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

        #region 各洲活動_類別_新增 States_Cate_Add
        public ActionResult States_Cate_Add()
        {
            string err_msg = "";
            ViewData["action_sty"] = "add";
            DataTable d_lang;
            d_lang = Clang.Lang_List(ref err_msg, "");

            ViewData["d_lang"] = d_lang;
            return View("States_Cate_Data");
        }
        #endregion

        #region 各洲活動_類別_修改 States_Cate_Edit
        public ActionResult States_Cate_Edit(string cate_id = "")
        {
            string err_msg = "";
            DataTable dt;
            DataTable d_lang;
            dt = CStates.Cate_List(ref err_msg, cate_id);
            d_lang = Clang.Lang_List(ref err_msg, "");
            ViewData["dt"] = dt;
            ViewData["d_lang"] = d_lang;
            ViewData["action_sty"] = "edit";

            return View("AboutUs_Cate_Data");
        }
        #endregion

        #region 各洲活動_類別_刪除 States_Cate_Del
        public ActionResult States_Cate_Del(string cate_id = "")
        {
            //OverlookDBService OverlookDB = new OverlookDBService();
            CStates.Cate_Del(cate_id);
            return RedirectToAction("States_Cate_List");
        }
        #endregion

        #region 各洲活動_類別_儲存 States_Cate_Save
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult States_Cate_Save(string action_sty, string cate_id, string cate_name, string cate_desc, string show, string sort, string lang_id)
        {
            //OverlookDBService OverlookDB = new OverlookDBService();

            switch (action_sty)
            {
                case "add":
                    CStates.Cate_Insert(cate_name, cate_desc, show, sort, lang_id);
                    break;
                case "edit":
                    CStates.Cate_Update(cate_id, cate_name, cate_desc, show, sort, lang_id);
                    break;
            }

            return RedirectToAction("States_Cate_List");
        }

        #endregion

        #endregion

        #region 各洲活動_直播

        #region 各洲活動_直播_陳列 States_Video_List
        public ActionResult States_Video_List(string txt_title_query = "", int page = 1, string txt_sort = "", string txt_a_d = "", string txt_show = "", string txt_lang = "", string txt_cate = "")
        {
            //定義變數
            string c_sort = "";
            DataTable dt;
            DataTable d_lang;
            DataTable d_cate;
            string err_msg = "";

            //排序設定
            if (txt_sort.Trim().Length > 0)
            {
                c_sort = c_sort + "a1." + txt_sort;
            }
            if (txt_a_d.Trim().Length > 0)
            {
                c_sort = c_sort + " " + txt_a_d;
            }

            //抓取資料
            dt = CStates.Video_List(ref err_msg, "", c_sort, txt_show, txt_title_query, txt_cate, txt_lang);
            //語系
            d_lang = Clang.Lang_List(ref err_msg, "");
            //類別
            d_cate = CStates.Cate_List(ref err_msg, "", "sort", "Y", "", txt_lang);
            //設定傳值
            ViewData["page"] = page;
            ViewData["dt"] = dt;
            ViewData["d_lang"] = d_lang;
            ViewData["d_cate"] = d_cate;
            ViewData["txt_title_query"] = txt_title_query;
            ViewData["txt_sort"] = txt_sort;
            ViewData["txt_a_d"] = txt_a_d;
            ViewData["txt_lang"] = txt_lang;
            ViewData["txt_cate"] = txt_cate;

            return View();
        }
        #endregion

        #region 各洲活動_直播_新增 States_Video_Add
        public ActionResult States_Video_Add()
        {
            //定義變數
            string err_msg = "";
            DataTable d_cate;
            DataTable d_lang;
            //DataTable d_img;
            //抓取消息類別資料

            d_lang = Clang.Lang_List(ref err_msg, "");
            d_cate = CStates.Cate_List(ref err_msg, "", "sort", "Y", "", d_lang.Rows[0]["lang_id"].ToString());
            //d_img = DB.Img_List(ref err_msg, "", "", "AboutUs");
            //設定傳值
            ViewData["d_lang"] = d_lang;
            ViewData["d_cate"] = d_cate;
            //ViewData["d_img"] = d_img;
            ViewData["action_sty"] = "add";

            return View("States_Video_Data");
        }
        #endregion

        #region 各洲活動_直播_修改 States_Video_Edit
        public ActionResult States_Video_Edit(string id = "")
        {
            string err_msg = "";

            DataTable d_cate;
            DataTable d_lang;
            DataTable dt;
            //DataTable d_img;
            //抓取類別資料
            dt = CAboutUs.List(ref err_msg, id);
            d_lang = Clang.Lang_List(ref err_msg, "");
            d_cate = CStates.Cate_List(ref err_msg, "", "sort", "Y", "", dt.Rows[0]["lang_id"].ToString());
            //d_img = DB.Img_List(ref err_msg, id, "", "AboutUs");
            //設定傳值
            ViewData["dt"] = dt;
            ViewData["d_lang"] = d_lang;
            ViewData["d_cate"] = d_cate;
            //ViewData["d_img"] = d_img;
            ViewData["action_sty"] = "edit";

            return View("States_Video_Data");
        }
        #endregion

        #region 各洲活動_直播_刪除 States_Video_Del
        public ActionResult States_Video_Del(string id = "")
        {
            //OverlookDBService OverlookDB = new OverlookDBService();
            CAboutUs.Del(id);
            return RedirectToAction("States_Video_List");
        }
        #endregion

        #region 各洲活動_直播_儲存 States_Video_Save
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult States_Video_Save(string action_sty, string id, string c_url, string c_desc, string show, string sort, string lang_id, string cate_id, string img_no)
        {
            //OverlookDBService OverlookDB = new OverlookDBService();
            switch (action_sty)
            {
                case "add":
                    CStates.Video_Insert(c_url, c_desc, show, sort, lang_id, cate_id, img_no);
                    break;
                case "edit":
                    CStates.Video_Update(id, c_url, c_desc, show, sort, lang_id, cate_id);
                    break;
            }

            return RedirectToAction("States_Video_List");
        }

        #endregion

        #endregion 各洲活動_直播資料

        #endregion 各洲活動

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


        #region 最新消息

        #region 消息類別

        #region 消息類別_陳列 News_Cate_List
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

        #region 消息陳列 News_List
        public ActionResult News_List(string txt_title_query = "", int page = 1, string txt_sort = "", string txt_a_d = "", string txt_start_date = "", string txt_end_date = "", string txt_show = "", string txt_index = "",string txt_lang = "", string txt_cate = "")
        {
            //定義變數
            string c_sort = "";
            DataTable dt;
            DataTable d_lang;
            DataTable d_cate;
            string err_msg = "";

            //排序設定
            if (txt_sort.Trim().Length > 0)
            {
                c_sort = c_sort + "a1." + txt_sort;
            }
            if (txt_a_d.Trim().Length > 0)
            {
                c_sort = c_sort + " " + txt_a_d;
            }

            //抓取消息資料
            dt = CNews.News_List(ref err_msg, "", c_sort, txt_show, txt_title_query, txt_start_date, txt_end_date, txt_index,txt_cate,txt_lang);
            //語系
            d_lang = Clang.Lang_List(ref err_msg, "");
            //消息類別
            d_cate = CNews.News_Cate_List(ref err_msg, "", "sort", "Y","",txt_lang);
            //設定傳值
            ViewData["page"] = page;
            ViewData["dt"] = dt;
            ViewData["d_lang"] = d_lang;
            ViewData["d_cate"] = d_cate;
            ViewData["txt_title_query"] = txt_title_query;
            ViewData["txt_start_date"] = txt_start_date;
            ViewData["txt_end_date"] = txt_end_date;
            ViewData["txt_sort"] = txt_sort;
            ViewData["txt_a_d"] = txt_a_d;
            ViewData["txt_lang"] = txt_lang;
            ViewData["txt_cate"] = txt_cate;

            return View();
        }
        #endregion

        #region 最新消息新增 News_Add
        public ActionResult News_Add()
        {
            //定義變數
            string err_msg = "";
            DataTable d_cate;
            DataTable d_lang;
            DataTable d_img;
            //抓取消息類別資料
            
            d_lang = Clang.Lang_List(ref err_msg, "");
            d_cate = CNews.News_Cate_List(ref err_msg, "", "sort", "Y", "", d_lang.Rows[0]["lang_id"].ToString());
            d_img = DB.Img_List(ref err_msg, "", "", "News");
            //設定傳值
            ViewData["d_lang"] = d_lang;
            ViewData["d_cate"] = d_cate;
            ViewData["d_img"] = d_img;
            ViewData["action_sty"] = "add";

            return View("News_Data");
        }
        #endregion


        #region 最新消息修改 News_Edit
        public ActionResult News_Edit(string n_id = "")
        {
            string err_msg = "";
            
            DataTable d_cate;
            DataTable d_lang;
            DataTable d_news;
            DataTable d_img;
            //抓取消息類別資料
            d_news = CNews.News_List(ref err_msg, n_id);
            d_lang = Clang.Lang_List(ref err_msg, "");
            d_cate = CNews.News_Cate_List(ref err_msg, "", "sort", "Y", "", d_news.Rows[0]["lang_id"].ToString());
            d_img = DB.Img_List(ref err_msg, n_id, "", "News");
            //設定傳值
            ViewData["d_news"] = d_news;
            ViewData["d_lang"] = d_lang;
            ViewData["d_cate"] = d_cate;
            ViewData["d_img"] = d_img;
            ViewData["action_sty"] = "edit";

            return View("News_Data");
        }
        #endregion

        #region 最新消息刪除 News_Del
        public ActionResult News_Del(string n_id = "")
        {
            //OverlookDBService OverlookDB = new OverlookDBService();
            CNews.News_Del(n_id);
            return RedirectToAction("News_List");
        }
        #endregion

        #region 最新消息儲存 News_Save
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult News_Save(string action_sty, string n_id, string n_title, string n_date, string n_desc, string show, string hot, string sort,string lang_id, string cate_id, string img_no)
        {
            //OverlookDBService OverlookDB = new OverlookDBService();
            string n_memo = "";
            switch (action_sty)
            {
                case "add":
                    CNews.News_Insert(n_title, n_date, n_desc, show, hot, sort, n_memo,lang_id,cate_id,img_no);
                    break;
                case "edit":
                    CNews.News_Update(n_id, n_title, n_date, n_desc, show, hot, sort, n_memo,lang_id,cate_id);
                    break;
            }

            return RedirectToAction("News_List");
        }

        #endregion

        #endregion 最新消息

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

        #region ajax_get

        #region 類別 Cate_get
        public ActionResult Cate_Get(string lang, string cate_kind)
        {
            string str_return = "";
            string err_msg = "";
            DataTable dt;
            switch (cate_kind)
            {
                case "News":
                    dt = CNews.News_Cate_List(ref err_msg, "", "sort", "Y", "", lang);
                    str_return = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.Indented);
                    break;
                case "AboutUs":
                    dt = CAboutUs.Cate_List(ref err_msg, "", "sort", "Y", "", lang);
                    str_return = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.Indented);
                    break;
                case "JoinUs":
                    dt = CJoinUs.Cate_List(ref err_msg, "", "sort", "Y", "", lang);
                    str_return = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.Indented);
                    break;
                case "Edu":
                    dt = CEdu.Cate_List(ref err_msg, "", "sort", "Y", "", lang);
                    str_return = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.Indented);
                    break;
                case "Focus":
                    dt = CFocus.Cate_List(ref err_msg, "", "sort", "Y", "", lang);
                    str_return = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.Indented);
                    break;
                case "School":
                    dt = CSchool.Cate_List(ref err_msg, "", "sort", "Y", "", lang);
                    str_return = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.Indented);
                    break;
                case "States":
                    dt = CStates.Cate_List(ref err_msg, "", "sort", "Y", "", lang);
                    str_return = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.Indented);
                    break;
            }
            

            return Content(str_return);
        }
        #endregion

        #endregion
    }
}