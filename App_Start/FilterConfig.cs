using System.Web;
using System.Web.Mvc;

namespace TrajectoryVerisiIsleme.WepApi
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
