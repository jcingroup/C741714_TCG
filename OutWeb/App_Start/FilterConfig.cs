using System.Web;
using System.Web.Mvc;
using OutWeb.Service;

namespace OutWeb
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
