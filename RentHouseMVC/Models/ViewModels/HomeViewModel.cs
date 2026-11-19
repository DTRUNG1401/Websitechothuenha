using System.Collections.Generic;

namespace RentHouseMVC.Models.ViewModels
{
    /// <summary>
    /// Dữ liệu cho trang chủ.
    /// </summary>
    public class HomeViewModel
    {
        public List<House> LatestHouses { get; set; }
        public List<CategorySummary> Categories { get; set; }
        public int TotalHouses { get; set; }
        public int AvailableHouses { get; set; }
        public int TotalDistricts { get; set; }
        public int TotalRequests { get; set; }
    }
}
