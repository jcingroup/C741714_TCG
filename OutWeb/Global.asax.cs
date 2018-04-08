using OutWeb.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using OutWeb.Service;

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
    }
}
