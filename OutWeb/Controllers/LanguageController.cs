using System.Linq;
using System;
using System.Collections.Generic;
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
using System.Web.Configuration;
using System.Globalization;

namespace OutWeb.Controllers
{
    public class LanguageController : Controller
    {
        HttpCookie langCookie;
        Language Clang = new Language();
        CultureInfo ci;
        public void Change_Lang(string lang)
        {

            HttpCookie cookie = Request.Cookies["_culture"];
            if (lang != "" && lang!=null)   //點擊其他頁面時
            {
                // update cookie value 
                cookie.Value = lang;
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(lang);
                Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;// 主要
            }else if ((cookie == null) && ((lang == "") || lang == null)) //初次瀏覽頁面時
            {
                try
                {
                    //取得用戶端瀏覽器語言喜好設定
                    var userLanguages = Request.UserLanguages;
                    if (userLanguages.Length > 0)
                    {
                        try
                        {
                            ci = new CultureInfo(userLanguages[0]);
                        }
                        catch (CultureNotFoundException)
                        {
                            ci = CultureInfo.InvariantCulture;
                        }
                    }
                    else
                    {
                        ci = CultureInfo.InvariantCulture;
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    var webLang = ci.ToString();
                    // create cookie value 
                    cookie = new HttpCookie("_culture");
                    if (webLang == "en-US" || webLang == "zh-CN" || webLang == "zh-TW")
                    {
                        System.Diagnostics.Debug.WriteLine(webLang);
                    }
                    else if (webLang == "ja")
                    {
                        webLang = "ja-JP";
                    }
                    else
                    {
                        webLang = "en-US";
                    }
                    cookie.Value = webLang;
                    cookie.Expires = DateTime.Now.AddDays(1);
                    Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cookie.Value);
                    Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;// 主要
                }
            }
            else //非初次瀏覽頁面
            {
                // resume cookie value 
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cookie.Value);
                Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;// 主要
            }

            Response.Cookies.Add(cookie);
        } 
        
        // Layout 進版面用
        public string Lang_Page(string pageName)
        {  // 其他頁面用
            HttpCookie cookie = Request.Cookies["_culture"];
            string lang = cookie.Value;
            string pageFullName = "";
            if (lang == "zh-TW")
            {
                pageFullName = pageName;
            }
            else
            {
                pageFullName = pageName + "." + lang;
            }
            return pageFullName;
        }

        //轉換成Table所存語言
        public string GetLang()
        {
            HttpCookie cookie = Request.Cookies["_culture"];
            Change_Lang(cookie.Value);
            switch (cookie.Value) {
                case "en-US":
                    return "en";
                case "zh-TW":
                    return "zh-tw";
                case "zh-CN":
                    return "cn";
                case "ja-JP":
                    return "JPN";
                default:
                    return "";

            }
        }
    }
}