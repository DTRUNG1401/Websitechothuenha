using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace RentHouseMVC.Models
{
    /// <summary>
    /// DbContext của Entity Framework 6 (Database First).
    /// Database RentHouseDB được tạo sẵn bằng script Database/RentHouseDB.sql,
    /// các entity class được viết tay khớp đúng với tên bảng / tên cột trong script.
    /// </summary>
    public partial class RentHouseDBContext : DbContext
    {
        static RentHouseDBContext()
        {
            // Không để EF tự tạo / sửa database (không dùng Migrations)
            Database.SetInitializer<RentHouseDBContext>(null);
        }

        // "name=RentHouseDBContext": bắt buộc lấy chuỗi kết nối cùng tên trong Web.config
        public RentHouseDBContext()
            : base("name=RentHouseDBContext")
        {
        }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<House> Houses { get; set; }
        public virtual DbSet<RentalRequest> RentalRequests { get; set; }
        public virtual DbSet<Account> Accounts { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Tắt quy tắc đổi tên bảng sang số nhiều (Category -> Categories)
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            // Category (1) - (n) House, FK_House_Category: không xóa dây chuyền
            modelBuilder.Entity<Category>()
                .HasMany(c => c.Houses)
                .WithRequired(h => h.Category)
                .HasForeignKey(h => h.CatId)
                .WillCascadeOnDelete(false);

            // House (1) - (n) RentalRequest, FK_RentalRequest_House: xóa nhà thì xóa luôn yêu cầu
            modelBuilder.Entity<House>()
                .HasMany(h => h.RentalRequests)
                .WithRequired(r => r.House)
                .HasForeignKey(r => r.HouseId)
                .WillCascadeOnDelete(true);

            // Khớp kiểu dữ liệu với script SQL
            modelBuilder.Entity<House>()
                .Property(h => h.Price)
                .HasPrecision(18, 0);

            modelBuilder.Entity<RentalRequest>()
                .Property(r => r.CustomerPhone)
                .IsUnicode(false);   // varchar(15)

            modelBuilder.Entity<Account>()
                .Property(a => a.Password)
                .IsUnicode(false);   // varchar(64)
        }
    }
}
