using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentHouseMVC.Models
{
    /// <summary>
    /// Loại nhà - ánh xạ tới bảng dbo.Category
    /// </summary>
    [Table("Category")]
    public partial class Category
    {
        public Category()
        {
            Houses = new HashSet<House>();
        }

        [Key]
        [Display(Name = "Mã loại")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên loại nhà")]
        [StringLength(100, ErrorMessage = "Tên loại nhà tối đa 100 ký tự")]
        [Display(Name = "Tên loại nhà")]
        public string CatName { get; set; }

        // Một loại nhà có nhiều nhà
        public virtual ICollection<House> Houses { get; set; }
    }
}
