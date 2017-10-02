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
using OutWeb.Modules.Manage;

namespace OutWeb.Controllers
{
    public class ManageController : Controller
    {
        DBService DB = new DBService();

        public ManageController()
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

        // 登入頁
        public ActionResult Login()
        {
            return View();
        }

        //登入檢查
        [HttpPost]
        public ActionResult Login_Chk(string account, string pwd, string ValidCode)
        {
            DataTable user_info;
            string cmsg = "";
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
                        user_info = DB.User_Info(account);
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
                                if (pwd == user_info.Rows[0]["pwd"].ToString())
                                {
                                    //輸入值
                                    Session["IsLogined"] = "Y";
                                    Session["Account"] = account;
                                    Session["Account_Name"] = user_info.Rows[0]["account_name"].ToString();
                                    Session["Rank"] = user_info.Rows[0]["rank"].ToString();
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
                    return RedirectToAction("ProductList");
                }

            }
            catch
            {
                return View("Login");
            }
        }

        //後台登出
        public ActionResult Logout()
        {
            //清除 Session();
            Session.Remove("IsLogined");
            Session.Remove("Account");
            Session.Remove("Account_Name");
            Session.Remove("Rank");

            return Redirect(Url.Content("~/Manage"));
        }

        // 產品管理
        // 分類
        public ActionResult ProductKind()
        {
            return View();
        }
        // 產品資料
        public ActionResult ProductList()
        {
            return View();
        }
        public ActionResult ProductData()
        {
            return View();
        }

        // 最新消息=消息報報
        public ActionResult NewsList()
        {
            return View();
        }
        public ActionResult NewsData()
        {
            return View();
        }

        // 案例分享
        public ActionResult WorksList()
        {
            return View();
        }
        public ActionResult WorksData()
        {
            return View();
        }

        // 代理商
        public ActionResult Agents()
        {
            return View();
        }

        // 修改密碼
        public ActionResult ChangePW()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChangePW(string now_pwd, string new_pwd, string chk_new_pwd)
        {
            string Account = Convert.ToString(Session["Account"]);
            string cmsg = "";
            DataTable user_info;
            //抓取資料
            user_info = DB.User_Info(Account);
            //檢查登入密碼是否正確
            if (now_pwd == user_info.Rows[0]["pwd"].ToString())
            {
                DB.User_Update(Account, new_pwd);
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
                cmsg = "Y";
            }

            TempData["message"] = cmsg;
            return View();

        }
    }
}