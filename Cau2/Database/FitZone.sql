IF DB_ID(N'FitZone') IS NULL
    CREATE DATABASE FitZone;
GO
USE FitZone;
GO
IF OBJECT_ID(N'dbo.HoiVien', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.HoiVien
    (
        MaHV int IDENTITY(1,1) CONSTRAINT PK_HoiVien PRIMARY KEY,
        HoTen nvarchar(100) NOT NULL,
        GioiTinh bit NULL,
        NgaySinh date NULL,
        SDT varchar(15) NULL,
        Email varchar(100) NULL,
        HangThanhVien nvarchar(20) NULL,
        NgayDangKy datetime NULL CONSTRAINT DF_HoiVien_NgayDangKy DEFAULT GETDATE(),
        TrangThai bit NULL
    );
END;
GO
