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

namespace OutWeb
{
    public class MvcApplication : System.Web.HttpApplication
    {
        Service.Service CService = new Service.Service();

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

            Language Clang = new Language();
            CultureInfo ci = null;
            string lang = string.Empty;
            lang = Request.QueryString["lang"]; //參數切換語系 參數查詢列為高優先權

            if (string.IsNullOrEmpty(lang))
            {
                lang = Request.QueryString["langCode"];
            }

            HttpCookie cookie = Request.Cookies["_culture"];
            if (lang != "" && lang != null)   //點擊其他頁面時
            {
                if (GetCurrentCulture() != lang || cookie.Value != lang)
                {
                    // update cookie value 
                    cookie.Value = lang;
                    Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(lang);
                    Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;// 主要
                }
            }
            else if ((cookie == null) && ((lang == "") || lang == null)) //初次瀏覽頁面時
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
                    Response.Cookies.Add(cookie);
                }
            }
            else //非初次瀏覽頁面
            {
                // resume cookie value 
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cookie.Value);
                Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;// 主要
            }

        }

        public string GetCurrentCulture()
        {
            return System.Threading.Thread.CurrentThread.CurrentCulture.Name;
        }
    }
}
