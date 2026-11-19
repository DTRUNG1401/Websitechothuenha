using System.Linq;
using System.Web.Mvc;
using RentHouseMVC.Helpers;
using RentHouseMVC.Models;
using RentHouseMVC.Models.ViewModels;

namespace RentHouseMVC.Areas.Admin.Controllers
{
    public class AccountController : AdminBaseController
    {
        // GET: /Admin/Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (Session[SessionKeys.AdminId] != null)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        // POST: /Admin/Account/Login
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Truy vấn qua EF (tham số hóa), so sánh mật khẩu dạng SHA256
            string username = model.Username.Trim();
            Account account = db.Accounts.FirstOrDefault(a => a.Username == username);
            if (account == null || !PasswordHelper.Verify(model.Password, account.Password))
            {
                ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không đúng.");
                return View(model);
            }

            Session[SessionKeys.AdminId] = account.AccountId;
            Session[SessionKeys.AdminUsername] = account.Username;
            Session[SessionKeys.AdminFullName] = account.FullName;

            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }
            return RedirectToAction("Index", "Dashboard");
        }

        // POST: /Admin/Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Login");
        }

        // GET: /Admin/Account/ChangePassword
        public ActionResult ChangePassword()
        {
            return View(new ChangePasswordViewModel());
        }

        // POST: /Admin/Account/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            int accountId = (int)Session[SessionKeys.AdminId];
            Account account = db.Accounts.Find(accountId);
            if (account == null)
            {
                Session.Clear();
                return RedirectToAction("Login");
            }

            if (!PasswordHelper.Verify(model.OldPassword, account.Password))
            {
                ModelState.AddModelError("OldPassword", "Mật khẩu hiện tại không đúng.");
                return View(model);
            }

            if (model.NewPassword == model.OldPassword)
            {
                ModelState.AddModelError("NewPassword", "Mật khẩu mới phải khác mật khẩu hiện tại.");
                return View(model);
            }

            account.Password = PasswordHelper.HashSHA256(model.NewPassword);
            db.SaveChanges();

            SetSuccess("Đổi mật khẩu thành công. Lần đăng nhập sau hãy dùng mật khẩu mới.");
            return RedirectToAction("Index", "Dashboard");
        }
    }
}
