using System;
using System.Globalization;
using RentHouseMVC.Models;

namespace RentHouseMVC.Helpers
{
    /// <summary>
    /// Các hàm định dạng hiển thị dùng chung trong View.
    /// </summary>
    public static class DisplayHelper
    {
        // Định dạng số kiểu Việt Nam: 12.000.000
        private static readonly NumberFormatInfo VietnameseNumber = new NumberFormatInfo
        {
            NumberGroupSeparator = ".",
            NumberDecimalSeparator = ","
        };

        /// <summary>12000000 => "12.000.000 đ/tháng"</summary>
        public static string FormatPrice(decimal price)
        {
            return FormatMoney(price) + "/tháng";
        }

        /// <summary>12000000 => "12.000.000 đ"</summary>
        public static string FormatMoney(decimal money)
        {
            return money.ToString("#,##0", VietnameseNumber) + " đ";
        }

        /// <summary>45.5 => "45,5 m²"</summary>
        public static string FormatAcreage(double acreage)
        {
            return acreage.ToString("#,##0.#", VietnameseNumber) + " m²";
        }

        public static string FormatDate(DateTime date)
        {
            return date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        }

        public static string FormatDateTime(DateTime date)
        {
            return date.ToString("HH:mm - dd/MM/yyyy", CultureInfo.InvariantCulture);
        }

        /// <summary>Thời gian tương đối: "5 phút trước", "2 ngày trước"...</summary>
        public static string TimeAgo(DateTime date)
        {
            TimeSpan span = DateTime.Now - date;
            if (span.TotalMinutes < 1)
            {
                return "Vừa xong";
            }
            if (span.TotalHours < 1)
            {
                return (int)span.TotalMinutes + " phút trước";
            }
            if (span.TotalDays < 1)
            {
                return (int)span.TotalHours + " giờ trước";
            }
            if (span.TotalDays < 30)
            {
                return (int)span.TotalDays + " ngày trước";
            }
            return FormatDate(date);
        }

        /// <summary>Class CSS của badge trạng thái nhà</summary>
        public static string HouseStatusCss(string status)
        {
            return status == HouseStatus.Available ? "text-bg-success" : "text-bg-secondary";
        }

        /// <summary>Class CSS của badge trạng thái yêu cầu thuê</summary>
        public static string RequestStatusCss(string status)
        {
            switch (status)
            {
                case RequestStatus.Pending:
                    return "text-bg-warning";
                case RequestStatus.Approved:
                    return "text-bg-success";
                case RequestStatus.Cancelled:
                    return "text-bg-danger";
                default:
                    return "text-bg-light";
            }
        }

        /// <summary>Icon Bootstrap Icons cho từng trạng thái yêu cầu</summary>
        public static string RequestStatusIcon(string status)
        {
            switch (status)
            {
                case RequestStatus.Pending:
                    return "bi-hourglass-split";
                case RequestStatus.Approved:
                    return "bi-check-circle";
                case RequestStatus.Cancelled:
                    return "bi-x-circle";
                default:
                    return "bi-circle";
            }
        }

        /// <summary>Chọn icon phù hợp theo tên loại nhà</summary>
        public static string CategoryIcon(string catName)
        {
            string name = (catName ?? string.Empty).ToLowerInvariant();
            if (name.Contains("trọ"))
            {
                return "bi-door-open";
            }
            if (name.Contains("mini"))
            {
                return "bi-building";
            }
            if (name.Contains("chung cư") || name.Contains("căn hộ"))
            {
                return "bi-buildings";
            }
            if (name.Contains("nguyên căn") || name.Contains("nhà"))
            {
                return "bi-house-door";
            }
            return "bi-house";
        }
    }
}
