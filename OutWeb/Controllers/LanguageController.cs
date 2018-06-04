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
              
        //轉換成Table所存語言
        public string GetLang()
        {
            HttpCookie cookie = Request.Cookies["_culture"];
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
                    return cookie.Value;

            }
        }

        public string GetCurrentCulture()
        {
            return System.Threading.Thread.CurrentThread.CurrentCulture.Name;
        }
    }
}