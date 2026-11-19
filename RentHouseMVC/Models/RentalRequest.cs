using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentHouseMVC.Models
{
    /// <summary>
    /// Yêu cầu thuê / đặt lịch xem nhà - ánh xạ tới bảng dbo.RentalRequest
    /// </summary>
    [Table("RentalRequest")]
    public partial class RentalRequest
    {
        [Key]
        [Display(Name = "Mã yêu cầu")]
        public int RequestId { get; set; }

        [Display(Name = "Nhà")]
        public int HouseId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Họ tên khách hàng")]
        public string CustomerName { get; set; }

        [Required]
        [StringLength(15)]
        [Display(Name = "Số điện thoại")]
        public string CustomerPhone { get; set; }

        [StringLength(100)]
        [Display(Name = "Email")]
        public string CustomerEmail { get; set; }

        [Display(Name = "Ngày giờ xem nhà")]
        public DateTime ViewingDate { get; set; }

        [StringLength(500)]
        [Display(Name = "Ghi chú")]
        public string Note { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Trạng thái")]
        public string Status { get; set; }

        [Display(Name = "Ngày gửi")]
        public DateTime CreatedDate { get; set; }

        // Yêu cầu thuộc về một nhà (khóa ngoại HouseId)
        [ForeignKey("HouseId")]
        public virtual House House { get; set; }
    }
}
