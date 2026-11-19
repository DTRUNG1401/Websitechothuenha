using System;
using System.ComponentModel.DataAnnotations;

namespace RentHouseMVC.Models.ViewModels
{
    /// <summary>
    /// Form gửi yêu cầu thuê / đặt lịch xem nhà ở trang chi tiết.
    /// Chỉ chứa các trường khách được phép nhập (tránh overposting Status, CreatedDate...).
    /// </summary>
    public class RentalRequestFormModel
    {
        public int HouseId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Họ tên từ 2 đến 100 ký tự")]
        [Display(Name = "Họ và tên")]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [RegularExpression(@"^(0|\+84)[0-9]{9,10}$", ErrorMessage = "Số điện thoại không hợp lệ (VD: 0901234567)")]
        [Display(Name = "Số điện thoại")]
        public string CustomerPhone { get; set; }

        [StringLength(100, ErrorMessage = "Email tối đa 100 ký tự")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string CustomerEmail { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày giờ muốn xem nhà")]
        [Display(Name = "Ngày giờ xem nhà")]
        public DateTime? ViewingDate { get; set; }

        [StringLength(500, ErrorMessage = "Lời nhắn tối đa 500 ký tự")]
        [Display(Name = "Lời nhắn")]
        public string Note { get; set; }
    }
}
