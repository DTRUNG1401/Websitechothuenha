using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using RentHouseMVC.Helpers;
using RentHouseMVC.Models;
using RentHouseMVC.Models.ViewModels;

namespace RentHouseMVC.Controllers
{
    public class HouseController : Controller
    {
        private const int PageSize = 9; // 9 nhà / trang
        private readonly RentHouseDBContext db = new RentHouseDBContext();

        // GET: /House/Index/{id}?page=2
        // id: mã loại nhà (Category.Id); id = null => lấy tất cả nhà
        public ActionResult Index(int? id, int page = 1)
        {
            Category category = null;
            IQueryable<House> houses = db.Houses.Include(h => h.Category);

            if (id.HasValue)
            {
                category = db.Categories.Find(id.Value);
                if (category == null)
                {
                    return HttpNotFound();
                }

                int catId = id.Value;
                houses = houses.Where(h => h.CatId == catId);
            }

            var ordered = houses
                .OrderByDescending(h => h.PostedDate)
                .ThenByDescending(h => h.HouseId);

            var model = new HouseListViewModel
            {
                CurrentCategory = category,
                Categories = db.GetCategorySummaries(),
                Houses = new PagedList<House>(ordered, page, PageSize)
            };
            return View(model);
        }

        // GET: /House/Search?keyword=...&district=...&price=...&minAcreage=...&bedrooms=...&sort=...
        // Tìm kiếm và lọc nhà theo nhiều điều kiện (mọi điều kiện đều đi qua LINQ to Entities, không ghép chuỗi SQL)
        public ActionResult Search(HouseSearchViewModel filter)
        {
            if (filter == null)
            {
                filter = new HouseSearchViewModel();
            }

            IQueryable<House> houses = db.Houses.Include(h => h.Category);

            if (!string.IsNullOrWhiteSpace(filter.Keyword))
            {
                string keyword = filter.Keyword.Trim();
                houses = houses.Where(h => h.Title.Contains(keyword)
                                        || h.Address.Contains(keyword)
                                        || h.District.Contains(keyword)
                                        || h.Description.Contains(keyword));
            }

            if (filter.CatId.HasValue)
            {
                int catId = filter.CatId.Value;
                houses = houses.Where(h => h.CatId == catId);
            }

            if (!string.IsNullOrEmpty(filter.District))
            {
                string district = filter.District;
                houses = houses.Where(h => h.District == district);
            }

            PriceRange range = SearchOptions.FindPriceRange(filter.Price);
            if (range != null)
            {
                if (range.Min.HasValue)
                {
                    decimal minPrice = range.Min.Value;
                    houses = houses.Where(h => h.Price >= minPrice);
                }
                if (range.Max.HasValue)
                {
                    decimal maxPrice = range.Max.Value;
                    houses = houses.Where(h => h.Price <= maxPrice);
                }
            }
            else
            {
                filter.Price = null;
            }

            if (filter.MinAcreage.HasValue && filter.MinAcreage.Value > 0)
            {
                double minAcreage = filter.MinAcreage.Value;
                houses = houses.Where(h => h.Acreage >= minAcreage);
            }

            if (filter.Bedrooms.HasValue && filter.Bedrooms.Value > 0)
            {
                int bedrooms = filter.Bedrooms.Value;
                houses = houses.Where(h => h.Bedrooms >= bedrooms);
            }

            if (filter.OnlyAvailable)
            {
                houses = houses.Where(h => h.Status == HouseStatus.Available);
            }

            IOrderedQueryable<House> ordered;
            switch (filter.Sort)
            {
                case "gia-tang":
                    ordered = houses.OrderBy(h => h.Price).ThenByDescending(h => h.PostedDate);
                    break;
                case "gia-giam":
                    ordered = houses.OrderByDescending(h => h.Price).ThenByDescending(h => h.PostedDate);
                    break;
                case "cu-nhat":
                    ordered = houses.OrderBy(h => h.PostedDate).ThenBy(h => h.HouseId);
                    break;
                default:
                    filter.Sort = "moi-nhat";
                    ordered = houses.OrderByDescending(h => h.PostedDate).ThenByDescending(h => h.HouseId);
                    break;
            }

            filter.Results = new PagedList<House>(ordered, filter.Page, PageSize);
            filter.Categories = db.Categories.OrderBy(c => c.Id).ToList();
            return View(filter);
        }

        // GET: /House/Details/5 - Chi tiết nhà
        public ActionResult Details(int id)
        {
            House house = db.Houses
                .Include(h => h.Category)
                .FirstOrDefault(h => h.HouseId == id);
            if (house == null)
            {
                return HttpNotFound();
            }

            var form = new RentalRequestFormModel { HouseId = house.HouseId };
            return View(BuildDetailsModel(house, form));
        }

        // POST: /House/SendRequest - Gửi yêu cầu thuê / đặt lịch xem nhà
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendRequest([Bind(Prefix = "RequestForm")] RentalRequestFormModel form)
        {
            if (form == null)
            {
                return HttpNotFound();
            }

            House house = db.Houses
                .Include(h => h.Category)
                .FirstOrDefault(h => h.HouseId == form.HouseId);
            if (house == null)
            {
                return HttpNotFound();
            }

            // Nhà đã cho thuê thì không nhận yêu cầu (form cũng đã bị ẩn ở giao diện)
            if (house.Status != HouseStatus.Available)
            {
                TempData["Error"] = "Nhà này đã được cho thuê nên không thể gửi yêu cầu. Bạn hãy tham khảo các nhà tương tự bên dưới.";
                return RedirectToAction("Details", new { id = house.HouseId });
            }

            // Kiểm tra thêm ở server: ngày xem phải ở tương lai và trong vòng 3 tháng
            if (form.ViewingDate.HasValue)
            {
                if (form.ViewingDate.Value <= DateTime.Now)
                {
                    ModelState.AddModelError("RequestForm.ViewingDate", "Ngày giờ xem nhà phải sau thời điểm hiện tại");
                }
                else if (form.ViewingDate.Value > DateTime.Now.AddMonths(3))
                {
                    ModelState.AddModelError("RequestForm.ViewingDate", "Chỉ có thể đặt lịch trong vòng 3 tháng tới");
                }
            }

            if (!ModelState.IsValid)
            {
                return View("Details", BuildDetailsModel(house, form));
            }

            var request = new RentalRequest
            {
                HouseId = house.HouseId,
                CustomerName = form.CustomerName.Trim(),
                CustomerPhone = form.CustomerPhone.Trim(),
                CustomerEmail = string.IsNullOrWhiteSpace(form.CustomerEmail) ? null : form.CustomerEmail.Trim(),
                ViewingDate = form.ViewingDate.GetValueOrDefault(),
                Note = string.IsNullOrWhiteSpace(form.Note) ? null : form.Note.Trim(),
                Status = RequestStatus.Pending,
                CreatedDate = DateTime.Now
            };
            db.RentalRequests.Add(request);
            db.SaveChanges();

            TempData["RequestSuccess"] = "Gửi yêu cầu thành công! Chúng tôi sẽ liên hệ với bạn qua số "
                + request.CustomerPhone + " để xác nhận lịch xem nhà.";

            // Post/Redirect/Get: tránh gửi lại form khi người dùng bấm F5
            return Redirect(Url.Action("Details", new { id = house.HouseId }) + "#dat-lich");
        }

        private HouseDetailsViewModel BuildDetailsModel(House house, RentalRequestFormModel form)
        {
            var related = db.Houses
                .Include(h => h.Category)
                .Where(h => h.CatId == house.CatId && h.HouseId != house.HouseId)
                .OrderBy(h => h.Status == HouseStatus.Available ? 0 : 1)
                .ThenByDescending(h => h.PostedDate)
                .Take(4)
                .ToList();

            return new HouseDetailsViewModel
            {
                House = house,
                RelatedHouses = related,
                RequestForm = form
            };
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
