using System;

namespace RentHouseMVC.Models.ViewModels
{
    /// <summary>
    /// Dữ liệu cho partial view _Pager (thanh phân trang dùng chung).
    /// </summary>
    public class PagerModel
    {
        public PagerModel(int currentPage, int totalPages, Func<int, string> pageUrl)
        {
            CurrentPage = currentPage;
            TotalPages = totalPages;
            PageUrl = pageUrl;
        }

        public int CurrentPage { get; private set; }
        public int TotalPages { get; private set; }

        // Hàm tạo URL cho trang số n (giữ nguyên các tham số lọc)
        public Func<int, string> PageUrl { get; private set; }
    }
}
