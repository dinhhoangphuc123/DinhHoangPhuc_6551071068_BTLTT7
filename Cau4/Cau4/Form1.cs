using Microsoft.EntityFrameworkCore;

namespace Cau4;

public partial class Form1 : Form
{
    private readonly TextBox patient = new() { MaxLength = 100 };
    private readonly TextBox phone = new() { MaxLength = 15 };
    private readonly DateTimePicker date = new() { Format = DateTimePickerFormat.Short };
    private readonly DateTimePicker time = new() { Format = DateTimePickerFormat.Time, ShowUpDown = true };
    private readonly ComboBox doctor = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox status = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly DateTimePicker from = new() { Format = DateTimePickerFormat.Short, Value = DateTime.Today };
    private readonly DateTimePicker to = new() { Format = DateTimePickerFormat.Short, Value = DateTime.Today.AddMonths(1) };
    private readonly ComboBox filterDoctor = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly DataGridView grid = FormUi.Grid();
    private int? selectedId;
    private bool filtering;
    private bool loading;
    private readonly FlowLayoutPanel editor = new() { Dock = DockStyle.Top, Height = 110, Padding = new Padding(8), AutoScroll = true };
    private readonly FlowLayoutPanel filters = new() { Dock = DockStyle.Top, Height = 60, Padding = new Padding(8) };
    private sealed record DoctorOption(int Id, string Display);
    private sealed record AppointmentRow(int MaLich, string TenBenhNhan, string? SDT, DateOnly NgayKham, TimeOnly GioKham, int MaBS, string BacSi, string? ChuyenKhoa, string TrangThai);

    public Form1()
    {
        InitializeComponent();
        Text = "Quản lý Lịch khám bệnh - An Khang Clinic";
        ClientSize = new Size(1180, 650); MinimumSize = new Size(1100, 500); StartPosition = FormStartPosition.CenterScreen;
        FormUi.Field(editor, "Tên bệnh nhân", patient); FormUi.Field(editor, "Số điện thoại", phone);
        FormUi.Field(editor, "Ngày khám", date); FormUi.Field(editor, "Giờ khám", time);
        FormUi.Field(editor, "Bác sĩ", doctor); doctor.Width = 260;
        FormUi.Field(editor, "Trạng thái", status);
        status.Items.AddRange(new object[] { "Chờ khám", "Đã khám", "Đã hủy" }); status.SelectedIndex = 0;
        FormUi.Button(editor, "Thêm", () => SaveAsync(false)); FormUi.Button(editor, "Sửa", () => SaveAsync(true));
        FormUi.Button(editor, "Xóa", DeleteAsync);
        FormUi.Button(editor, "Làm mới", async () => { filtering = false; await LoadDoctorsAsync(); await LoadAppointmentsAsync(); ClearEditor(); });
        FormUi.Button(editor, "Quản lý Bác sĩ", async () => { using var form = new DoctorsForm(); form.ShowDialog(this); await LoadDoctorsAsync(); await LoadAppointmentsAsync(); ClearEditor(); });
        FormUi.Field(filters, "Từ ngày", from); FormUi.Field(filters, "Đến ngày", to);
        FormUi.Field(filters, "Bác sĩ", filterDoctor); filterDoctor.Width = 270;
        FormUi.Button(filters, "Tìm kiếm", async () => {
            if (from.Value.Date > to.Value.Date) { MessageBox.Show("Từ ngày phải nhỏ hơn hoặc bằng đến ngày."); return; }
            filtering = true; await LoadAppointmentsAsync(); ClearEditor();
        });
        foreach (var c in new[] { ("MaLich", "Mã lịch"), ("TenBenhNhan", "Tên bệnh nhân"), ("SDT", "SĐT"), ("NgayKham", "Ngày khám"), ("GioKham", "Giờ khám"), ("BacSi", "Bác sĩ"), ("ChuyenKhoa", "Chuyên khoa"), ("TrangThai", "Trạng thái") }) FormUi.Column(grid, c.Item1, c.Item2);
        grid.Columns["NgayKham"]!.DefaultCellStyle.Format = "dd/MM/yyyy";
        grid.Columns["GioKham"]!.DefaultCellStyle.Format = "HH:mm";
        grid.SelectionChanged += (_, _) => {
            if (loading || grid.CurrentRow?.DataBoundItem is not AppointmentRow row || !grid.CurrentRow.Selected) return;
            selectedId = row.MaLich; patient.Text = row.TenBenhNhan; phone.Text = row.SDT;
            date.Value = row.NgayKham.ToDateTime(TimeOnly.MinValue); time.Value = DateTime.Today.Add(row.GioKham.ToTimeSpan());
            doctor.SelectedValue = row.MaBS; status.SelectedItem = row.TrangThai;
        };
        Controls.Add(grid); Controls.Add(filters); Controls.Add(editor);
        Shown += async (_, _) => {
            editor.Enabled = filters.Enabled = false;
            try { await ClinicContext.InitializeDatabaseAsync(); await LoadDoctorsAsync(); await LoadAppointmentsAsync(); ClearEditor(); }
            catch (Exception ex) { MessageBox.Show("Không thể kết nối hoặc khởi tạo SQL Server. Kiểm tra server localhost và quyền tạo database.\n\n" + ex.Message, "An Khang Clinic", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            finally { editor.Enabled = filters.Enabled = true; }
        };
    }
    private async Task LoadDoctorsAsync()
    {
        var oldFilter = filterDoctor.SelectedValue is int id ? id : 0;
        using var db = new ClinicContext();
        var doctors = await db.BacSis.AsNoTracking().OrderBy(x => x.HoTen).ToListAsync();
        var options = doctors.Select(x => new DoctorOption(x.MaBS, $"BS. {x.HoTen} - {x.ChuyenKhoa}")).ToList();
        doctor.DisplayMember = "Display"; doctor.ValueMember = "Id"; doctor.DataSource = options; doctor.SelectedIndex = -1;
        filterDoctor.DisplayMember = "Display"; filterDoctor.ValueMember = "Id";
        filterDoctor.DataSource = new[] { new DoctorOption(0, "Tất cả bác sĩ") }.Concat(options).ToList();
        filterDoctor.SelectedValue = options.Any(x => x.Id == oldFilter) ? oldFilter : 0;
    }
    private async Task LoadAppointmentsAsync()
    {
        using var db = new ClinicContext();
        // Include tải kèm bác sĩ; Where kết hợp khoảng ngày và mã bác sĩ.
        var query = db.LichKhams.AsNoTracking().Include(x => x.MaBSNavigation).AsQueryable();
        if (filtering)
        {
            var start = DateOnly.FromDateTime(from.Value); var end = DateOnly.FromDateTime(to.Value);
            var doctorId = filterDoctor.SelectedValue is int id ? id : 0;
            query = query.Where(x => x.NgayKham >= start && x.NgayKham <= end && (doctorId == 0 || x.MaBS == doctorId));
        }
        var rows = await query.OrderBy(x => x.NgayKham).ThenBy(x => x.GioKham).ToListAsync();
        loading = true;
        try { grid.DataSource = rows.Select(x => new AppointmentRow(x.MaLich, x.TenBenhNhan, x.SDT, x.NgayKham, x.GioKham, x.MaBS, x.MaBSNavigation?.HoTen ?? "", x.MaBSNavigation?.ChuyenKhoa, x.TrangThai)).ToList(); grid.ClearSelection(); }
        finally { loading = false; }
        selectedId = null;
    }
    private void ClearEditor()
    {
        selectedId = null; grid.ClearSelection(); patient.Clear(); phone.Clear(); date.Value = DateTime.Today;
        time.Value = DateTime.Today.AddHours(8); doctor.SelectedIndex = -1; status.SelectedIndex = 0;
    }
    private async Task SaveAsync(bool editing)
    {
        if (editing && selectedId is null) { MessageBox.Show("Hãy chọn lịch khám cần sửa."); return; }
        if (string.IsNullOrWhiteSpace(patient.Text)) { MessageBox.Show("Không được bỏ trống tên bệnh nhân."); patient.Focus(); return; }
        if (doctor.SelectedValue is not int doctorId) { MessageBox.Show("Hãy chọn bác sĩ."); return; }
        if (date.Value.Date < DateTime.Today) { MessageBox.Show("Không được đặt lịch vào ngày trong quá khứ."); return; }
        using var db = new ClinicContext();
        var entity = editing ? await db.LichKhams.FindAsync(selectedId!.Value) : new LichKham();
        if (entity is null) { MessageBox.Show("Lịch khám không còn tồn tại."); await LoadAppointmentsAsync(); return; }
        entity.TenBenhNhan = patient.Text.Trim(); entity.SDT = phone.Text.Trim(); entity.MaBS = doctorId;
        entity.NgayKham = DateOnly.FromDateTime(date.Value); entity.GioKham = TimeOnly.FromDateTime(time.Value);
        entity.TrangThai = status.SelectedItem?.ToString() ?? "Chờ khám";
        if (!editing) db.LichKhams.Add(entity);
        await db.SaveChangesAsync(); await LoadAppointmentsAsync(); ClearEditor();
    }
    private async Task DeleteAsync()
    {
        if (selectedId is not int id) { MessageBox.Show("Hãy chọn lịch khám cần xóa."); return; }
        if (MessageBox.Show("Bạn có chắc muốn xóa lịch khám đã chọn?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
        using var db = new ClinicContext(); var entity = await db.LichKhams.FindAsync(id);
        if (entity != null) { db.LichKhams.Remove(entity); await db.SaveChangesAsync(); }
        await LoadAppointmentsAsync(); ClearEditor();
    }
}