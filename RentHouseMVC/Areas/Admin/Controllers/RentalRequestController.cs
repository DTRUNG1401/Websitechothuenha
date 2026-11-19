using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using RentHouseMVC.Helpers;
using RentHouseMVC.Models;
using RentHouseMVC.Models.ViewModels;

namespace RentHouseMVC.Areas.Admin.Controllers
{
    public class RentalRequestController : AdminBaseController
    {
        private const int PageSize = 10;

        // GET: /Admin/RentalRequest?status=Chờ duyệt&page=1 - Danh sách yêu cầu, lọc theo trạng thái
        public ActionResult Index(string status, int page = 1)
        {
            IQueryable<RentalRequest> requests = db.RentalRequests.Include(r => r.House);

            if (!string.IsNullOrEmpty(status) && RequestStatus.All.Contains(status))
            {
                requests = requests.Where(r => r.Status == status);
            }
            else
            {
                status = null;
            }

            var counts = db.RentalRequests
                .GroupBy(r => r.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToList();

            var model = new AdminRequestListViewModel
            {
                Status = status,
                Requests = new PagedList<RentalRequest>(
                    requests.OrderByDescending(r => r.CreatedDate).ThenByDescending(r => r.RequestId), page, PageSize),
                TotalCount = counts.Sum(c => c.Count),
                StatusCounts = counts.ToDictionary(c => c.Status, c => c.Count)
            };
            return View(model);
        }

        // GET: /Admin/RentalRequest/Details/5
        public ActionResult Details(int id)
        {
            RentalRequest request = db.RentalRequests
                .Include(r => r.House.Category)
                .FirstOrDefault(r => r.RequestId == id);
            if (request == null)
            {
                return HttpNotFound();
            }
            return View(request);
        }

        // POST: /Admin/RentalRequest/Approve/5 - Duyệt yêu cầu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Approve(int id, string returnUrl)
        {
            return ChangeStatus(id, RequestStatus.Approved, returnUrl);
        }

        // POST: /Admin/RentalRequest/Cancel/5 - Hủy yêu cầu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Cancel(int id, string returnUrl)
        {
            return ChangeStatus(id, RequestStatus.Cancelled, returnUrl);
        }

        // POST: /Admin/RentalRequest/Delete/5 - Xóa yêu cầu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, string returnUrl)
        {
            RentalRequest request = db.RentalRequests.Find(id);
            if (request == null)
            {
                return HttpNotFound();
            }

            db.RentalRequests.Remove(request);
            db.SaveChanges();
            SetSuccess("Đã xóa yêu cầu #" + id + " của khách hàng " + request.CustomerName + ".");

            // Sau khi xóa không quay lại trang chi tiết của chính yêu cầu đó
            if (!string.IsNullOrEmpty(returnUrl) && returnUrl.Contains("/Details/"))
            {
                returnUrl = null;
            }
            return RedirectToLocal(returnUrl, "Index");
        }

        private ActionResult ChangeStatus(int id, string newStatus, string returnUrl)
        {
            RentalRequest request = db.RentalRequests.Find(id);
            if (request == null)
            {
                return HttpNotFound();
            }

            if (request.Status == newStatus)
            {
                SetError("Yêu cầu #" + id + " đã ở trạng thái \"" + newStatus + "\".");
            }
            else
            {
                request.Status = newStatus;
                db.SaveChanges();
                SetSuccess("Yêu cầu #" + id + " của " + request.CustomerName + " đã chuyển sang \"" + newStatus + "\".");
            }
            return RedirectToLocal(returnUrl, "Index");
        }
    }
}
