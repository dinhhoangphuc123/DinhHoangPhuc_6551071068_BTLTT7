using Cau1.Models;
using Microsoft.EntityFrameworkCore;

namespace Cau1.Data;

public partial class TriThucBooksContext : DbContext
{
    public TriThucBooksContext() { }

    public TriThucBooksContext(DbContextOptions<TriThucBooksContext> options)
        : base(options) { }

    public virtual DbSet<TheLoaiSach> TheLoaiSachs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Kết nối SQL Server cục bộ bằng tài khoản Windows hiện tại.
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(
                "Server=localhost;Database=TriThucBooks;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Ánh xạ theo cấu trúc bảng hiện có trong SQL Server.
        modelBuilder.Entity<TheLoaiSach>(entity =>
        {
            entity.HasKey(e => e.MaTL);
            entity.ToTable("TheLoaiSach", "dbo");
            entity.HasIndex(e => e.TenTheLoai)
                .IsUnique()
                .HasDatabaseName("UQ_TheLoaiSach_TenTheLoai");
            entity.Property(e => e.MaTL).UseIdentityColumn();
            entity.Property(e => e.TenTheLoai).IsRequired().HasMaxLength(100);
            entity.Property(e => e.MoTa).HasMaxLength(255);
            entity.Property(e => e.SoLuongSach).HasDefaultValue(0);
            entity.Property(e => e.NgayTao)
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");
        });
    }
}
