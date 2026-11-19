using System.Security.Cryptography;
using System.Text;

namespace RentHouseMVC.Helpers
{
    /// <summary>
    /// Băm mật khẩu bằng SHA256 (chuỗi hex 64 ký tự, chữ thường).
    /// VD: "123456" => 8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92
    /// </summary>
    public static class PasswordHelper
    {
        public static string HashSHA256(string input)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input ?? string.Empty));
                var builder = new StringBuilder(64);
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        /// <summary>
        /// So sánh mật khẩu người dùng nhập với chuỗi băm lưu trong database.
        /// </summary>
        public static bool Verify(string password, string storedHash)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(storedHash))
            {
                return false;
            }

            string inputHash = HashSHA256(password);
            string saved = storedHash.Trim().ToLowerInvariant();
            if (inputHash.Length != saved.Length)
            {
                return false;
            }

            // So sánh với thời gian không đổi (tránh dò mật khẩu theo thời gian phản hồi)
            int diff = 0;
            for (int i = 0; i < inputHash.Length; i++)
            {
                diff |= inputHash[i] ^ saved[i];
            }
            return diff == 0;
        }
    }
}
