using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentHouseMVC.Models
{
    /// <summary>
    /// Tài khoản quản trị - ánh xạ tới bảng dbo.Account
    /// </summary>
    [Table("Account")]
    public partial class Account
    {
        [Key]
        [Display(Name = "Mã tài khoản")]
        public int AccountId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Tên đăng nhập")]
        public string Username { get; set; }

        // Lưu chuỗi băm SHA256 (64 ký tự hex), không lưu mật khẩu gốc
        [Required]
        [StringLength(64)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Họ tên")]
        public string FullName { get; set; }
    }
}
