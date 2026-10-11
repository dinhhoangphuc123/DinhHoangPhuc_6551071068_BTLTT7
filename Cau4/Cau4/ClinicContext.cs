using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace Cau4;

public partial class BacSi
{
    public int MaBS { get; set; }
    public string HoTen { get; set; } = "";
    public string? ChuyenKhoa { get; set; }
    public string? SDT { get; set; }
    public virtual ICollection<LichKham> LichKhams { get; set; } = new List<LichKham>();
}

public partial class LichKham
{
    public int MaLich { get; set; }
    public string TenBenhNhan { get; set; } = "";
    public string? SDT { get; set; }
    public DateOnly NgayKham { get; set; }
    public TimeOnly GioKham { get; set; }
    public int MaBS { get; set; }
    public string TrangThai { get; set; } = "Chờ khám";
    public virtual BacSi MaBSNavigation { get; set; } = null!;
}

public partial class ClinicContext : DbContext
{
    public static string ConnectionString => Environment.GetEnvironmentVariable("ANKHANG_CONNECTION_STRING")
        ?? "Server=localhost;Database=AnKhangClinic;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=True;Connect Timeout=10";
    public DbSet<BacSi> BacSis => Set<BacSi>();
    public DbSet<LichKham> LichKhams => Set<LichKham>();
    protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlServer(ConnectionString);
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BacSi>(e => {
            e.ToTable("BacSi"); e.HasKey(x => x.MaBS);
            e.Property(x => x.HoTen).HasMaxLength(100).IsRequired();
            e.Property(x => x.ChuyenKhoa).HasMaxLength(100);
            e.Property(x => x.SDT).HasMaxLength(15).IsUnicode(false);
        });
        modelBuilder.Entity<LichKham>(e => {
            e.ToTable("LichKham"); e.HasKey(x => x.MaLich);
            e.Property(x => x.TenBenhNhan).HasMaxLength(100).IsRequired();
            e.Property(x => x.SDT).HasMaxLength(15).IsUnicode(false);
            e.Property(x => x.NgayKham).HasColumnType("date");
            e.Property(x => x.GioKham).HasColumnType("time");
            e.Property(x => x.TrangThai).HasMaxLength(20).IsRequired();
            e.HasOne(x => x.MaBSNavigation).WithMany(x => x.LichKhams).HasForeignKey(x => x.MaBS).OnDelete(DeleteBehavior.ClientSetNull);
        });
    }
    public static async Task InitializeDatabaseAsync()
    {
        // Tạo bảng bằng SQL trước khi truy vấn qua EF (Database First).
        var builder = new SqlConnectionStringBuilder(ConnectionString) { InitialCatalog = "master" };
        await using var connection = new SqlConnection(builder.ConnectionString);
        await connection.OpenAsync();
        var script = await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, "Database.sql"));
        foreach (var batch in System.Text.RegularExpressions.Regex.Split(script, @"^GO\s*$", System.Text.RegularExpressions.RegexOptions.Multiline | System.Text.RegularExpressions.RegexOptions.IgnoreCase))
        {
            if (string.IsNullOrWhiteSpace(batch)) continue;
            await using var command = new SqlCommand(batch, connection);
            await command.ExecuteNonQueryAsync();
        }
    }
}