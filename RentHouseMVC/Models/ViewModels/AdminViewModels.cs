using System.Collections.Generic;
using System.Web.Mvc;
using RentHouseMVC.Helpers;

namespace RentHouseMVC.Models.ViewModels
{
    /// <summary>
    /// Dữ liệu cho trang Dashboard quản trị.
    /// </summary>
    public class DashboardViewModel
    {
        public int TotalHouses { get; set; }
        public int AvailableHouses { get; set; }
        public int RentedHouses { get; set; }
        public int TotalCategories { get; set; }
        public int PendingRequests { get; set; }
        public int TotalRequests { get; set; }
        public List<RentalRequest> LatestRequests { get; set; }
    }

    /// <summary>
    /// Danh sách nhà trong trang quản trị (có lọc + phân trang).
    /// </summary>
    public class AdminHouseListViewModel
    {
        public PagedList<House> Houses { get; set; }
        public string Keyword { get; set; }
        public int? CatId { get; set; }
        public string Status { get; set; }
        public IEnumerable<SelectListItem> Categories { get; set; }
    }

    /// <summary>
    /// Danh sách yêu cầu thuê trong trang quản trị (lọc theo trạng thái + phân trang).
    /// </summary>
    public class AdminRequestListViewModel
    {
        public PagedList<RentalRequest> Requests { get; set; }
        public string Status { get; set; }
        public int TotalCount { get; set; }
        public Dictionary<string, int> StatusCounts { get; set; }

        public int CountOf(string status)
        {
            int count;
            return StatusCounts != null && StatusCounts.TryGetValue(status, out count) ? count : 0;
        }
    }

    /// <summary>
    /// Thông tin hiển thị trên trang xác nhận xóa loại nhà.
    /// </summary>
    public class CategoryDeleteViewModel
    {
        public Category Category { get; set; }
        public int HouseCount { get; set; }

        public bool CanDelete
        {
            get { return HouseCount == 0; }
        }
    }
}
