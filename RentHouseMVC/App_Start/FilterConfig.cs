using System.Web.Mvc;
using RentHouseMVC.Filters;

namespace RentHouseMVC
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            // Kiểm tra đăng nhập cho TOÀN BỘ Area Admin.
            // Filter tự bỏ qua các controller phía khách và các action có [AllowAnonymous] (trang đăng nhập).
            filters.Add(new AdminAuthorizeAttribute());
        }
    }
}
