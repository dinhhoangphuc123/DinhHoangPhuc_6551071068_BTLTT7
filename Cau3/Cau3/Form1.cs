using Cau3.Models;
using Microsoft.EntityFrameworkCore;

namespace Cau3;

public partial class Form1 : Form
{
    private readonly TextBox roomNumber = new() { Width = 120, MaxLength = 10 };
    private readonly NumericUpDown floor = new() { Width = 90, Minimum = 0, Maximum = 1000 };
    private readonly ComboBox roomType = new() { Width = 160, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox status = new() { Width = 130, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox filterType = new() { Width = 190, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox filterStatus = new() { Width = 180, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly PictureBox photo = new() { Width = 110, Height = 85, SizeMode = PictureBoxSizeMode.Zoom, BorderStyle = BorderStyle.FixedSingle };
    private readonly DataGridView grid = Ui.Grid();
    private readonly List<Image> thumbnails = new();
    private int? selectedId;
    private string? imageName;
    private bool loading;
    private static readonly string[] Statuses = { "Trống", "Đang ở", "Đang dọn" };
    private static string ImagesDirectory => Path.Combine(AppContext.BaseDirectory, "Images");

    public Form1()
    {
        InitializeComponent();
        Text = "Quản lý Phòng - Sunrise Homestay";
        ClientSize = new Size(1120, 620);
        MinimumSize = new Size(1000, 500);
        StartPosition = FormStartPosition.CenterScreen;
        var editor = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 145, Padding = new Padding(10) };
        Ui.Field(editor, "Số phòng", roomNumber);
        Ui.Field(editor, "Tầng số", floor);
        Ui.Field(editor, "Loại phòng", roomType);
        Ui.Field(editor, "Tình trạng", status);
        editor.Controls.Add(photo);
        editor.Controls.Add(Ui.Button("Chọn ảnh...", () => Ui.Run(ChooseImage)));
        editor.Controls.Add(Ui.Button("Thêm", () => Ui.Run(() => SaveRoom(false))));
        editor.Controls.Add(Ui.Button("Sửa", () => Ui.Run(() => SaveRoom(true))));
        editor.Controls.Add(Ui.Button("Xóa", () => Ui.Run(DeleteRoom)));
        editor.Controls.Add(Ui.Button("Làm mới", () => Ui.Run(() => { filterType.SelectedIndex = 0; filterStatus.SelectedIndex = 0; LoadRooms(); ClearEditor(); })));
        editor.Controls.Add(Ui.Button("Quản lý Loại phòng", () => Ui.Run(() =>
        {
            using var form = new LoaiPhongForm();
            form.ShowDialog(this);
            LoadTypes(); LoadRooms(); ClearEditor();
        })));
        var filters = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 48, Padding = new Padding(10, 3, 10, 3) };
        filters.Controls.Add(filterType);
        filters.Controls.Add(filterStatus);
        filters.Controls.Add(Ui.Button("Tìm kiếm", () => Ui.Run(() => { LoadRooms(); ClearEditor(); })));
        Ui.Column(grid, "MaPhong", "Mã phòng");
        grid.Columns.Add(new DataGridViewImageColumn { Name = "Anh", HeaderText = "Ảnh", ImageLayout = DataGridViewImageCellLayout.Zoom, ValuesAreIcons = false });
        grid.Columns["Anh"]!.DefaultCellStyle.NullValue = null;
        Ui.Column(grid, "SoPhong", "Số phòng");
        Ui.Column(grid, "TangSo", "Tầng");
        Ui.Column(grid, "TenLoai", "Loại phòng");
        Ui.Column(grid, "GiaMoiDem", "Giá/đêm");
        grid.Columns["GiaMoiDem"]!.DefaultCellStyle.Format = "N0";
        Ui.Column(grid, "TinhTrang", "Tình trạng");
        grid.RowTemplate.Height = 65;
        grid.SelectionChanged += (_, _) => { if (!loading) Ui.Run(SelectRoom); };
        Controls.Add(grid); Controls.Add(filters); Controls.Add(editor);
        status.Items.AddRange(Statuses);
        filterStatus.Items.Add("Tất cả tình trạng"); filterStatus.Items.AddRange(Statuses);
        filterStatus.SelectedIndex = 0;
        Shown += (_, _) => Ui.Run(() => { LoadTypes(); LoadRooms(); ClearEditor(); });
        FormClosed += (_, _) => { photo.Image?.Dispose(); foreach (var image in thumbnails) image.Dispose(); };
    }

    private void LoadTypes()
    {
        using var db = new HomestayContext();
        var types = db.LoaiPhongs.AsNoTracking().OrderBy(x => x.TenLoai).ToList();
        roomType.DataSource = types;
        roomType.DisplayMember = "TenLoai"; roomType.ValueMember = "MaLoai";
        filterType.DataSource = new[] { new LoaiPhong { MaLoai = 0, TenLoai = "Tất cả loại phòng" } }.Concat(types).ToList();
        filterType.DisplayMember = "TenLoai"; filterType.ValueMember = "MaLoai";
    }

    private void LoadRooms()
    {
        using var db = new HomestayContext();
        var query = db.Phongs.AsNoTracking().Include(x => x.MaLoaiNavigation).AsQueryable();
        if (filterType.SelectedItem is LoaiPhong type && type.MaLoai != 0)
            query = query.Where(x => x.MaLoai == type.MaLoai);
        if (filterStatus.SelectedIndex > 0)
        {
            var value = filterStatus.Text;
            query = query.Where(x => x.TinhTrang == value);
        }
        var rooms = query.OrderBy(x => x.SoPhong).ToList();
        loading = true;
        try
        {
            grid.Rows.Clear();
            foreach (var image in thumbnails) image.Dispose();
            thumbnails.Clear();
            foreach (var room in rooms)
            {
                var image = ReadImage(room.HinhAnh);
                if (image != null) thumbnails.Add(image);
                int index = grid.Rows.Add(room.MaPhong, (object?)image ?? DBNull.Value, room.SoPhong, (object?)room.TangSo ?? DBNull.Value,
                    (object?)room.MaLoaiNavigation?.TenLoai ?? DBNull.Value, (object?)room.MaLoaiNavigation?.GiaMoiDem ?? DBNull.Value, (object?)room.TinhTrang ?? DBNull.Value);
                grid.Rows[index].Tag = room;
            }
            grid.ClearSelection();
            selectedId = null;
        }
        finally { loading = false; }
    }

    private static Image? ReadImage(string? name)
    {
        if (string.IsNullOrWhiteSpace(name)) return null;
        var path = Path.Combine(ImagesDirectory, Path.GetFileName(name));
        if (!File.Exists(path)) return null;
        try { using var original = Image.FromFile(path); return new Bitmap(original); }
        catch (ArgumentException) { return null; }
    }

    private void SetPhoto(Image? image)
    {
        var old = photo.Image; photo.Image = image; old?.Dispose();
    }

    private void ChooseImage()
    {
        using var dialog = new OpenFileDialog { Filter = "Ảnh phòng|*.jpg;*.jpeg;*.png;*.bmp;*.gif", CheckFileExists = true };
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        using var source = Image.FromFile(dialog.FileName);
        var preview = new Bitmap(source);
        try
        {
            Directory.CreateDirectory(ImagesDirectory);
            string name = Guid.NewGuid().ToString("N") + Path.GetExtension(dialog.FileName).ToLowerInvariant();
            File.Copy(dialog.FileName, Path.Combine(ImagesDirectory, name));
            imageName = name;
            SetPhoto(preview);
        }
        catch { preview.Dispose(); throw; }
    }

    private void SelectRoom()
    {
        if (grid.SelectedRows.Count == 0 || grid.SelectedRows[0].Tag is not Phong room) return;
        selectedId = room.MaPhong;
        roomNumber.Text = room.SoPhong;
        floor.Value = Math.Clamp(room.TangSo ?? 0, 0, 1000);
        roomType.SelectedValue = room.MaLoai ?? 0;
        status.SelectedItem = room.TinhTrang;
        imageName = room.HinhAnh;
        SetPhoto(ReadImage(imageName));
    }

    private void ClearEditor()
    {
        selectedId = null; roomNumber.Clear(); floor.Value = 0;
        roomType.SelectedIndex = roomType.Items.Count > 0 ? 0 : -1;
        status.SelectedIndex = 0; imageName = null; SetPhoto(null); grid.ClearSelection();
    }

    private void SaveRoom(bool update)
    {
        if (update && selectedId == null) throw new InvalidOperationException("Vui lòng chọn phòng cần sửa.");
        if (string.IsNullOrWhiteSpace(roomNumber.Text)) throw new InvalidOperationException("Vui lòng nhập số phòng.");
        if (roomType.SelectedItem is not LoaiPhong type) throw new InvalidOperationException("Vui lòng tạo và chọn loại phòng.");
        if (status.SelectedIndex < 0) throw new InvalidOperationException("Vui lòng chọn tình trạng.");
        using var db = new HomestayContext();
        string number = roomNumber.Text.Trim();
        if (db.Phongs.Any(x => x.SoPhong == number && (!update || x.MaPhong != selectedId)))
            throw new InvalidOperationException("Số phòng đã tồn tại.");
        var room = update ? db.Phongs.Find(selectedId!.Value) ?? throw new InvalidOperationException("Phòng không còn tồn tại.") : new Phong();
        room.SoPhong = number; room.TangSo = (int)floor.Value; room.MaLoai = type.MaLoai;
        room.TinhTrang = status.Text; room.HinhAnh = imageName;
        if (!update) db.Phongs.Add(room);
        db.SaveChanges(); LoadRooms(); ClearEditor();
    }

    private void DeleteRoom()
    {
        if (selectedId == null) throw new InvalidOperationException("Vui lòng chọn phòng cần xóa.");
        if (MessageBox.Show("Xóa phòng đang chọn?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
        using var db = new HomestayContext();
        var room = db.Phongs.Find(selectedId.Value);
        if (room != null) { db.Phongs.Remove(room); db.SaveChanges(); }
        LoadRooms(); ClearEditor();
    }
}