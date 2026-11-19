using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using RentHouseMVC.Helpers;
using RentHouseMVC.Models;
using RentHouseMVC.Models.ViewModels;

namespace RentHouseMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly RentHouseDBContext db = new RentHouseDBContext();

        // GET: / hoặc /Home/Index - Trang chủ
        public ActionResult Index()
        {
            var model = new HomeViewModel
            {
                // 6 nhà còn trống mới đăng gần nhất
                LatestHouses = db.Houses
                    .Include(h => h.Category)
                    .Where(h => h.Status == HouseStatus.Available)
                    .OrderByDescending(h => h.PostedDate)
                    .ThenByDescending(h => h.HouseId)
                    .Take(6)
                    .ToList(),
                Categories = db.GetCategorySummaries(),
                TotalHouses = db.Houses.Count(),
                AvailableHouses = db.Houses.Count(h => h.Status == HouseStatus.Available),
                TotalDistricts = db.Houses.Select(h => h.District).Distinct().Count(),
                TotalRequests = db.RentalRequests.Count()
            };
            return View(model);
        }

        // GET: /Home/About - Giới thiệu
        public ActionResult About()
        {
            ViewBag.TotalHouses = db.Houses.Count();
            ViewBag.TotalCategories = db.Categories.Count();
            ViewBag.TotalRequests = db.RentalRequests.Count();
            return View();
        }

        // GET: /Home/Contact - Liên hệ
        public ActionResult Contact()
        {
            return View();
        }

        // Child action: menu "Loại nhà" trên thanh điều hướng (_Layout)
        [ChildActionOnly]
        public ActionResult CategoryMenu()
        {
            List<CategorySummary> categories;
            try
            {
                categories = db.GetCategorySummaries();
            }
            catch (Exception ex)
            {
                // Không để lỗi kết nối database làm hỏng cả layout
                System.Diagnostics.Trace.TraceError("[RentHouseMVC] CategoryMenu: " + ex.Message);
                categories = new List<CategorySummary>();
            }
            return PartialView("_CategoryMenu", categories);
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
