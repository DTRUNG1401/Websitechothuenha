using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentHouseMVC.Models
{
    /// <summary>
    /// Nhà cho thuê - ánh xạ tới bảng dbo.House
    /// </summary>
    [Table("House")]
    public partial class House
    {
        public House()
        {
            RentalRequests = new HashSet<RentalRequest>();
        }

        [Key]
        [Display(Name = "Mã nhà")]
        public int HouseId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tiêu đề")]
        [StringLength(200, ErrorMessage = "Tiêu đề tối đa 200 ký tự")]
        [Display(Name = "Tiêu đề")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
        [StringLength(255, ErrorMessage = "Địa chỉ tối đa 255 ký tự")]
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn quận/huyện")]
        [StringLength(50, ErrorMessage = "Quận/huyện tối đa 50 ký tự")]
        [Display(Name = "Quận/Huyện")]
        public string District { get; set; }

        [Display(Name = "Giá thuê (đ/tháng)")]
        public decimal Price { get; set; }

        [Display(Name = "Diện tích (m²)")]
        public double Acreage { get; set; }

        [Display(Name = "Số phòng ngủ")]
        public int Bedrooms { get; set; }

        [Display(Name = "Mô tả")]
        public string Description { get; set; }

        [StringLength(255)]
        [Display(Name = "Hình ảnh")]
        public string Image { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Trạng thái")]
        public string Status { get; set; }

        [Display(Name = "Ngày đăng")]
        public DateTime PostedDate { get; set; }

        [Display(Name = "Loại nhà")]
        public int CatId { get; set; }

        // Nhà thuộc về một loại nhà (khóa ngoại CatId)
        [ForeignKey("CatId")]
        public virtual Category Category { get; set; }

        // Một nhà có nhiều yêu cầu thuê
        public virtual ICollection<RentalRequest> RentalRequests { get; set; }
    }
}
