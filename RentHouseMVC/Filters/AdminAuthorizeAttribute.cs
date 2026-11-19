using System;
using System.Web.Mvc;
using System.Web.Routing;
using RentHouseMVC.Helpers;

namespace RentHouseMVC.Filters
{
    /// <summary>
    /// Filter kiểm tra đăng nhập cho toàn bộ Area Admin.
    /// Được đăng ký toàn cục trong App_Start/FilterConfig.cs.
    /// - Chỉ áp dụng cho controller thuộc Area "Admin".
    /// - Bỏ qua action / controller có [AllowAnonymous] (trang đăng nhập).
    /// - Chưa đăng nhập (Session["AdminId"] rỗng) => chuyển về trang đăng nhập.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class AdminAuthorizeAttribute : FilterAttribute, IAuthorizationFilter
    {
        private const string AdminArea = "Admin";
        private const string AdminNamespace = "RentHouseMVC.Areas.Admin";

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            if (!IsAdminArea(filterContext))
            {
                return;
            }

            bool allowAnonymous =
                filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true) ||
                filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true);
            if (allowAnonymous)
            {
                return;
            }

            var session = filterContext.HttpContext.Session;
            if (session != null && session[SessionKeys.AdminId] != null)
            {
                return; // Đã đăng nhập
            }

            // Chưa đăng nhập: chuyển tới trang đăng nhập, nhớ lại trang đang mở (chỉ với GET)
            var request = filterContext.HttpContext.Request;
            string returnUrl = request.HttpMethod == "GET" ? request.RawUrl : null;

            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
            {
                { "area", AdminArea },
                { "controller", "Account" },
                { "action", "Login" },
                { "returnUrl", returnUrl }
            });
        }

        private static bool IsAdminArea(AuthorizationContext filterContext)
        {
            var area = filterContext.RouteData.DataTokens["area"] as string;
            if (string.Equals(area, AdminArea, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            // Kiểm tra thêm theo namespace để chắc chắn không controller Admin nào bị bỏ sót
            string controllerNamespace = filterContext.Controller.GetType().Namespace ?? string.Empty;
            return controllerNamespace.StartsWith(AdminNamespace, StringComparison.Ordinal);
        }
    }
}
