using System.Collections.Generic;
using System.Linq;
using RentHouseMVC.Models;
using RentHouseMVC.Models.ViewModels;

namespace RentHouseMVC.Helpers
{
    /// <summary>
    /// Các truy vấn EF dùng chung giữa nhiều controller.
    /// </summary>
    public static class QueryExtensions
    {
        /// <summary>
        /// Danh sách loại nhà kèm số nhà và số nhà còn trống (1 câu SQL duy nhất).
        /// </summary>
        public static List<CategorySummary> GetCategorySummaries(this RentHouseDBContext db)
        {
            return db.Categories
                .OrderBy(c => c.Id)
                .Select(c => new CategorySummary
                {
                    Id = c.Id,
                    CatName = c.CatName,
                    HouseCount = c.Houses.Count(),
                    AvailableCount = c.Houses.Count(h => h.Status == HouseStatus.Available)
                })
                .ToList();
        }
    }
}
