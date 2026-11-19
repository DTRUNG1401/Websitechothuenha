using System.Web.Mvc;
using RentHouseMVC.Models;

namespace RentHouseMVC.Areas.Admin.Controllers
{
    /// <summary>
    /// Lớp cha của các controller quản trị: dùng chung DbContext và hàm tiện ích.
    /// Việc kiểm tra đăng nhập do AdminAuthorizeAttribute (đăng ký toàn cục) đảm nhận.
    /// </summary>
    public abstract class AdminBaseController : Controller
    {
        protected readonly RentHouseDBContext db = new RentHouseDBContext();

        protected void SetSuccess(string message)
        {
            TempData["Success"] = message;
        }

        protected void SetError(string message)
        {
            TempData["Error"] = message;
        }

        /// <summary>
        /// Quay lại trang trước (chỉ chấp nhận URL nội bộ để chống Open Redirect).
        /// </summary>
        protected ActionResult RedirectToLocal(string returnUrl, string fallbackAction)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction(fallbackAction);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
