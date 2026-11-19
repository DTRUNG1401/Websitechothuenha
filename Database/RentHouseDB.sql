/* =====================================================================
   ĐỒ ÁN LẬP TRÌNH WEB - WEBSITE CHO THUÊ NHÀ (RentHouseMVC)
   Script tạo cơ sở dữ liệu RentHouseDB + dữ liệu mẫu
   ---------------------------------------------------------------------
   Cách chạy: mở file này bằng SQL Server Management Studio (SSMS),
   kết nối tới .\SQLEXPRESS rồi bấm Execute (F5).
   Script sẽ XÓA database RentHouseDB cũ (nếu có) và tạo lại từ đầu.
   ---------------------------------------------------------------------
   Collation Vietnamese_CI_AI: so sánh chuỗi không phân biệt hoa/thường
   và không phân biệt dấu thanh => tìm "phong tro" vẫn ra "Phòng trọ".
   ===================================================================== */

USE master;
GO

IF DB_ID(N'RentHouseDB') IS NOT NULL
BEGIN
    ALTER DATABASE RentHouseDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE RentHouseDB;
END
GO

CREATE DATABASE RentHouseDB COLLATE Vietnamese_CI_AI;
GO

USE RentHouseDB;
GO

/* ---------------------------------------------------------------------
   1. Bảng Category - Loại nhà
   --------------------------------------------------------------------- */
CREATE TABLE dbo.Category
(
    Id          INT            IDENTITY(1,1) NOT NULL,
    CatName     NVARCHAR(100)  NOT NULL,

    CONSTRAINT PK_Category PRIMARY KEY (Id),
    CONSTRAINT UQ_Category_CatName UNIQUE (CatName)
);
GO

/* ---------------------------------------------------------------------
   2. Bảng House - Nhà cho thuê
   --------------------------------------------------------------------- */
CREATE TABLE dbo.House
(
    HouseId     INT            IDENTITY(1,1) NOT NULL,
    Title       NVARCHAR(200)  NOT NULL,
    Address     NVARCHAR(255)  NOT NULL,
    District    NVARCHAR(50)   NOT NULL,
    Price       DECIMAL(18,0)  NOT NULL,
    Acreage     FLOAT          NOT NULL,
    Bedrooms    INT            NOT NULL,
    Description NVARCHAR(MAX)  NULL,
    Image       NVARCHAR(255)  NULL,
    Status      NVARCHAR(20)   NOT NULL CONSTRAINT DF_House_Status DEFAULT (N'Còn trống'),
    PostedDate  DATETIME       NOT NULL CONSTRAINT DF_House_PostedDate DEFAULT (GETDATE()),
    CatId       INT            NOT NULL,

    CONSTRAINT PK_House PRIMARY KEY (HouseId),
    CONSTRAINT FK_House_Category FOREIGN KEY (CatId)
        REFERENCES dbo.Category (Id),
    CONSTRAINT CK_House_Status   CHECK (Status IN (N'Còn trống', N'Đã thuê')),
    CONSTRAINT CK_House_Price    CHECK (Price > 0),
    CONSTRAINT CK_House_Acreage  CHECK (Acreage > 0),
    CONSTRAINT CK_House_Bedrooms CHECK (Bedrooms >= 0)
);
GO

CREATE NONCLUSTERED INDEX IX_House_CatId ON dbo.House (CatId);
GO

/* ---------------------------------------------------------------------
   3. Bảng RentalRequest - Yêu cầu thuê / đặt lịch xem nhà
      Xóa nhà => xóa luôn các yêu cầu của nhà đó (ON DELETE CASCADE)
   --------------------------------------------------------------------- */
CREATE TABLE dbo.RentalRequest
(
    RequestId     INT            IDENTITY(1,1) NOT NULL,
    HouseId       INT            NOT NULL,
    CustomerName  NVARCHAR(100)  NOT NULL,
    CustomerPhone VARCHAR(15)    NOT NULL,
    CustomerEmail NVARCHAR(100)  NULL,
    ViewingDate   DATETIME       NOT NULL,
    Note          NVARCHAR(500)  NULL,
    Status        NVARCHAR(20)   NOT NULL CONSTRAINT DF_RentalRequest_Status DEFAULT (N'Chờ duyệt'),
    CreatedDate   DATETIME       NOT NULL CONSTRAINT DF_RentalRequest_CreatedDate DEFAULT (GETDATE()),

    CONSTRAINT PK_RentalRequest PRIMARY KEY (RequestId),
    CONSTRAINT FK_RentalRequest_House FOREIGN KEY (HouseId)
        REFERENCES dbo.House (HouseId) ON DELETE CASCADE,
    CONSTRAINT CK_RentalRequest_Status CHECK (Status IN (N'Chờ duyệt', N'Đã duyệt', N'Đã hủy'))
);
GO

CREATE NONCLUSTERED INDEX IX_RentalRequest_HouseId ON dbo.RentalRequest (HouseId);
GO

/* ---------------------------------------------------------------------
   4. Bảng Account - Tài khoản quản trị
      Password lưu chuỗi SHA256 dạng hex (64 ký tự)
   --------------------------------------------------------------------- */
CREATE TABLE dbo.Account
(
    AccountId   INT            IDENTITY(1,1) NOT NULL,
    Username    NVARCHAR(50)   NOT NULL,
    Password    VARCHAR(64)    NOT NULL,
    FullName    NVARCHAR(100)  NOT NULL,

    CONSTRAINT PK_Account PRIMARY KEY (AccountId),
    CONSTRAINT UQ_Account_Username UNIQUE (Username)
);
GO

/* =====================================================================
   DỮ LIỆU MẪU
   ===================================================================== */

/* ---- Loại nhà ---- */
SET IDENTITY_INSERT dbo.Category ON;
INSERT INTO dbo.Category (Id, CatName) VALUES
(1, N'Nhà nguyên căn'),
(2, N'Phòng trọ'),
(3, N'Căn hộ chung cư'),
(4, N'Căn hộ mini');
SET IDENTITY_INSERT dbo.Category OFF;
GO

/* ---- Nhà cho thuê (ngày đăng tính lùi từ ngày chạy script) ---- */
SET IDENTITY_INSERT dbo.House ON;
INSERT INTO dbo.House (HouseId, Title, Address, District, Price, Acreage, Bedrooms, Description, Image, Status, PostedDate, CatId) VALUES
(1, N'Nhà nguyên căn 1 trệt 2 lầu hẻm xe hơi Nguyễn Văn Đậu',
    N'45/12 Nguyễn Văn Đậu, Phường 6', N'Bình Thạnh', 18000000, 80, 4,
N'Nhà 1 trệt 2 lầu, hẻm xe hơi 6m thông thoáng, an ninh tốt.
Gồm 4 phòng ngủ, 4 WC, phòng khách rộng, bếp riêng, sân thượng phơi đồ.
Có sẵn máy lạnh 3 phòng, máy nước nóng năng lượng mặt trời.
Phù hợp gia đình ở hoặc làm văn phòng công ty nhỏ. Hợp đồng tối thiểu 1 năm.',
    N'mau-01.svg', N'Còn trống', DATEADD(HOUR, -30, GETDATE()), 1),

(2, N'Nhà mặt tiền Lê Văn Sỹ tiện kinh doanh',
    N'128 Lê Văn Sỹ, Phường 13', N'Quận 3', 35000000, 120, 5,
N'Nhà mặt tiền đường lớn, vỉa hè rộng, thích hợp mở cửa hàng, showroom, quán cà phê.
Kết cấu 1 trệt 3 lầu, 5 phòng ngủ, 5 WC, mặt bằng trệt trống suốt.
Khu vực đông dân cư, gần chợ Lê Văn Sỹ và nhiều trường học.
Chủ nhà dễ thương, hỗ trợ giấy tờ đăng ký kinh doanh.',
    N'mau-02.svg', N'Còn trống', DATEADD(DAY, -6, GETDATE()), 1),

(3, N'Nhà nguyên căn gần chợ Hạnh Thông Tây',
    N'23/7 Quang Trung, Phường 10', N'Gò Vấp', 12000000, 64, 3,
N'Nhà 1 trệt 1 lầu, 3 phòng ngủ, 2 WC, hẻm 4m xe ba gác vào tận nhà.
Gần chợ Hạnh Thông Tây, siêu thị, bệnh viện 175, di chuyển ra trung tâm nhanh.
Điện nước giá nhà nước, không chung chủ.',
    N'mau-03.svg', N'Đã thuê', DATEADD(DAY, -20, GETDATE()), 1),

(4, N'Phòng trọ mới xây có gác lửng gần sân bay',
    N'156/8 Cộng Hòa, Phường 4', N'Tân Bình', 3500000, 22, 1,
N'Phòng mới xây 100%, có gác lửng, cửa sổ thoáng, WC riêng trong phòng.
Có kệ bếp, chỗ để xe miễn phí, camera an ninh, giờ giấc tự do (khóa vân tay).
Cách sân bay Tân Sơn Nhất 5 phút, gần Etown Cộng Hòa.
Điện 3.800 đ/kWh, nước 100.000 đ/người/tháng, wifi miễn phí.',
    N'mau-04.svg', N'Còn trống', DATEADD(HOUR, -5, GETDATE()), 2),

(5, N'Phòng trọ sinh viên gần ĐH Bách Khoa',
    N'45/3 Tô Hiến Thành, Phường 14', N'Quận 10', 3000000, 18, 1,
N'Phòng sạch sẽ, phù hợp sinh viên, đi bộ 5 phút tới ĐH Bách Khoa.
Có máy lạnh, giường, tủ quần áo, WC riêng.
Khu trọ yên tĩnh, có người quản lý, nhà xe rộng rãi.
Ưu tiên sinh viên ở lâu dài, cọc 1 tháng.',
    N'mau-05.svg', N'Còn trống', DATEADD(DAY, -4, GETDATE()), 2),

(6, N'Phòng trọ ban công thoáng mát khu Công nghệ cao',
    N'12/4 Lê Văn Việt, Phường Hiệp Phú', N'TP. Thủ Đức', 2500000, 20, 1,
N'Phòng có ban công, nhiều ánh sáng tự nhiên, gần Khu Công nghệ cao và làng đại học.
WC riêng, có chỗ nấu ăn, giờ giấc tự do, không chung chủ.
Gần chợ, siêu thị, trạm xe buýt.',
    NULL, N'Đã thuê', DATEADD(DAY, -15, GETDATE()), 2),

(7, N'Căn hộ chung cư 2 phòng ngủ view sông Sài Gòn',
    N'208 Nguyễn Hữu Cảnh, Phường 22', N'Bình Thạnh', 20000000, 80, 2,
N'Căn hộ tầng cao, view sông Sài Gòn cực đẹp, nội thất cao cấp đầy đủ.
2 phòng ngủ, 2 WC, phòng khách liên thông bếp, ban công rộng.
Tiện ích: hồ bơi, phòng gym, công viên ven sông, bảo vệ 24/7, hầm gửi xe.
Phí quản lý đã bao gồm trong giá thuê.',
    N'mau-07.svg', N'Còn trống', DATEADD(DAY, -3, GETDATE()), 3),

(8, N'Căn hộ 3 phòng ngủ khu Phú Mỹ Hưng',
    N'15 Nguyễn Lương Bằng, Phường Tân Phú', N'Quận 7', 25000000, 110, 3,
N'Căn hộ rộng rãi trong khu đô thị Phú Mỹ Hưng xanh sạch đẹp.
3 phòng ngủ, 2 WC, nội thất cơ bản, có máy lạnh tất cả các phòng.
Gần trường quốc tế, bệnh viện, siêu thị và khu ẩm thực.
Thích hợp cho gia đình hoặc chuyên gia nước ngoài.',
    N'mau-08.svg', N'Còn trống', DATEADD(DAY, -9, GETDATE()), 3),

(9, N'Căn hộ 1 phòng ngủ full nội thất gần Mai Chí Thọ',
    N'90 Mai Chí Thọ, Phường An Phú', N'TP. Thủ Đức', 11000000, 50, 1,
N'Căn hộ 1 phòng ngủ đầy đủ nội thất, chỉ việc xách vali vào ở.
Có máy giặt, tủ lạnh, bếp từ, rèm cửa, sofa.
Từ đây qua Quận 1 chỉ 10 phút qua hầm Thủ Thiêm.',
    N'mau-09.svg', N'Còn trống', DATEADD(DAY, -12, GETDATE()), 3),

(10, N'Căn hộ mini full nội thất trung tâm Phú Nhuận',
    N'35/2 Nguyễn Trọng Tuyển, Phường 15', N'Phú Nhuận', 6500000, 35, 1,
N'Căn hộ mini mới, thiết kế hiện đại, có cửa sổ lớn đón nắng.
Nội thất: giường, nệm, tủ, máy lạnh, tủ lạnh, máy giặt riêng.
Có thang máy, thẻ từ ra vào, dọn phòng 2 lần/tuần.
Gần sân bay và các tuyến đường lớn đi Quận 1, Quận 3.',
    N'mau-10.svg', N'Còn trống', DATEADD(HOUR, -2, GETDATE()), 4),

(11, N'Căn hộ mini có thang máy gần công viên Hoàng Văn Thụ',
    N'102 Bạch Đằng, Phường 2', N'Tân Bình', 7000000, 40, 1,
N'Căn hộ mini có ban công, bếp riêng, thang máy, bảo vệ.
Gần công viên Hoàng Văn Thụ, thuận tiện đi làm và đi sân bay.
Giá đã bao gồm phí dịch vụ, internet và rác.',
    N'mau-11.svg', N'Còn trống', DATEADD(DAY, -5, GETDATE()), 4),

(12, N'Căn hộ mini 2 phòng ngủ ngay trung tâm Quận 1',
    N'18 Trần Đình Xu, Phường Cô Giang', N'Quận 1', 9500000, 45, 2,
N'Căn hộ mini 2 phòng ngủ, phù hợp nhóm bạn hoặc gia đình trẻ.
Nằm ngay trung tâm, đi bộ ra phố đi bộ Bùi Viện, chợ Bến Thành.
Nội thất đầy đủ, có máy giặt chung, sân phơi trên sân thượng.',
    N'mau-12.svg', N'Đã thuê', DATEADD(DAY, -25, GETDATE()), 4);
SET IDENTITY_INSERT dbo.House OFF;
GO

/* ---- Yêu cầu thuê / đặt lịch xem nhà (đủ 3 trạng thái) ---- */
SET IDENTITY_INSERT dbo.RentalRequest ON;
INSERT INTO dbo.RentalRequest (RequestId, HouseId, CustomerName, CustomerPhone, CustomerEmail, ViewingDate, Note, Status, CreatedDate) VALUES
(1, 1, N'Nguyễn Văn An', '0901234567', N'an.nguyen@example.com',
    DATEADD(HOUR, 9, CAST(CAST(DATEADD(DAY, 2, GETDATE()) AS DATE) AS DATETIME)),
    N'Tôi muốn xem nhà vào buổi sáng, gia đình 5 người.', N'Chờ duyệt', DATEADD(HOUR, -20, GETDATE())),

(2, 4, N'Trần Thị Bình', '0912345678', NULL,
    DATEADD(HOUR, 18, CAST(CAST(DATEADD(DAY, 1, GETDATE()) AS DATE) AS DATETIME)),
    N'Sinh viên năm 3, cần thuê ở lâu dài.', N'Chờ duyệt', DATEADD(HOUR, -3, GETDATE())),

(3, 7, N'Lê Hoàng Cường', '0987654321', N'cuong.le@example.com',
    DATEADD(HOUR, 15, CAST(CAST(DATEADD(DAY, 3, GETDATE()) AS DATE) AS DATETIME)),
    N'Gia đình 3 người, có xe ô tô, cần chỗ đậu xe.', N'Đã duyệt', DATEADD(DAY, -2, GETDATE())),

(4, 3, N'Phạm Minh Đức', '0934567890', N'duc.pham@example.com',
    DATEADD(HOUR, 10, CAST(CAST(DATEADD(DAY, -18, GETDATE()) AS DATE) AS DATETIME)),
    NULL, N'Đã duyệt', DATEADD(DAY, -19, GETDATE())),

(5, 10, N'Võ Thị Hồng Nhung', '0978123456', NULL,
    DATEADD(HOUR, 17, CAST(CAST(DATEADD(DAY, 4, GETDATE()) AS DATE) AS DATETIME)),
    N'Cho tôi hỏi có chỗ để xe máy không?', N'Đã hủy', DATEADD(DAY, -1, GETDATE()));
SET IDENTITY_INSERT dbo.RentalRequest OFF;
GO

/* ---- Tài khoản quản trị: admin / 123456 (mật khẩu lưu dạng SHA256) ---- */
INSERT INTO dbo.Account (Username, Password, FullName) VALUES
(N'admin', '8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', N'Quản trị viên');
GO

/* ---- Kiểm tra nhanh ---- */
SELECT N'Category' AS [Bảng], COUNT(*) AS [Số dòng] FROM dbo.Category
UNION ALL SELECT N'House', COUNT(*) FROM dbo.House
UNION ALL SELECT N'RentalRequest', COUNT(*) FROM dbo.RentalRequest
UNION ALL SELECT N'Account', COUNT(*) FROM dbo.Account;
GO
