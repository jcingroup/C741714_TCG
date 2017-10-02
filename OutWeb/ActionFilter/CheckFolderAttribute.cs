using System.IO;
using System.Web;
using System.Web.Mvc;
namespace OutWeb.ActionFilter
{
    public class CheckFolderAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string temp = HttpContext.Current.Server.MapPath("~/Content/Upload/Manage/Images/Temp");
            if (Directory.Exists(temp))
            {
                var files = Directory.GetFiles(temp);
                if (files.Length > 0)
                {
                    foreach (var f in files)
                        File.Delete(f);
                }
                Directory.Delete(temp);
            }

            string[] dirAry = new string[] { "Content", "Upload", "Manage", "Images", "Temp" };
            string serverRoorDir = HttpContext.Current.Server.MapPath("~");
            string dir = string.Empty;
            foreach (string d in dirAry)
            {
                dir = serverRoorDir += @"\" + d;
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
            }
            base.OnActionExecuting(filterContext);
        }
    }
}