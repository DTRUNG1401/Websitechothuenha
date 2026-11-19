using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace RentHouseMVC.Helpers
{
    /// <summary>
    /// Một khoảng giá trong bộ lọc tìm kiếm.
    /// </summary>
    public class PriceRange
    {
        public PriceRange(string key, string text, decimal? min, decimal? max)
        {
            Key = key;
            Text = text;
            Min = min;
            Max = max;
        }

        public string Key { get; private set; }
        public string Text { get; private set; }
        public decimal? Min { get; private set; }
        public decimal? Max { get; private set; }
    }

    /// <summary>
    /// Dữ liệu cho các ô chọn trong form tìm kiếm / form nhập nhà.
    /// </summary>
    public static class SearchOptions
    {
        public static readonly IList<PriceRange> PriceRanges = new List<PriceRange>
        {
            new PriceRange("duoi-3", "Dưới 3 triệu", null, 3000000m),
            new PriceRange("3-5", "Từ 3 - 5 triệu", 3000000m, 5000000m),
            new PriceRange("5-10", "Từ 5 - 10 triệu", 5000000m, 10000000m),
            new PriceRange("10-20", "Từ 10 - 20 triệu", 10000000m, 20000000m),
            new PriceRange("tren-20", "Trên 20 triệu", 20000000m, null)
        };

        // Quận / huyện TP.HCM
        public static readonly string[] Districts =
        {
            "Quận 1", "Quận 3", "Quận 4", "Quận 5", "Quận 6", "Quận 7", "Quận 8",
            "Quận 10", "Quận 11", "Quận 12", "Bình Thạnh", "Phú Nhuận", "Gò Vấp",
            "Tân Bình", "Tân Phú", "Bình Tân", "TP. Thủ Đức", "Nhà Bè",
            "Bình Chánh", "Hóc Môn", "Củ Chi", "Cần Giờ"
        };

        public static readonly int[] MinAcreages = { 15, 20, 30, 50, 80, 100 };

        public static readonly int[] BedroomOptions = { 1, 2, 3, 4 };

        public static PriceRange FindPriceRange(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }
            return PriceRanges.FirstOrDefault(p => p.Key == key);
        }

        public static IEnumerable<SelectListItem> PriceRangeItems(string selected)
        {
            return PriceRanges.Select(p => new SelectListItem { Value = p.Key, Text = p.Text, Selected = p.Key == selected });
        }

        public static IEnumerable<SelectListItem> DistrictItems(string selected)
        {
            return Districts.Select(d => new SelectListItem { Value = d, Text = d, Selected = d == selected });
        }

        public static IEnumerable<SelectListItem> AcreageItems(double? selected)
        {
            return MinAcreages.Select(a => new SelectListItem
            {
                Value = a.ToString(),
                Text = "Từ " + a + " m²",
                Selected = selected.HasValue && (int)selected.Value == a
            });
        }

        public static IEnumerable<SelectListItem> BedroomItems(int? selected)
        {
            return BedroomOptions.Select(b => new SelectListItem
            {
                Value = b.ToString(),
                Text = "Từ " + b + " phòng ngủ",
                Selected = selected.HasValue && selected.Value == b
            });
        }

        public static IEnumerable<SelectListItem> SortItems(string selected)
        {
            var items = new List<SelectListItem>
            {
                new SelectListItem { Value = "moi-nhat", Text = "Mới đăng nhất" },
                new SelectListItem { Value = "cu-nhat", Text = "Đăng lâu nhất" },
                new SelectListItem { Value = "gia-tang", Text = "Giá thấp đến cao" },
                new SelectListItem { Value = "gia-giam", Text = "Giá cao đến thấp" }
            };
            foreach (var item in items)
            {
                item.Selected = item.Value == (selected ?? "moi-nhat");
            }
            return items;
        }
    }
}
