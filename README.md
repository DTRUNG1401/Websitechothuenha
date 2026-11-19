# RentHouseMVC – Website cho thuê nhà

Đồ án môn **Lập trình web**: xây dựng ứng dụng web cho thuê nhà tại TP.HCM bằng **ASP.NET MVC 5**,
**Entity Framework 6 (Database First)** và **SQL Server**.

- **Khách** (không cần đăng nhập): xem nhà theo loại, tìm kiếm / lọc / sắp xếp, xem chi tiết và gửi yêu cầu đặt lịch xem nhà.
- **Quản trị viên** (Area `Admin`, đăng nhập bằng Session): dashboard thống kê, quản lý loại nhà, nhà (có upload ảnh), yêu cầu thuê và đổi mật khẩu.

---

## 1. Công nghệ sử dụng

| Thành phần | Phiên bản / ghi chú |
|---|---|
| Ngôn ngữ | C# (.NET Framework **4.8**) |
| Framework web | **ASP.NET MVC 5.2.9** (Razor 3.2.9, WebPages 3.2.9) |
| ORM | **Entity Framework 6.4.4** – kiểu *Database First*: database tạo bằng script SQL, entity class và `RentHouseDBContext` viết tay khớp đúng script |
| Cơ sở dữ liệu | **SQL Server Express** (database `RentHouseDB`) |
| Giao diện | **Bootstrap 5.3.3**, **Bootstrap Icons 1.11.3** (copy thẳng vào project, lấy từ npm – không dùng CDN) |
| Kiểm tra dữ liệu phía client | jQuery 3.7.1, jQuery Validation 1.19.5, jQuery Validation Unobtrusive 4.0.0 (lấy từ npm) |
| Biên dịch view C# mới | Microsoft.CodeDom.Providers.DotNetCompilerPlatform 2.0.1 |
| IDE | Visual Studio 2022, chạy bằng IIS Express |

Gói NuGet (khai báo trong `RentHouseMVC/packages.config`, Visual Studio tự tải về thư mục `packages/` khi Build – thư mục này không được đưa lên GitHub):
`EntityFramework 6.4.4`, `Microsoft.AspNet.Mvc 5.2.9`, `Microsoft.AspNet.Razor 3.2.9`, `Microsoft.AspNet.WebPages 3.2.9`,
`Microsoft.Web.Infrastructure 2.0.0`, `Microsoft.CodeDom.Providers.DotNetCompilerPlatform 2.0.1`.

---

## 2. Cấu trúc thư mục

```
RentHouseMVC.sln                  <- mở file này bằng Visual Studio 2022
Database/
  RentHouseDB.sql                 <- script tạo database + dữ liệu mẫu
BaoCao/
  NOI_DUNG_BAO_CAO.md             <- nội dung viết sẵn cho báo cáo
  GHI_CHU_CHUP_HINH.md            <- hướng dẫn chụp hình cho Chương 3
RentHouseMVC/
  App_Start/                      <- RouteConfig, FilterConfig
  App_GlobalResources/            <- thông báo lỗi kiểu dữ liệu bằng tiếng Việt
  Areas/Admin/                    <- trang quản trị (Controllers, Views, _AdminLayout)
  Content/                        <- bootstrap, bootstrap-icons, site.css, admin.css, images/
    images/houses/                <- ảnh nhà (ảnh mẫu mau-*.svg + ảnh upload)
  Controllers/                    <- HomeController, HouseController, ErrorController
  Filters/                        <- AdminAuthorizeAttribute (kiểm tra đăng nhập Admin)
  Helpers/                        <- PagedList (phân trang tự viết), PasswordHelper (SHA256),
                                     DisplayHelper (định dạng giá), ImageHelper (upload ảnh)...
  Models/                         <- Category, House, RentalRequest, Account, RentHouseDBContext
    ViewModels/                   <- các lớp dữ liệu cho View / form
    RentHouseClassDiagram.cd      <- Class Diagram của Visual Studio
  Scripts/                        <- jquery, bootstrap.bundle, validation, site.js, admin.js
  Views/                          <- Home, House, Error, Shared (_Layout, partial)
  Global.asax, Web.config, packages.config, RentHouseMVC.csproj
```

---

## 3. Hướng dẫn cài đặt và chạy (từng bước)

### Bước 1 – Cài Visual Studio 2022
1. Tải **Visual Studio 2022 Community** (miễn phí) tại https://visualstudio.microsoft.com/vs/
2. Trong *Visual Studio Installer*, tab **Workloads**, tích chọn **ASP.NET and web development**.
3. Tab **Individual components**, kiểm tra đã tích:
   - **.NET Framework 4.8 targeting pack**
   - **Class Designer** (để mở file Class Diagram `.cd` khi làm báo cáo)
4. Bấm **Install** và chờ cài xong.

### Bước 2 – Cài SQL Server Express và SSMS
1. Tải **SQL Server 2022 Express** tại https://www.microsoft.com/sql-server/sql-server-downloads → chọn kiểu cài **Basic**.
   Tên instance mặc định là `SQLEXPRESS` → tên server là `.\SQLEXPRESS` (hoặc `TEN-MAY\SQLEXPRESS`).
2. Tải **SQL Server Management Studio (SSMS)** tại https://aka.ms/ssmsfullsetup và cài đặt.

### Bước 3 – Tải source code
- Cách 1: trên trang GitHub của repo bấm **Code → Download ZIP**, rồi giải nén.
- Cách 2: `git clone <link repo>`.

> Nên đặt project ở đường dẫn ngắn, **không có dấu tiếng Việt và khoảng trắng**, ví dụ `D:\DoAn\RentHouseMVC`.

### Bước 4 – Tạo database bằng script SQL
1. Mở **SSMS** → *Server name*: `.\SQLEXPRESS` → *Authentication*: **Windows Authentication** → **Connect**.
2. Menu **File → Open → File...** → chọn `Database\RentHouseDB.sql`.
3. Bấm **Execute** (F5). Script sẽ xóa database `RentHouseDB` cũ (nếu có), tạo lại 4 bảng và thêm dữ liệu mẫu.
4. Cuối script có bảng kiểm tra: `Category 4`, `House 12`, `RentalRequest 5`, `Account 1` là thành công.

### Bước 5 – Đổi tên server trong chuỗi kết nối (nếu cần)
Mở `RentHouseMVC\Web.config`, tìm:

```xml
<add name="RentHouseDBContext" connectionString="Data Source=.\SQLEXPRESS;Initial Catalog=RentHouseDB;Integrated Security=True" providerName="System.Data.SqlClient" />
```

Nếu ở Bước 4 bạn kết nối SSMS bằng tên server khác thì sửa phần `Data Source` cho giống, ví dụ:

| Bạn cài | Data Source |
|---|---|
| SQL Server Express (mặc định) | `.\SQLEXPRESS` |
| SQL Server bản Developer/Standard (instance mặc định) | `.` hoặc `localhost` |
| LocalDB đi kèm Visual Studio | `(localdb)\MSSQLLocalDB` |
| Instance có tên khác | `TEN-MAY\TEN-INSTANCE` |

### Bước 6 – Mở project, Build và chạy
1. Mở file **`RentHouseMVC.sln`** bằng Visual Studio 2022.
2. Menu **Build → Build Solution** (Ctrl+Shift+B). Lần đầu Visual Studio sẽ **tự tải các gói NuGet** về thư mục `packages` (cần Internet).
   Nếu không tự tải: chuột phải vào Solution → **Restore NuGet Packages**, rồi Build lại.
3. Bấm **F5** (hoặc Ctrl+F5) để chạy bằng IIS Express. Trình duyệt mở trang chủ `http://localhost:50780/`.

### Bước 7 – Đăng nhập trang quản trị
- Địa chỉ: `http://localhost:50780/Admin` (hoặc bấm nút **Quản trị** trên thanh menu)
- Tài khoản mẫu: **admin** / **123456**
- Nên vào **Đổi mật khẩu** để đổi mật khẩu sau khi đăng nhập lần đầu.

---

## 4. Xử lý lỗi thường gặp

| Lỗi | Cách xử lý |
|---|---|
| `Could not find a part of the path '...\bin\roslyn\csc.exe'` | Do lần Build đầu vừa tải gói NuGet. Chọn **Build → Rebuild Solution** rồi chạy lại (hoặc đóng và mở lại Visual Studio). |
| `This project references NuGet package(s) that are missing...` | Chuột phải Solution → **Restore NuGet Packages** → Build lại. Kiểm tra máy có Internet. |
| Trang lỗi 500 với chi tiết *A network-related or instance-specific error... SQL Server* | Sai `Data Source` (Bước 5) hoặc dịch vụ SQL Server chưa chạy: mở `services.msc` → **SQL Server (SQLEXPRESS)** → *Start*. |
| *Cannot open database "RentHouseDB" requested by the login* | Chưa chạy script SQL (Bước 4) hoặc đang kết nối nhầm server. |
| Port 50780 đã bị chương trình khác dùng | Chuột phải project → **Properties → Web** → đổi số port ở *Project Url* → **Create Virtual Directory**. |
| Tiếng Việt trong SSMS bị lỗi font | File script đã lưu dạng UTF-8 có BOM; hãy mở bằng **File → Open** của SSMS, không copy qua trình soạn thảo khác. |
| SSMS báo *Database diagram support objects cannot be installed...* khi vẽ Database Diagram | Xem cách xử lý trong `BaoCao/GHI_CHU_CHUP_HINH.md` (Hình Database Diagram). |

Khi chạy trên máy cá nhân (localhost), trang lỗi 500 hiển thị thêm dòng *Chi tiết kỹ thuật* để dễ tìm nguyên nhân; người truy cập từ máy khác chỉ thấy thông báo thân thiện.

---

## 5. Chức năng chính

**Phía khách**
1. Trang chủ: banner, ô tìm kiếm nhanh, thống kê, các loại nhà, 6 nhà còn trống mới đăng.
2. `/House/Index/{id}` – danh sách nhà theo loại (`id` = mã loại, bỏ trống = tất cả), phân trang 9 nhà/trang (tự viết `PagedList<T>`).
3. `/House/Search` – tìm theo từ khóa, lọc theo loại, quận, khoảng giá, diện tích tối thiểu, số phòng ngủ, chỉ nhà còn trống; sắp xếp theo ngày đăng hoặc giá.
4. `/House/Details/{id}` – ảnh, giá (dạng `12.000.000 đ/tháng`), diện tích, địa chỉ, mô tả, trạng thái, các nhà cùng loại.
5. Form **đặt lịch xem nhà** ngay trong trang chi tiết; nhà **"Đã thuê"** thì ẩn form (server cũng chặn).
6. Trang Giới thiệu, Liên hệ; trang lỗi 404 / 500 thân thiện.

**Phía quản trị** (`/Admin`)
1. Đăng nhập / đăng xuất bằng Session, mật khẩu so sánh dạng SHA256.
2. Dashboard: tổng số nhà, số nhà còn trống, số yêu cầu chờ duyệt, 5 yêu cầu mới nhất.
3. Thêm / sửa / xóa loại nhà – không cho xóa loại đang có nhà (báo lỗi rõ số nhà đang thuộc loại đó).
4. Thêm / sửa / xóa nhà, upload ảnh, đổi trạng thái Còn trống ⇄ Đã thuê bằng 1 cú bấm.
5. Danh sách yêu cầu thuê lọc theo trạng thái (Chờ duyệt / Đã duyệt / Đã hủy); duyệt, hủy, xóa, xem chi tiết.
6. Đổi mật khẩu.

---

## 6. Bảo mật đã áp dụng
- Filter `AdminAuthorizeAttribute` đăng ký toàn cục, kiểm tra Session cho **toàn bộ Area Admin** (chỉ trang đăng nhập có `[AllowAnonymous]`).
  Route mặc định giới hạn namespace (`UseNamespaceFallback = false`) để không thể gọi controller Admin qua URL ngoài Area.
- `[ValidateAntiForgeryToken]` cho **mọi** action POST, `@Html.AntiForgeryToken()` trong mọi form POST (kể cả nút Đăng xuất).
- Kiểm tra dữ liệu ở **cả client** (jQuery Validation Unobtrusive) **và server** (DataAnnotations + kiểm tra bổ sung trong controller).
- Dùng ViewModel / `[Bind(Include=...)]` để chống overposting.
- Upload ảnh: chỉ nhận `.jpg/.jpeg/.png/.webp`, < 2MB, kiểm tra "chữ ký" đầu file, **đổi tên ngẫu nhiên** bằng GUID.
- Mọi truy vấn đi qua Entity Framework (LINQ, có tham số), **không ghép chuỗi SQL**.
- Không dùng `Html.Raw` với dữ liệu người dùng nhập (Razor tự mã hóa HTML, mô tả nhà giữ xuống dòng bằng CSS `white-space: pre-line`).
- Mật khẩu lưu dạng băm SHA256, so sánh thời gian không đổi; cookie `HttpOnly`; chống Open Redirect với `Url.IsLocalUrl`.
- Ẩn header phiên bản (`X-AspNetMvc-Version`, `X-Powered-By`), thêm `X-Frame-Options`, `X-Content-Type-Options`.
- Trang lỗi 404 / 500 thân thiện, không lộ thông tin kỹ thuật cho người dùng bên ngoài.

---

## 7. Các lựa chọn thiết kế (tự quyết định)

| Vấn đề | Lựa chọn | Lý do |
|---|---|---|
| Kiểu EF | Database First viết tay (không EDMX, không Migrations) | Đúng yêu cầu đề; code entity rõ ràng, dễ chụp hình báo cáo. `Database.SetInitializer<RentHouseDBContext>(null)` đặt trong static constructor của DbContext. |
| Collation database | `Vietnamese_CI_AI` | Tìm kiếm không phân biệt hoa/thường và dấu thanh: gõ "phong tro" vẫn ra các nhà "Phòng trọ". |
| Cột cho phép NULL | `House.Description`, `House.Image`, `RentalRequest.CustomerEmail`, `RentalRequest.Note` | Nhà chưa có ảnh dùng ảnh placeholder; mô tả, email, ghi chú là thông tin không bắt buộc. Các cột khác `NOT NULL`. |
| Ràng buộc thêm | `CHECK` cho Status/Price/Acreage/Bedrooms, `UNIQUE` cho `Account.Username` và `Category.CatName`, `DEFAULT` cho Status và ngày | Dữ liệu luôn hợp lệ ngay ở mức database. |
| Quận/huyện | Danh sách cố định 22 quận/huyện TP.HCM (`Helpers/SearchOptions.cs`) | Tránh nhập sai chính tả, dùng chung cho form nhập và bộ lọc. |
| Khoảng giá | Ô chọn: dưới 3tr, 3–5tr, 5–10tr, 10–20tr, trên 20tr | Dễ dùng hơn 2 ô nhập số. Số phòng ngủ và diện tích lọc theo "từ ... trở lên". |
| Phân trang | Tự viết `PagedList<T>` + partial `_Pager` | Không cài thêm thư viện (đúng yêu cầu). Khách 9 nhà/trang, Admin 10 dòng/trang. |
| Định dạng số | `globalization culture="en-US"`, giá hiển thị bằng `DisplayHelper.FormatPrice` | Client (jQuery Validate) và server đọc số `45.5` giống nhau; giá vẫn hiển thị kiểu Việt Nam `12.000.000 đ/tháng`. |
| Thư viện JS | jQuery, jQuery Validation lấy từ npm và copy vào `Scripts/` | Giống cách lấy Bootstrap; giảm số gói NuGet. Không dùng `System.Web.Optimization` (bundling) để project gọn. |
| Ảnh mẫu | 11 ảnh minh họa SVG `mau-*.svg` + 1 nhà để trống ảnh (hiện placeholder) | Không phụ thuộc ảnh bản quyền bên ngoài. Ảnh upload mới lưu ở `Content/images/houses` với tên GUID. |
| Đăng xuất | Form POST có AntiForgeryToken | Tránh bị đăng xuất bằng link giả mạo (CSRF). |
| HTTPS | Tắt SSL của IIS Express | Chạy được ngay, không phải cài chứng chỉ khi F5. Khi publish thật nên bật HTTPS. |
| Port | `http://localhost:50780/` | Cố định để tài liệu hướng dẫn nhất quán. |

---

## 8. Kiểm tra trước khi đẩy lên GitHub
Project được viết trên môi trường Linux (không có Visual Studio/MSBuild cho .NET Framework), nên đã kiểm tra bằng các bước tự động:
- Biên dịch **toàn bộ code C# và cả 31 file view Razor** (sinh code bằng chính Razor 3 của MVC 5) trên reference assemblies .NET Framework 4.8 với đúng các DLL NuGet ở trên: **0 lỗi, 0 cảnh báo**.
- Phân tích cú pháp script SQL bằng thư viện T-SQL ScriptDom của Microsoft: 0 lỗi; số giá trị trong mọi lệnh INSERT khớp số cột.
- Cho Entity Framework 6.4.4 tự dựng mô hình từ `RentHouseDBContext` rồi so khớp với script: tên 4 bảng, tên/kiểu/độ dài/NULL của từng cột, 2 khóa ngoại và `ON DELETE CASCADE` **khớp hoàn toàn**; các truy vấn LINQ chính dịch được sang SQL có tham số.
- Rà toàn bộ 126 liên kết `Url.Action` / `Html.BeginForm` / `RedirectToAction` đều trỏ tới action có thật; mọi action POST có `[ValidateAntiForgeryToken]`; mọi file đều có trong `.csproj`.

Do không chạy được IIS Express + SQL Server trên Linux, bạn hãy chạy thử theo **Bước 6** trên Windows; nếu gặp lỗi xem mục 4.

---

## 9. Tài khoản mẫu
| Vai trò | Tên đăng nhập | Mật khẩu |
|---|---|---|
| Quản trị viên | `admin` | `123456` |

Mật khẩu trong database được lưu dạng SHA256 (`8d969eef...6c92`), không lưu mật khẩu gốc. Chuỗi kết nối dùng *Windows Authentication* nên không chứa mật khẩu SQL.
