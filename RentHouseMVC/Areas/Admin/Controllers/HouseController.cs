using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using RentHouseMVC.Helpers;
using RentHouseMVC.Models;
using RentHouseMVC.Models.ViewModels;

namespace RentHouseMVC.Areas.Admin.Controllers
{
    public class HouseController : AdminBaseController
    {
        private const int PageSize = 10;

        // GET: /Admin/House?keyword=...&catId=...&status=...&page=...
        public ActionResult Index(string keyword, int? catId, string status, int page = 1)
        {
            IQueryable<House> houses = db.Houses.Include(h => h.Category);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                string kw = keyword.Trim();
                houses = houses.Where(h => h.Title.Contains(kw) || h.Address.Contains(kw) || h.District.Contains(kw));
            }
            if (catId.HasValue)
            {
                int cat = catId.Value;
                houses = houses.Where(h => h.CatId == cat);
            }
            if (!string.IsNullOrEmpty(status) && HouseStatus.All.Contains(status))
            {
                houses = houses.Where(h => h.Status == status);
            }
            else
            {
                status = null;
            }

            var ordered = houses.OrderByDescending(h => h.PostedDate).ThenByDescending(h => h.HouseId);

            var model = new AdminHouseListViewModel
            {
                Keyword = keyword,
                CatId = catId,
                Status = status,
                Houses = new PagedList<House>(ordered, page, PageSize),
                Categories = new SelectList(db.Categories.OrderBy(c => c.Id).ToList(), "Id", "CatName", catId)
            };
            return View(model);
        }

        // GET: /Admin/House/Create
        public ActionResult Create()
        {
            PopulateDropdowns();
            return View(new HouseFormViewModel { Status = HouseStatus.Available });
        }

        // POST: /Admin/House/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(HouseFormViewModel model)
        {
            model.HouseId = 0;
            ValidateHouseForm(model);
            if (!ModelState.IsValid)
            {
                PopulateDropdowns();
                return View(model);
            }

            var house = new House { PostedDate = DateTime.Now };
            model.ApplyTo(house);

            if (model.ImageFile != null && model.ImageFile.ContentLength > 0)
            {
                house.Image = ImageHelper.Save(model.ImageFile);
            }

            db.Houses.Add(house);
            db.SaveChanges();

            SetSuccess("Đã thêm nhà \"" + house.Title + "\".");
            return RedirectToAction("Index");
        }

        // GET: /Admin/House/Edit/5
        public ActionResult Edit(int id)
        {
            House house = db.Houses.Find(id);
            if (house == null)
            {
                return HttpNotFound();
            }

            PopulateDropdowns();
            return View(HouseFormViewModel.FromEntity(house));
        }

        // POST: /Admin/House/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(HouseFormViewModel model)
        {
            House house = db.Houses.Find(model.HouseId);
            if (house == null)
            {
                return HttpNotFound();
            }

            ValidateHouseForm(model);
            if (!ModelState.IsValid)
            {
                model.CurrentImage = house.Image;
                PopulateDropdowns();
                return View(model);
            }

            model.ApplyTo(house);

            if (model.ImageFile != null && model.ImageFile.ContentLength > 0)
            {
                string oldImage = house.Image;
                house.Image = ImageHelper.Save(model.ImageFile);
                ImageHelper.Delete(oldImage);
            }

            db.SaveChanges();

            SetSuccess("Đã cập nhật nhà \"" + house.Title + "\".");
            return RedirectToAction("Index");
        }

        // GET: /Admin/House/Delete/5 - Trang xác nhận xóa
        public ActionResult Delete(int id)
        {
            House house = db.Houses.Include(h => h.Category).FirstOrDefault(h => h.HouseId == id);
            if (house == null)
            {
                return HttpNotFound();
            }

            ViewBag.RequestCount = db.RentalRequests.Count(r => r.HouseId == id);
            return View(house);
        }

        // POST: /Admin/House/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            House house = db.Houses.Find(id);
            if (house == null)
            {
                return HttpNotFound();
            }

            string title = house.Title;
            string image = house.Image;

            // FK_RentalRequest_House có ON DELETE CASCADE: các yêu cầu thuê của nhà sẽ bị xóa theo
            db.Houses.Remove(house);
            db.SaveChanges();
            ImageHelper.Delete(image);

            SetSuccess("Đã xóa nhà \"" + title + "\" và các yêu cầu thuê liên quan.");
            return RedirectToAction("Index");
        }

        // POST: /Admin/House/ToggleStatus/5 - Đổi trạng thái Còn trống <-> Đã thuê
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ToggleStatus(int id, string returnUrl)
        {
            House house = db.Houses.Find(id);
            if (house == null)
            {
                return HttpNotFound();
            }

            house.Status = house.Status == HouseStatus.Available ? HouseStatus.Rented : HouseStatus.Available;
            db.SaveChanges();

            SetSuccess("Đã chuyển nhà \"" + house.Title + "\" sang trạng thái \"" + house.Status + "\".");
            return RedirectToLocal(returnUrl, "Index");
        }

        // Kiểm tra thêm ở server ngoài DataAnnotations
        private void ValidateHouseForm(HouseFormViewModel model)
        {
            if (model.CatId.HasValue)
            {
                int catId = model.CatId.Value;
                if (!db.Categories.Any(c => c.Id == catId))
                {
                    ModelState.AddModelError("CatId", "Loại nhà không tồn tại.");
                }
            }

            if (!string.IsNullOrEmpty(model.District) && !SearchOptions.Districts.Contains(model.District))
            {
                ModelState.AddModelError("District", "Quận/huyện không hợp lệ.");
            }

            if (!string.IsNullOrEmpty(model.Status) && !HouseStatus.All.Contains(model.Status))
            {
                ModelState.AddModelError("Status", "Trạng thái không hợp lệ.");
            }

            string imageError = ImageHelper.Validate(model.ImageFile);
            if (imageError != null)
            {
                ModelState.AddModelError("ImageFile", imageError);
            }
        }

        private void PopulateDropdowns()
        {
            ViewBag.Categories = new SelectList(db.Categories.OrderBy(c => c.Id).ToList(), "Id", "CatName");
            ViewBag.Districts = new SelectList(SearchOptions.Districts);
            ViewBag.Statuses = new SelectList(HouseStatus.All);
        }
    }
}
