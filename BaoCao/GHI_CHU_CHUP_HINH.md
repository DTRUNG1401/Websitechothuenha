# Ghi chú chụp hình cho Chương 3 của báo cáo

Tài liệu này hướng dẫn chụp từng hình trong **Chương 3** theo mẫu báo cáo của thầy: mở file nào, chụp đoạn nào
(kèm số dòng), và cách mở trang web để chụp kết quả.

## Chuẩn bị chung

1. Làm xong các bước trong `README.md` (tạo database, Build, chạy được F5).
2. Trong Visual Studio nên:
   - Bật số dòng: **Tools → Options → Text Editor → All Languages → General → Line numbers**.
   - Thu nhỏ/phóng to chữ bằng **Ctrl + lăn chuột** để đoạn code vừa một màn hình.
   - Đóng bớt cửa sổ Output/Error List (**Shift+Alt+Enter** để toàn màn hình) cho ảnh gọn.
3. Công cụ chụp: **Windows + Shift + S** (Snipping Tool), chọn vùng chữ nhật.
4. Khi chụp trang web: dùng Chrome/Edge, cửa sổ rộng khoảng 1366px, zoom 100%. Muốn chụp **cả trang dài** thì
   bấm **F12 → Ctrl+Shift+P → gõ "Capture full size screenshot"**.
5. Số dòng dưới đây tính theo file trong repo (có thể lệch 1–2 dòng nếu bạn đã sửa code).
6. Ảnh nhà hiện là **khung xám** (chưa có ảnh thật). Nên chép ảnh thật vào `RentHouseMVC/Content/images` trước khi
   chụp kết quả – xem mục "Ảnh nhà" trong `README.md`.

---

## Hình 3.x – Layout page

| | |
|---|---|
| File | `RentHouseMVC/Views/Shared/_Layout.cshtml` |
| Đoạn nên chụp | **Dòng 13 – 78**: phần `<head>` (link Bootstrap, Bootstrap Icons, theme.css, site.css), header có menu `<nav>` và `@RenderBody()` |
| Chụp thêm (nếu cần) | Dòng 80 – 127: `<footer>` và các thẻ `<script>`, `@RenderSection("scripts", required: false)` |
| Hình phụ cho Admin | `RentHouseMVC/Areas/Admin/Views/Shared/_AdminLayout.cshtml` (dòng 27 – 60: menu bên trái; dòng 62 – 82: thanh trên và `@RenderBody()`) |

Gợi ý chú thích: *"Layout page _Layout.cshtml dùng chung cho các trang phía khách: khai báo Bootstrap, menu điều hướng
(menu loại nhà lấy từ database bằng child action `Html.Action("CategoryMenu", "Home")`) và vị trí `@RenderBody()`."*

---

## Hình 3.x – Database Diagram

Vẽ bằng SQL Server Management Studio:

1. Mở SSMS, kết nối `.\SQLEXPRESS`.
2. Ở **Object Explorer** mở **Databases → RentHouseDB**.
3. Chuột phải **Database Diagrams → New Database Diagram**.
   - Nếu SSMS hỏi *"...does not have one or more of the support objects required to use database diagramming. Do you wish to create them?"* → bấm **Yes**.
4. Trong hộp **Add Table** chọn cả 4 bảng `Account`, `Category`, `House`, `RentalRequest` → **Add** → **Close**.
5. Kéo sắp xếp: `Category` bên trái, `House` ở giữa, `RentalRequest` bên phải, `Account` đứng riêng phía dưới.
   Chuột phải vùng trống → **Arrange Tables** nếu muốn tự sắp. Chuột phải từng bảng → **Table View → Standard**
   để hiện cả kiểu dữ liệu và *Allow Nulls*.
6. Các đường nối sẽ hiện tên khóa ngoại khi rê chuột: `FK_House_Category`, `FK_RentalRequest_House`.
7. Bấm **Ctrl+S**, đặt tên `RentHouseDiagram`, rồi chụp màn hình vùng sơ đồ.
   (Hoặc chuột phải vùng trống → **Copy Diagram to Clipboard** → dán vào Word.)

**Nếu gặp lỗi** *"Database diagram support objects cannot be installed because this database does not have a valid owner"*:
mở **New Query** và chạy (thay `TEN-MAY\TenDangNhapWindows` bằng tài khoản Windows của bạn, xem ở góc Object Explorer):

```sql
USE RentHouseDB;
ALTER AUTHORIZATION ON DATABASE::RentHouseDB TO [TEN-MAY\TenDangNhapWindows];
```

Sau đó chuột phải **Database Diagrams → Refresh** và làm lại bước 3.

---

## Hình 3.x – Class Diagram

| | |
|---|---|
| File | `RentHouseMVC/Models/RentHouseClassDiagram.cd` |
| Cách mở | Solution Explorer → `Models` → nhấp đúp `RentHouseClassDiagram.cd` |
| Nội dung | 4 entity `Category`, `House`, `RentalRequest`, `Account` và lớp `RentHouseDBContext`; mũi tên quan hệ `Category → House` (thuộc tính `Houses`) và `House → RentalRequest` (thuộc tính `RentalRequests`) |

- Nếu file mở ra dạng XML hoặc báo không mở được: vào **Visual Studio Installer → Modify → Individual components** → tích **Class Designer** → Modify.
- Nhấp mũi tên nhỏ ở góc mỗi lớp để mở rộng danh sách thuộc tính. Có thể kéo lại vị trí các lớp cho đẹp rồi **Ctrl+S**.
- Xuất ảnh: chuột phải vùng trống của sơ đồ → **Export Diagram as Image...** → lưu PNG để chèn vào báo cáo.

---

## Hình 3.x – DbContext

| | |
|---|---|
| File | `RentHouseMVC/Models/RentHouseDBContext.cs` |
| Đoạn nên chụp | **Toàn bộ file (dòng 1 – 63)**. Nếu phải chia 2 hình: dòng 1 – 28 (static constructor gọi `Database.SetInitializer<RentHouseDBContext>(null)`, constructor `base("name=RentHouseDBContext")`, 4 `DbSet`) và dòng 30 – 61 (`OnModelCreating`: tắt `PluralizingTableNameConvention`, cấu hình quan hệ, `HasPrecision(18, 0)`, `IsUnicode(false)`) |
| Chụp kèm | Chuỗi kết nối trong `RentHouseMVC/Web.config` (dòng có `name="RentHouseDBContext"`) |

---

## Hình 3.x – Category class

| | |
|---|---|
| File | `RentHouseMVC/Models/Category.cs` |
| Đoạn nên chụp | **Toàn bộ file (dòng 1 – 30)**: `[Table("Category")]`, khóa chính `Id`, `CatName` với `[Required]`, `[StringLength(100)]`, `[Display]`, và navigation `ICollection<House> Houses` |

---

## Hình 3.x – House class

| | |
|---|---|
| File | `RentHouseMVC/Models/House.cs` |
| Đoạn nên chụp | **Dòng 1 – 72** (toàn bộ). Nếu dài quá một màn hình, chia 2 hình: dòng 1 – 45 (HouseId, Title, Address, District, Price, Acreage, Bedrooms) và dòng 45 – 72 (Description, Image, Status, PostedDate, CatId, navigation `Category` với `[ForeignKey("CatId")]`, `RentalRequests`) |

---

## Hình 3.x – Index Action

| | |
|---|---|
| File | `RentHouseMVC/Controllers/HouseController.cs` |
| Đoạn nên chụp | **Dòng 11 – 46**: hằng số `PageSize = 9`, khai báo `RentHouseDBContext db` và action `Index(int? id, int page = 1)` – lọc theo mã loại (`id` null thì lấy tất cả), sắp xếp theo ngày đăng, phân trang bằng `PagedList<House>` |
| Chụp kèm (tùy chọn) | `RentHouseMVC/Helpers/PagedList.cs` (lớp phân trang tự viết, dòng 37 – 71 là constructor dùng `Skip/Take`) |

---

## Hình 3.x – Details Action

| | |
|---|---|
| File | `RentHouseMVC/Controllers/HouseController.cs` |
| Đoạn nên chụp | **Dòng 139 – 152**: action `Details(int id)` (lấy nhà kèm loại bằng `Include`, `HttpNotFound()` nếu không có) |
| Chụp thêm | **Dòng 154 – 216**: action POST `SendRequest` (`[HttpPost]`, `[ValidateAntiForgeryToken]`, chặn nhà "Đã thuê", kiểm tra ngày xem, lưu `RentalRequest`) và **dòng 218 – 234**: `BuildDetailsModel` lấy 4 nhà cùng loại |

---

## Hình 3.x – Index View

| | |
|---|---|
| File | `RentHouseMVC/Views/House/Index.cshtml` |
| Đoạn nên chụp | Chia 2 hình: **dòng 1 – 52** (`@model HouseListViewModel`, tiêu đề trang, thanh loại nhà `@foreach (var c in Model.Categories)`) và **dòng 54 – 105** (bộ lọc `@Html.Partial("_HouseFilter", filter)`, dòng "12 nhà" + ô sắp xếp, lưới nhà `@foreach (var house in Model.Houses)` + `@Html.Partial("_HouseCard", house)`, thanh phân trang `_Pager`) |
| Chụp kèm | `RentHouseMVC/Views/Shared/_HouseCard.cshtml` (dòng 1 – 24): thẻ hiển thị 1 nhà – ảnh, giá `DisplayHelper.FormatMoney`, tiêu đề, dòng "80 m² · 3 PN · Bình Thạnh", nhãn "Đã thuê" |

---

## Hình 3.x – Kết quả trang Index

1. Chạy project (**F5**).
2. Cách 1 – tất cả nhà: trên menu bấm **Nhà cho thuê → Tất cả nhà** (URL `http://localhost:50780/House`).
   Có 12 nhà mẫu nên trang 1 hiện 9 nhà và **thanh phân trang** có trang 2 → nên chụp phần cuối trang để thấy phân trang.
3. Cách 2 – theo loại: **Nhà cho thuê → Căn hộ mini** (URL `http://localhost:50780/House/Index/4`); mục "Căn hộ mini" trên thanh loại nhà được gạch chân.
   Trên máy tính, bộ lọc nằm ở cột trái; thu nhỏ cửa sổ dưới 992px thì bộ lọc chuyển thành nút **Bộ lọc** mở khung trượt.
4. Chụp cả trang (F12 → Ctrl+Shift+P → *Capture full size screenshot*) hoặc chụp phần banner + 2 hàng nhà đầu.
5. Hình phụ gợi ý: trang tìm kiếm `http://localhost:50780/House/Search?district=Bình Thạnh&sort=gia-tang` để minh họa chức năng lọc/sắp xếp.

---

## Hình 3.x – Details View

| | |
|---|---|
| File | `RentHouseMVC/Views/House/Details.cshtml` |
| Đoạn nên chụp | Chia 2–3 hình: **dòng 1 – 70** (thông tin nhà: ảnh, tiêu đề, địa chỉ, thông số dạng danh sách 2 cột `spec-list`, mô tả), **dòng 72 – 141** (hộp đặt lịch: giá ở đầu hộp, `@if (Model.CanRequest)`, `Html.BeginForm("SendRequest", ...)`, `@Html.AntiForgeryToken()`, `TextBoxFor`/`ValidationMessageFor`; nhánh `else` hiện "Nhà đã có người thuê"), **dòng 144 – 172** (nhà cùng loại và `@section scripts` nạp jQuery Validation) |

---

## Hình 3.x – Kết quả trang Details

1. Từ trang chủ hoặc trang danh sách bấm vào một nhà **Còn trống**, ví dụ `http://localhost:50780/House/Details/10`
   (Căn hộ mini full nội thất trung tâm Phú Nhuận). Chụp cả trang: ảnh, thông số, hộp **Đặt lịch xem nhà** bên phải có giá `6.500.000 đ /tháng` ở đầu hộp, các nhà cùng loại bên dưới.
2. Hình minh họa kiểm tra dữ liệu: bấm **Gửi yêu cầu** khi để trống họ tên / nhập SĐT sai (vd `123`) → chụp các dòng báo lỗi đỏ.
3. Hình gửi thành công: nhập đủ (vd *Nguyễn Văn B*, *0909123456*, chọn ngày giờ ngày mai) → **Gửi yêu cầu** → chụp thông báo *"Gửi yêu cầu thành công!..."* trong hộp đặt lịch.
   Sau đó vào trang Admin → **Yêu cầu thuê** sẽ thấy yêu cầu mới ở trạng thái *Chờ duyệt* (hình cho phần quản trị).
4. Hình nhà **Đã thuê** (form bị ẩn): `http://localhost:50780/House/Details/3` hoặc `/House/Details/12`.

---

## Các hình gợi ý thêm cho phần quản trị (nếu báo cáo cần)

| Hình | Cách mở |
|---|---|
| Trang đăng nhập | `http://localhost:50780/Admin` (tự chuyển tới `/Admin/Account/Login`) |
| Dashboard (Tổng quan) | Đăng nhập `admin` / `123456` |
| Quản lý loại nhà | Menu **Loại nhà**; thử **Xóa** loại "Phòng trọ" để chụp thông báo *"Không thể xóa!..."* |
| Thêm nhà + upload ảnh | Menu **Nhà cho thuê → Đăng nhà mới** |
| Danh sách yêu cầu | Menu **Yêu cầu thuê**, bấm các tab *Chờ duyệt / Đã duyệt / Đã hủy* |
| Trang lỗi 404 | `http://localhost:50780/House/Details/9999` hoặc `http://localhost:50780/abc` |
| Trang lỗi 500 | Tạm sửa `Data Source` trong Web.config thành tên sai rồi mở trang chủ (nhớ sửa lại sau khi chụp) |
