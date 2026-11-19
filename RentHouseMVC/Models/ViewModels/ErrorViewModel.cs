namespace RentHouseMVC.Models.ViewModels
{
    /// <summary>
    /// Dữ liệu cho trang lỗi thân thiện.
    /// </summary>
    public class ErrorViewModel
    {
        public int StatusCode { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }

        // Chi tiết lỗi kỹ thuật - chỉ hiển thị khi truy cập từ chính máy chạy web (localhost)
        public string Detail { get; set; }
    }
}
