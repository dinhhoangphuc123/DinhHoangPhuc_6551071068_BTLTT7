using Cau3.Models;
using Microsoft.EntityFrameworkCore;

namespace Cau3;

public class LoaiPhongForm : Form
{
    private readonly TextBox name = new() { Width = 200, MaxLength = 100 };
    private readonly NumericUpDown price = new() { Width = 150, Maximum = 9999999999999999m, DecimalPlaces = 2, ThousandsSeparator = true };
    private readonly TextBox description = new() { Width = 280, MaxLength = 255 };
    private readonly TextBox search = new() { Width = 240, PlaceholderText = "Tìm tên loại phòng..." };
    private readonly DataGridView grid = Ui.Grid();
    private int? selectedId;

    public LoaiPhongForm()
    {
        Text = "Quản lý Loại phòng - Sunrise Homestay";
        ClientSize = new Size(850, 480); MinimumSize = new Size(800, 450);
        StartPosition = FormStartPosition.CenterParent;
        var editor = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 110, Padding = new Padding(10) };
        Ui.Field(editor, "Tên loại phòng", name);
        Ui.Field(editor, "Giá mỗi đêm", price);
        Ui.Field(editor, "Mô tả", description);
        editor.Controls.Add(Ui.Button("Thêm", () => Ui.Run(() => Save(false))));
        editor.Controls.Add(Ui.Button("Sửa", () => Ui.Run(() => Save(true))));
        editor.Controls.Add(Ui.Button("Xóa", () => Ui.Run(Delete)));
        editor.Controls.Add(Ui.Button("Làm mới", () => Ui.Run(() => { search.Clear(); LoadRows(); ClearEditor(); })));
        var filters = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 45, Padding = new Padding(10, 0, 10, 0) };
        filters.Controls.Add(search);
        filters.Controls.Add(Ui.Button("Tìm kiếm", () => Ui.Run(() => { LoadRows(); ClearEditor(); })));
        Ui.Column(grid, "MaLoai", "Mã loại"); Ui.Column(grid, "TenLoai", "Tên loại phòng");
        Ui.Column(grid, "GiaMoiDem", "Giá mỗi đêm"); Ui.Column(grid, "MoTa", "Mô tả");
        grid.Columns["GiaMoiDem"]!.DefaultCellStyle.Format = "N2";
        grid.SelectionChanged += (_, _) =>
        {
            if (grid.SelectedRows.Count == 0 || grid.SelectedRows[0].DataBoundItem is not LoaiPhong type) return;
            selectedId = type.MaLoai; name.Text = type.TenLoai;
            price.Value = Math.Clamp(type.GiaMoiDem ?? 0, price.Minimum, price.Maximum);
            description.Text = type.MoTa;
        };
        Controls.Add(grid); Controls.Add(filters); Controls.Add(editor);
        Shown += (_, _) => Ui.Run(() => { LoadRows(); ClearEditor(); });
    }

    private void LoadRows()
    {
        using var db = new HomestayContext();
        string keyword = search.Text.Trim();
        grid.DataSource = db.LoaiPhongs.AsNoTracking().Where(x => x.TenLoai.Contains(keyword)).OrderBy(x => x.MaLoai).ToList();
        grid.ClearSelection();
    }

    private void ClearEditor()
    {
        selectedId = null; name.Clear(); price.Value = 0; description.Clear(); grid.ClearSelection();
    }

    private void Save(bool update)
    {
        if (update && selectedId == null) throw new InvalidOperationException("Vui lòng chọn loại phòng cần sửa.");
        if (string.IsNullOrWhiteSpace(name.Text)) throw new InvalidOperationException("Vui lòng nhập tên loại phòng.");
        using var db = new HomestayContext();
        var type = update ? db.LoaiPhongs.Find(selectedId!.Value) ?? throw new InvalidOperationException("Loại phòng không còn tồn tại.") : new LoaiPhong();
        type.TenLoai = name.Text.Trim(); type.GiaMoiDem = price.Value; type.MoTa = description.Text.Trim();
        if (!update) db.LoaiPhongs.Add(type);
        db.SaveChanges(); LoadRows(); ClearEditor();
    }

    private void Delete()
    {
        if (selectedId == null) throw new InvalidOperationException("Vui lòng chọn loại phòng cần xóa.");
        using var db = new HomestayContext();
        if (db.Phongs.Any(x => x.MaLoai == selectedId)) throw new InvalidOperationException("Không thể xóa loại phòng đang có phòng sử dụng.");
        if (MessageBox.Show("Xóa loại phòng đang chọn?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
        var type = db.LoaiPhongs.Find(selectedId.Value);
        if (type != null) { db.LoaiPhongs.Remove(type); db.SaveChanges(); }
        LoadRows(); ClearEditor();
    }
}