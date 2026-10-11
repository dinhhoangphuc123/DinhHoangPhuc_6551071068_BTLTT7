IF DB_ID(N'SunriseHomestay') IS NULL
    CREATE DATABASE SunriseHomestay;
GO
USE SunriseHomestay;
GO
IF OBJECT_ID(N'dbo.LoaiPhong', N'U') IS NULL
CREATE TABLE dbo.LoaiPhong (
    MaLoai int IDENTITY(1,1) PRIMARY KEY,
    TenLoai nvarchar(100) NOT NULL,
    GiaMoiDem decimal(18,2),
    MoTa nvarchar(255)
);
IF OBJECT_ID(N'dbo.Phong', N'U') IS NULL
CREATE TABLE dbo.Phong (
    MaPhong int IDENTITY(1,1) PRIMARY KEY,
    SoPhong varchar(10) NOT NULL,
    TangSo int,
    TinhTrang nvarchar(20),
    HinhAnh nvarchar(255),
    MaLoai int,
    CONSTRAINT FK_Phong_LoaiPhong FOREIGN KEY (MaLoai) REFERENCES dbo.LoaiPhong(MaLoai)
);
IF NOT EXISTS (SELECT 1 FROM dbo.LoaiPhong)
INSERT dbo.LoaiPhong(TenLoai, GiaMoiDem, MoTa) VALUES
(N'Phòng đơn', 350000, N'Phòng dành cho một khách'),
(N'Phòng đôi', 500000, N'Phòng dành cho hai khách'),
(N'Phòng VIP', 900000, N'Phòng cao cấp');
GO