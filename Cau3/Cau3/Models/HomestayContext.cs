using Microsoft.EntityFrameworkCore;

namespace Cau3.Models;

public partial class LoaiPhong
{
    public int MaLoai { get; set; }
    public string TenLoai { get; set; } = null!;
    public decimal? GiaMoiDem { get; set; }
    public string? MoTa { get; set; }
    public virtual ICollection<Phong> Phongs { get; set; } = new List<Phong>();
}

public partial class Phong
{
    public int MaPhong { get; set; }
    public string SoPhong { get; set; } = null!;
    public int? TangSo { get; set; }
    public string? TinhTrang { get; set; }
    public string? HinhAnh { get; set; }
    public int? MaLoai { get; set; }
    public virtual LoaiPhong? MaLoaiNavigation { get; set; }
}

public partial class HomestayContext : DbContext
{
    public virtual DbSet<LoaiPhong> LoaiPhongs { get; set; }
    public virtual DbSet<Phong> Phongs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(Environment.GetEnvironmentVariable("SUNRISE_CONNECTION_STRING")
            ?? "Server=localhost;Database=SunriseHomestay;Trusted_Connection=True;TrustServerCertificate=True;Connect Timeout=10");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LoaiPhong>(entity =>
        {
            entity.ToTable("LoaiPhong");
            entity.HasKey(e => e.MaLoai);
            entity.Property(e => e.TenLoai).HasMaxLength(100).IsRequired();
            entity.Property(e => e.GiaMoiDem).HasColumnType("decimal(18,2)");
            entity.Property(e => e.MoTa).HasMaxLength(255);
        });
        modelBuilder.Entity<Phong>(entity =>
        {
            entity.ToTable("Phong");
            entity.HasKey(e => e.MaPhong);
            entity.Property(e => e.SoPhong).HasMaxLength(10).IsUnicode(false).IsRequired();
            entity.Property(e => e.TinhTrang).HasMaxLength(20);
            entity.Property(e => e.HinhAnh).HasMaxLength(255);
            entity.HasOne(e => e.MaLoaiNavigation).WithMany(e => e.Phongs)
                .HasForeignKey(e => e.MaLoai).OnDelete(DeleteBehavior.ClientSetNull);
        });
    }
}