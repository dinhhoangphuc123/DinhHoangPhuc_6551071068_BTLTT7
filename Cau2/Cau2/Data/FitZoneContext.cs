using Cau2.Models;
using Microsoft.EntityFrameworkCore;

namespace Cau2.Data;

public partial class FitZoneContext : DbContext
{
    public DbSet<HoiVien> HoiViens { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
            optionsBuilder.UseSqlServer(Environment.GetEnvironmentVariable("FITZONE_CONNECTION_STRING")
                ?? @"Server=localhost;Database=FitZone;Trusted_Connection=True;TrustServerCertificate=True");
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HoiVien>(entity =>
        {
            entity.ToTable("HoiVien");
            entity.HasKey(e => e.MaHV);
            entity.Property(e => e.MaHV).ValueGeneratedOnAdd();
            entity.Property(e => e.HoTen).HasMaxLength(100).IsRequired();
            entity.Property(e => e.NgaySinh).HasColumnType("date");
            entity.Property(e => e.SDT).HasMaxLength(15).IsUnicode(false);
            entity.Property(e => e.Email).HasMaxLength(100).IsUnicode(false);
            entity.Property(e => e.HangThanhVien).HasMaxLength(20);
            entity.Property(e => e.NgayDangKy).HasColumnType("datetime").HasDefaultValueSql("GETDATE()");
        });
    }
}
