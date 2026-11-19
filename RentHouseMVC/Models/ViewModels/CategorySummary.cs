namespace RentHouseMVC.Models.ViewModels
{
    /// <summary>
    /// Loại nhà kèm thống kê số lượng nhà (dùng cho menu, trang chủ, trang quản trị).
    /// </summary>
    public class CategorySummary
    {
        public int Id { get; set; }
        public string CatName { get; set; }
        public int HouseCount { get; set; }
        public int AvailableCount { get; set; }
    }
}
