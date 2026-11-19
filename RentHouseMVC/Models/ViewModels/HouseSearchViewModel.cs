using System.Collections.Generic;
using System.Web.Routing;
using RentHouseMVC.Helpers;

namespace RentHouseMVC.Models.ViewModels
{
    /// <summary>
    /// Điều kiện tìm kiếm / lọc nhà (nhận từ query string) và kết quả (House/Search).
    /// </summary>
    public class HouseSearchViewModel
    {
        // ----- Điều kiện lọc -----
        public string Keyword { get; set; }
        public int? CatId { get; set; }
        public string District { get; set; }
        public string Price { get; set; }          // khóa khoảng giá: duoi-3, 3-5, 5-10, 10-20, tren-20
        public double? MinAcreage { get; set; }    // diện tích tối thiểu (m²)
        public int? Bedrooms { get; set; }         // số phòng ngủ tối thiểu
        public bool OnlyAvailable { get; set; }    // chỉ hiện nhà còn trống
        public string Sort { get; set; }           // moi-nhat, cu-nhat, gia-tang, gia-giam
        public int Page { get; set; }

        // ----- Kết quả -----
        public PagedList<House> Results { get; set; }
        public List<Category> Categories { get; set; }

        public bool HasFilter
        {
            get
            {
                return !string.IsNullOrWhiteSpace(Keyword) || CatId.HasValue || !string.IsNullOrEmpty(District)
                    || !string.IsNullOrEmpty(Price) || MinAcreage.HasValue || Bedrooms.HasValue || OnlyAvailable;
            }
        }

        /// <summary>
        /// Giữ nguyên các điều kiện lọc khi chuyển trang.
        /// </summary>
        public RouteValueDictionary ToRouteValues(int page)
        {
            var values = new RouteValueDictionary();
            if (!string.IsNullOrWhiteSpace(Keyword)) values["keyword"] = Keyword.Trim();
            if (CatId.HasValue) values["catId"] = CatId.Value;
            if (!string.IsNullOrEmpty(District)) values["district"] = District;
            if (!string.IsNullOrEmpty(Price)) values["price"] = Price;
            if (MinAcreage.HasValue) values["minAcreage"] = MinAcreage.Value;
            if (Bedrooms.HasValue) values["bedrooms"] = Bedrooms.Value;
            if (OnlyAvailable) values["onlyAvailable"] = "true";
            if (!string.IsNullOrEmpty(Sort) && Sort != "moi-nhat") values["sort"] = Sort;
            if (page > 1) values["page"] = page;
            return values;
        }
    }
}
