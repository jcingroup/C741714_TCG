using OutWeb.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using OutWeb.Service;
using System.Globalization;
using System.Threading;
using System.Web.WebPages;

namespace OutWeb
{
    public class MvcApplication : System.Web.HttpApplication
    {
        Service.Service CService = new Service.Service();
        string langname = "_culture";
        //Log 記錄
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        protected void Application_Start()
        {
            BundleTable.EnableOptimizations = false;
            AreaRegistration.RegisterAllAreas();
            RouteTable.Routes.MapMvcAttributeRoutes();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            DisplayModeProvider.Instance.Modes.Insert(0, new DefaultDisplayMode("en-US")
            { //使用狀況條件為 Cookies值 或 QueryString["lang"]
                ContextCondition = (C =>
                (
                C.Request.Cookies[langname] != null &&
                C.Request.Cookies[langname].Value.Contains("en-US") &&
                string.IsNullOrEmpty(C.Request.QueryString["langCode"])
                ) ||
                C.Request.QueryString["langCode"] == "en-US"
                )
            });
            DisplayModeProvider.Instance.Modes.Insert(1, new DefaultDisplayMode("ja-JP")
            { //使用狀況條件為 Cookies值 或 QueryString["lang"]
                ContextCondition = (C =>
                (
                C.Request.Cookies[langname] != null &&
                C.Request.Cookies[langname].Value.Contains("ja-JP") &&
                string.IsNullOrEmpty(C.Request.QueryString["langCode"])
                ) ||
                C.Request.QueryString["langCode"] == "ja-JP"
              )
            });
            DisplayModeProvider.Instance.Modes.Insert(2, new DefaultDisplayMode("zh-CN")
            { //使用狀況條件為 Cookies值 或 QueryString["lang"]
                ContextCondition = (C =>
                (
                C.Request.Cookies[langname] != null &&
                C.Request.Cookies[langname].Value.Contains("zh-CN") &&
                string.IsNullOrEmpty(C.Request.QueryString["langCode"])
                ) ||
                C.Request.QueryString["langCode"] == "zh-CN"
          )
            });
        }

        protected void Application_Error(Object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            string err_msg = "";
            if (exception != null)
            {
                HttpException httpException = exception as HttpException;
                if (httpException != null)
                {
                    int errorCode = httpException.GetHttpCode();
                    if (errorCode == 400 || errorCode == 404)
                    {
                        Response.StatusCode = 404;
                        Response.Redirect(string.Format("~/Error/Error404"), true);
                        Server.ClearError();
                        return;
                    }
                }

                var postData = string.Empty;
                try
                {
                    using (System.IO.Stream stream = Request.InputStream)
                    {
                        using (System.IO.StreamReader streamReader = new System.IO.StreamReader(stream, System.Text.Encoding.UTF8))
                        {
                            postData = streamReader.ReadToEnd();
                        }
                    }
                }
                catch { }

                //該方法為寫錯誤日誌和發送錯誤郵件給開發者的方法（可忽略）
                //LogCache.Instance.saveToLog(Request, AppDomain.CurrentDomain.BaseDirectory + @"\privateFolder\SysLog\Error\", DateTime.Now.ToString("yyyyMMddHH") + ".log", postData, exception.ToString());
                err_msg = exception.Message;
                CService.msg_write("Error", exception.Message, exception.StackTrace, System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);

                Response.StatusCode = 500;
                Response.Redirect(string.Format("~/Error/Error500"), true);
                Server.ClearError();
            }
        }
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            #region language follow
            HttpCookie WebLang = Request.Cookies[langname];
            string set_lang = string.Empty;
            string fource_lang = string.Empty; //預設強制語系 
            string query_lang = string.Empty; //參數設定語系
            string[] allow_lang = new string[] { "zh-TW", "zh-CN", "en-US", "ja-JP" };

            fource_lang = allow_lang[0]; //不預強制語系 此行註解
            query_lang = Request.QueryString["langCode"]; //參數切換語系 參數查詢列為高優先權

            if (!string.IsNullOrEmpty(query_lang) && allow_lang.Contains(query_lang))
            {

                var n = System.Globalization.CultureInfo.CreateSpecificCulture(query_lang);
                set_lang = n.Name;
                //網址參數切換語系
                if (WebLang == null)
                {
                    WebLang = new HttpCookie(langname, set_lang);
                }
                else
                {
                    WebLang.Value = query_lang;            
                }
                Response.Cookies.Add(WebLang);
            }
            else if (WebLang == null)
            {
                if (!string.IsNullOrEmpty(fource_lang)) //採用系統強制設定語系
                {
                    var q = fource_lang;
                    var n = System.Globalization.CultureInfo.CreateSpecificCulture(q); //轉換完整 語系-國家 編碼
                    set_lang = n.Name;
                }
                else if (Request.UserLanguages != null && Request.UserLanguages.Length > 0) //使用瀏覽器提供的語系
                {
                    var q = Request.UserLanguages[0];
                    var n = System.Globalization.CultureInfo.CreateSpecificCulture(q);//轉換完整 語系-國家 編碼

                    if (allow_lang.Contains(n.Name))
                        set_lang = n.Name;
                    else
                        set_lang = allow_lang[0];
                }
                else //提供其他系統直接進行Request但Request Header裡無Accept-Language參數
                {
                    var n = System.Threading.Thread.CurrentThread.CurrentCulture;
                    if (allow_lang.Contains(n.Name))
                        set_lang = n.Name;
                    else
                        set_lang = allow_lang[0];
                }
                WebLang = new HttpCookie(langname, set_lang);
                Response.Cookies.Add(WebLang);
            }
            else
            {
                if (!allow_lang.Contains(WebLang.Value))
                {
                    set_lang = allow_lang[0];
                    WebLang.Value = set_lang;
                }
            }

            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(WebLang.Value);
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(System.Threading.Thread.CurrentThread.CurrentCulture.Name);

            #endregion

        }

        public string GetCurrentCulture()
        {
            return System.Threading.Thread.CurrentThread.CurrentCulture.Name;
        }
    }
}
