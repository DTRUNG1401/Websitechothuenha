using System.Web.Mvc;
using System.Web.Routing;

namespace RentHouseMVC
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            var route = routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "RentHouseMVC.Controllers" }
            );

            // Chỉ tìm controller trong RentHouseMVC.Controllers.
            // Không cho route mặc định "lọt" vào controller của Area Admin (VD: /Dashboard)
            route.DataTokens["UseNamespaceFallback"] = false;
        }
    }
}
