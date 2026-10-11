namespace Cau1.Models;

public partial class TheLoaiSach
{
    public int MaTL { get; set; }
    public string TenTheLoai { get; set; } = null!;
    public string? MoTa { get; set; }
    public int? SoLuongSach { get; set; }
    public DateTime? NgayTao { get; set; }
}
