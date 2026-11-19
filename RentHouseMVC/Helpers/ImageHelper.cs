using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace RentHouseMVC.Helpers
{
    /// <summary>
    /// Xử lý ảnh nhà: đường dẫn hiển thị, kiểm tra và lưu file upload.
    /// </summary>
    public static class ImageHelper
    {
        public const string UploadFolder = "~/Content/images/houses/";
        public const string Placeholder = "~/Content/images/no-image.svg";
        public const int MaxFileSize = 2 * 1024 * 1024; // 2MB

        public static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };

        /// <summary>
        /// Trả về URL ảnh của nhà; nếu chưa có ảnh hoặc file không tồn tại thì trả về ảnh placeholder.
        /// Dùng trong View: &lt;img src="@Url.HouseImage(item.Image)" /&gt;
        /// </summary>
        public static string HouseImage(this UrlHelper url, string image)
        {
            string fileName = SafeFileName(image);
            if (fileName != null)
            {
                string physicalPath = HostingEnvironment.MapPath(UploadFolder + fileName);
                if (physicalPath != null && File.Exists(physicalPath))
                {
                    return url.Content(UploadFolder + fileName);
                }
            }
            return url.Content(Placeholder);
        }

        /// <summary>
        /// Kiểm tra file upload. Trả về null nếu hợp lệ (hoặc không chọn file), ngược lại trả về thông báo lỗi.
        /// </summary>
        public static string Validate(HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength == 0)
            {
                return null;
            }

            string extension = GetExtension(file.FileName);
            if (extension == null || !AllowedExtensions.Contains(extension))
            {
                return "Chỉ chấp nhận ảnh định dạng JPG, PNG hoặc WEBP.";
            }

            if (file.ContentLength > MaxFileSize)
            {
                return "Dung lượng ảnh phải nhỏ hơn 2MB.";
            }

            if (!HasImageSignature(file.InputStream, extension))
            {
                return "File tải lên không phải là ảnh hợp lệ.";
            }

            return null;
        }

        /// <summary>
        /// Lưu ảnh vào Content/images/houses với tên ngẫu nhiên (GUID). Trả về tên file mới.
        /// Phải gọi Validate trước.
        /// </summary>
        public static string Save(HttpPostedFileBase file)
        {
            string extension = GetExtension(file.FileName);
            if (extension == ".jpeg")
            {
                extension = ".jpg";
            }

            string fileName = Guid.NewGuid().ToString("N") + extension;
            string folder = HostingEnvironment.MapPath(UploadFolder);
            Directory.CreateDirectory(folder);
            file.SaveAs(Path.Combine(folder, fileName));
            return fileName;
        }

        /// <summary>
        /// Xóa ảnh đã upload. Không xóa ảnh mẫu (mau-*.svg) đi kèm project.
        /// </summary>
        public static void Delete(string image)
        {
            string fileName = SafeFileName(image);
            if (fileName == null || fileName.StartsWith("mau-", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            try
            {
                string physicalPath = HostingEnvironment.MapPath(UploadFolder + fileName);
                if (physicalPath != null && File.Exists(physicalPath))
                {
                    File.Delete(physicalPath);
                }
            }
            catch (IOException)
            {
                // Bỏ qua nếu file đang bị khóa - không ảnh hưởng dữ liệu
            }
            catch (UnauthorizedAccessException)
            {
            }
        }

        // Chỉ lấy phần tên file, chặn các chuỗi kiểu "../../web.config"
        private static string SafeFileName(string image)
        {
            if (string.IsNullOrWhiteSpace(image))
            {
                return null;
            }
            try
            {
                string fileName = Path.GetFileName(image.Trim());
                return string.IsNullOrEmpty(fileName) ? null : fileName;
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        private static string GetExtension(string fileName)
        {
            try
            {
                string extension = Path.GetExtension(fileName ?? string.Empty);
                return string.IsNullOrEmpty(extension) ? null : extension.ToLowerInvariant();
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        // Kiểm tra "chữ ký" đầu file để chắc chắn là ảnh thật, không phải file đổi đuôi
        private static bool HasImageSignature(Stream stream, string extension)
        {
            var header = new byte[12];
            int read;
            long originalPosition = stream.CanSeek ? stream.Position : 0;
            try
            {
                if (stream.CanSeek)
                {
                    stream.Position = 0;
                }
                read = stream.Read(header, 0, header.Length);
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
                }
            }

            switch (extension)
            {
                case ".jpg":
                case ".jpeg":
                    return read >= 3 && header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF;
                case ".png":
                    return read >= 8 && header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47
                        && header[4] == 0x0D && header[5] == 0x0A && header[6] == 0x1A && header[7] == 0x0A;
                case ".webp":
                    return read >= 12 && header[0] == (byte)'R' && header[1] == (byte)'I' && header[2] == (byte)'F' && header[3] == (byte)'F'
                        && header[8] == (byte)'W' && header[9] == (byte)'E' && header[10] == (byte)'B' && header[11] == (byte)'P';
                default:
                    return false;
            }
        }
    }
}
