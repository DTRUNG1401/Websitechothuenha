namespace RentHouseMVC.Models
{
    /// <summary>
    /// Giá trị hợp lệ của cột House.Status (khớp CHECK constraint CK_House_Status)
    /// </summary>
    public static class HouseStatus
    {
        public const string Available = "Còn trống";
        public const string Rented = "Đã thuê";

        public static readonly string[] All = { Available, Rented };
    }

    /// <summary>
    /// Giá trị hợp lệ của cột RentalRequest.Status (khớp CHECK constraint CK_RentalRequest_Status)
    /// </summary>
    public static class RequestStatus
    {
        public const string Pending = "Chờ duyệt";
        public const string Approved = "Đã duyệt";
        public const string Cancelled = "Đã hủy";

        public static readonly string[] All = { Pending, Approved, Cancelled };
    }
}
