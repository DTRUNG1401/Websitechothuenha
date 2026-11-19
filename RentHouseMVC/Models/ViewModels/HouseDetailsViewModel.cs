using System.Collections.Generic;

namespace RentHouseMVC.Models.ViewModels
{
    /// <summary>
    /// Dữ liệu cho trang chi tiết nhà (House/Details).
    /// </summary>
    public class HouseDetailsViewModel
    {
        public House House { get; set; }

        // Các nhà cùng loại
        public List<House> RelatedHouses { get; set; }

        // Form gửi yêu cầu thuê
        public RentalRequestFormModel RequestForm { get; set; }

        // Chỉ nhà "Còn trống" mới hiện form
        public bool CanRequest
        {
            get { return House != null && House.Status == HouseStatus.Available; }
        }
    }
}
