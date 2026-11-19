using System.ComponentModel.DataAnnotations;
using System.Web;

namespace RentHouseMVC.Models.ViewModels
{
    /// <summary>
    /// Form thêm / sửa nhà trong trang quản trị.
    /// Dùng ViewModel riêng để: kiểm tra dữ liệu, nhận file ảnh upload và tránh overposting.
    /// </summary>
    public class HouseFormViewModel
    {
        public int HouseId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tiêu đề")]
        [StringLength(200, MinimumLength = 10, ErrorMessage = "Tiêu đề phải từ 10 đến 200 ký tự")]
        [Display(Name = "Tiêu đề")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn loại nhà")]
        [Display(Name = "Loại nhà")]
        public int? CatId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
        [StringLength(255, ErrorMessage = "Địa chỉ tối đa 255 ký tự")]
        [Display(Name = "Địa chỉ (số nhà, đường, phường)")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn quận/huyện")]
        [StringLength(50)]
        [Display(Name = "Quận/Huyện")]
        public string District { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá thuê")]
        [Range(typeof(decimal), "100000", "1000000000", ErrorMessage = "Giá thuê từ 100.000 đ đến 1.000.000.000 đ")]
        [Display(Name = "Giá thuê (đ/tháng)")]
        public decimal? Price { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập diện tích")]
        [Range(1, 10000, ErrorMessage = "Diện tích từ 1 đến 10.000 m²")]
        [Display(Name = "Diện tích (m²)")]
        public double? Acreage { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số phòng ngủ")]
        [Range(0, 50, ErrorMessage = "Số phòng ngủ từ 0 đến 50")]
        [Display(Name = "Số phòng ngủ")]
        public int? Bedrooms { get; set; }

        [Display(Name = "Mô tả chi tiết")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn trạng thái")]
        [Display(Name = "Trạng thái")]
        public string Status { get; set; }

        // Tên file ảnh hiện tại (khi sửa)
        public string CurrentImage { get; set; }

        [Display(Name = "Ảnh đại diện")]
        public HttpPostedFileBase ImageFile { get; set; }

        public bool IsEdit
        {
            get { return HouseId > 0; }
        }

        /// <summary>Tạo form từ entity (khi mở trang sửa)</summary>
        public static HouseFormViewModel FromEntity(House house)
        {
            return new HouseFormViewModel
            {
                HouseId = house.HouseId,
                Title = house.Title,
                CatId = house.CatId,
                Address = house.Address,
                District = house.District,
                Price = house.Price,
                Acreage = house.Acreage,
                Bedrooms = house.Bedrooms,
                Description = house.Description,
                Status = house.Status,
                CurrentImage = house.Image
            };
        }

        /// <summary>Chép dữ liệu đã kiểm tra từ form vào entity</summary>
        public void ApplyTo(House house)
        {
            house.Title = Title.Trim();
            house.CatId = CatId.GetValueOrDefault();
            house.Address = Address.Trim();
            house.District = District;
            house.Price = Price.GetValueOrDefault();
            house.Acreage = Acreage.GetValueOrDefault();
            house.Bedrooms = Bedrooms.GetValueOrDefault();
            house.Description = string.IsNullOrWhiteSpace(Description) ? null : Description.Trim();
            house.Status = Status;
        }
    }
}
