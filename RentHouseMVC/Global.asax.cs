using System;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Routing;
using RentHouseMVC.Controllers;

namespace RentHouseMVC
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            // Ẩn header "X-AspNetMvc-Version" (không lộ phiên bản framework)
            MvcHandler.DisableMvcResponseHeader = true;

            // Header X-Frame-Options đã khai báo trong Web.config, tránh bị gửi 2 lần
            AntiForgeryConfig.SuppressXFrameOptionsHeader = true;

            // Thông báo lỗi kiểu dữ liệu (số, ngày) bằng tiếng Việt - xem App_GlobalResources/ValidationMessages.resx
            ClientDataTypeModelValidatorProvider.ResourceClassKey = "ValidationMessages";
            DefaultModelBinder.ResourceClassKey = "ValidationMessages";

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        /// <summary>
        /// Bắt mọi lỗi chưa được xử lý và hiển thị trang lỗi thân thiện (404 / 500).
        /// </summary>
        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            if (exception == null)
            {
                return;
            }

            if (exception is HttpUnhandledException && exception.InnerException != null)
            {
                exception = exception.InnerException;
            }

            int statusCode = 500;
            var httpException = exception as HttpException;
            if (httpException != null)
            {
                statusCode = httpException.GetHttpCode();
            }
            else if (exception is ArgumentException && exception.Source == "System.Web.Mvc")
            {
                // Thiếu hoặc sai tham số trên URL (VD: /House/Details/abc) => coi như không tìm thấy trang
                statusCode = 404;
            }

            if (statusCode != 404)
            {
                System.Diagnostics.Trace.TraceError("[RentHouseMVC] " + exception);
            }

            Server.ClearError();
            Response.Clear();
            Response.TrySkipIisCustomErrors = true;
            Context.Items[ErrorController.ExceptionKey] = exception;

            var routeData = new RouteData();
            routeData.Values["controller"] = "Error";
            routeData.Values["action"] = statusCode == 404 ? "NotFound" : "ServerError";
            routeData.Values["code"] = statusCode;

            try
            {
                IController controller = new ErrorController();
                controller.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));
            }
            catch (Exception ex)
            {
                // Trường hợp chính trang lỗi cũng gặp lỗi: trả về thông báo dạng text đơn giản
                System.Diagnostics.Trace.TraceError("[RentHouseMVC] Lỗi khi hiển thị trang lỗi: " + ex);
                Response.Clear();
                Response.StatusCode = 500;
                Response.ContentType = "text/plain";
                Response.Write("Đã có lỗi xảy ra. Vui lòng thử lại sau.");
            }
        }
    }
}
