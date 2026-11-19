using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using RentHouseMVC.Models;
using RentHouseMVC.Models.ViewModels;

namespace RentHouseMVC.Areas.Admin.Controllers
{
    public class DashboardController : AdminBaseController
    {
        // GET: /Admin hoặc /Admin/Dashboard
        public ActionResult Index()
        {
            var model = new DashboardViewModel
            {
                TotalHouses = db.Houses.Count(),
                AvailableHouses = db.Houses.Count(h => h.Status == HouseStatus.Available),
                RentedHouses = db.Houses.Count(h => h.Status == HouseStatus.Rented),
                TotalCategories = db.Categories.Count(),
                PendingRequests = db.RentalRequests.Count(r => r.Status == RequestStatus.Pending),
                TotalRequests = db.RentalRequests.Count(),
                // 5 yêu cầu mới nhất
                LatestRequests = db.RentalRequests
                    .Include(r => r.House)
                    .OrderByDescending(r => r.CreatedDate)
                    .ThenByDescending(r => r.RequestId)
                    .Take(5)
                    .ToList()
            };
            return View(model);
        }

        // Child action: số yêu cầu chờ duyệt, hiển thị trên menu bên trái (_AdminLayout)
        [ChildActionOnly]
        public ActionResult PendingCount()
        {
            int count = db.RentalRequests.Count(r => r.Status == RequestStatus.Pending);
            return Content(count.ToString());
        }
    }
}
