using System;
using System.Web;
using System.Web.Mvc;
using RentHouseMVC.Models.ViewModels;

namespace RentHouseMVC.Controllers
{
    /// <summary>
    /// Trang lỗi thân thiện. Được gọi từ Application_Error (Global.asax) và từ httpErrors (Web.config).
    /// </summary>
    public class ErrorController : Controller
    {
        public const string ExceptionKey = "RentHouseMVC.LastException";

        // GET: /Error/NotFound - Lỗi 404
        public ActionResult NotFound()
        {
            Response.StatusCode = 404;
            Response.TrySkipIisCustomErrors = true;
            ViewBag.IsErrorPage = true;
            return View();
        }

        // GET: /Error/ServerError - Lỗi 500
        public ActionResult ServerError()
        {
            var exception = HttpContext.Items[ExceptionKey] as Exception;

            int statusCode = 500;
            var httpException = exception as HttpException;
            if (httpException != null && httpException.GetHttpCode() >= 400 && httpException.GetHttpCode() < 600)
            {
                statusCode = httpException.GetHttpCode();
            }

            var model = new ErrorViewModel
            {
                StatusCode = statusCode,
                Title = "Đã có lỗi xảy ra",
                Message = "Hệ thống đang gặp sự cố hoặc yêu cầu không thể xử lý. Vui lòng thử lại sau ít phút."
            };

            if (exception is HttpRequestValidationException)
            {
                model.StatusCode = 400;
                model.Title = "Dữ liệu không hợp lệ";
                model.Message = "Nội dung bạn nhập có chứa ký tự không được phép (ví dụ: < hoặc >). Vui lòng quay lại và nhập lại.";
            }
            else if (httpException != null && httpException.WebEventCode == System.Web.Management.WebEventCodes.RuntimeErrorPostTooLarge)
            {
                model.StatusCode = 413;
                model.Title = "Dữ liệu gửi lên quá lớn";
                model.Message = "File tải lên vượt quá dung lượng cho phép. Ảnh nhà phải nhỏ hơn 2MB.";
            }

            // Chỉ hiện chi tiết kỹ thuật khi chạy trên máy cá nhân (localhost) để dễ sửa lỗi
            if (exception != null && Request.IsLocal)
            {
                model.Detail = exception.GetType().FullName + ": " + exception.Message
                    + (exception.InnerException != null ? Environment.NewLine + "Inner: " + exception.GetBaseException().Message : string.Empty);
            }

            Response.StatusCode = model.StatusCode;
            Response.TrySkipIisCustomErrors = true;
            ViewBag.IsErrorPage = true;
            return View(model);
        }
    }
}
