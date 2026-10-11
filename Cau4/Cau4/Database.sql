IF DB_ID(N'AnKhangClinic') IS NULL CREATE DATABASE AnKhangClinic;
GO
USE AnKhangClinic;
GO
IF OBJECT_ID(N'dbo.BacSi', N'U') IS NULL
CREATE TABLE dbo.BacSi (MaBS int IDENTITY PRIMARY KEY, HoTen nvarchar(100) NOT NULL, ChuyenKhoa nvarchar(100), SDT varchar(15));
IF OBJECT_ID(N'dbo.LichKham', N'U') IS NULL
CREATE TABLE dbo.LichKham (MaLich int IDENTITY PRIMARY KEY, TenBenhNhan nvarchar(100) NOT NULL, SDT varchar(15), NgayKham date NOT NULL, GioKham time NOT NULL, MaBS int NOT NULL, TrangThai nvarchar(20) NOT NULL, CONSTRAINT FK_LichKham_BacSi FOREIGN KEY(MaBS) REFERENCES dbo.BacSi(MaBS), CONSTRAINT CK_LichKham_TrangThai CHECK(TrangThai IN(N'Chờ khám', N'Đã khám', N'Đã hủy')));
GO
-- File UTF-8: khi chạy bằng sqlcmd, dùng -f 65001.
-- Sửa ràng buộc đã tạo sai mã hóa; rollback nếu dữ liệu hiện có không hợp lệ.
SET XACT_ABORT ON;
BEGIN TRY
    BEGIN TRANSACTION;
    IF EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = N'CK_LichKham_TrangThai' AND parent_object_id = OBJECT_ID(N'dbo.LichKham'))
        ALTER TABLE dbo.LichKham DROP CONSTRAINT CK_LichKham_TrangThai;
    ALTER TABLE dbo.LichKham WITH CHECK ADD CONSTRAINT CK_LichKham_TrangThai
        CHECK (TrangThai IN (N'Chờ khám', N'Đã khám', N'Đã hủy'));
    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO