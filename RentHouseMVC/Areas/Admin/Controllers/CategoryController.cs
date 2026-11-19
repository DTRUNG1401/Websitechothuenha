using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Mvc;
using RentHouseMVC.Helpers;
using RentHouseMVC.Models;
using RentHouseMVC.Models.ViewModels;

namespace RentHouseMVC.Areas.Admin.Controllers
{
    public class CategoryController : AdminBaseController
    {
        // GET: /Admin/Category - Danh sách loại nhà
        public ActionResult Index()
        {
            return View(db.GetCategorySummaries());
        }

        // GET: /Admin/Category/Create
        public ActionResult Create()
        {
            return View(new Category());
        }

        // POST: /Admin/Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CatName")] Category category)
        {
            if (ModelState.IsValid)
            {
                category.CatName = category.CatName.Trim();
                if (IsDuplicateName(category.CatName, 0))
                {
                    ModelState.AddModelError("CatName", "Tên loại nhà đã tồn tại.");
                }
            }

            if (!ModelState.IsValid)
            {
                return View(category);
            }

            db.Categories.Add(category);
            db.SaveChanges();
            SetSuccess("Đã thêm loại nhà \"" + category.CatName + "\".");
            return RedirectToAction("Index");
        }

        // GET: /Admin/Category/Edit/5
        public ActionResult Edit(int id)
        {
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: /Admin/Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CatName")] Category category)
        {
            Category existing = db.Categories.Find(category.Id);
            if (existing == null)
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {
                category.CatName = category.CatName.Trim();
                if (IsDuplicateName(category.CatName, category.Id))
                {
                    ModelState.AddModelError("CatName", "Tên loại nhà đã tồn tại.");
                }
            }

            if (!ModelState.IsValid)
            {
                return View(category);
            }

            existing.CatName = category.CatName;
            db.SaveChanges();
            SetSuccess("Đã cập nhật loại nhà \"" + existing.CatName + "\".");
            return RedirectToAction("Index");
        }

        // GET: /Admin/Category/Delete/5 - Trang xác nhận xóa
        public ActionResult Delete(int id)
        {
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }

            var model = new CategoryDeleteViewModel
            {
                Category = category,
                HouseCount = db.Houses.Count(h => h.CatId == id)
            };
            return View(model);
        }

        // POST: /Admin/Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }

            // Không cho xóa loại nhà đang có nhà
            int houseCount = db.Houses.Count(h => h.CatId == id);
            if (houseCount > 0)
            {
                SetError("Không thể xóa loại nhà \"" + category.CatName + "\" vì đang có " + houseCount
                    + " nhà thuộc loại này. Hãy chuyển các nhà đó sang loại khác hoặc xóa chúng trước.");
                return RedirectToAction("Index");
            }

            try
            {
                db.Categories.Remove(category);
                db.SaveChanges();
                SetSuccess("Đã xóa loại nhà \"" + category.CatName + "\".");
            }
            catch (DbUpdateException)
            {
                // Phòng trường hợp có nhà vừa được thêm vào loại này (vi phạm FK_House_Category)
                SetError("Không thể xóa loại nhà \"" + category.CatName + "\" vì vẫn còn nhà thuộc loại này.");
            }
            return RedirectToAction("Index");
        }

        private bool IsDuplicateName(string name, int excludeId)
        {
            return db.Categories.Any(c => c.CatName == name && c.Id != excludeId);
        }
    }
}
