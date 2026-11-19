using System.Collections.Generic;
using RentHouseMVC.Helpers;

namespace RentHouseMVC.Models.ViewModels
{
    /// <summary>
    /// Dữ liệu cho trang danh sách nhà theo loại (House/Index).
    /// </summary>
    public class HouseListViewModel
    {
        // Loại nhà đang xem; null = tất cả
        public Category CurrentCategory { get; set; }

        public List<CategorySummary> Categories { get; set; }

        public PagedList<House> Houses { get; set; }

        public int? CurrentCategoryId
        {
            get { return CurrentCategory != null ? (int?)CurrentCategory.Id : null; }
        }

        public string PageTitle
        {
            get { return CurrentCategory != null ? CurrentCategory.CatName : "Tất cả nhà cho thuê"; }
        }
    }
}
