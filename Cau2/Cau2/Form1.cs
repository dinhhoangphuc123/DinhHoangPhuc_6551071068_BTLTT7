using Cau2.Data;
using Cau2.Models;
using Microsoft.EntityFrameworkCore;

namespace Cau2;

public partial class Form1 : Form
{
    private int? selectedId;
    private bool busy;

    public Form1()
    {
        InitializeComponent();
        Shown += async (_, _) => await ExecuteAsync(LoadMembersAsync);
        btnAdd.Click += async (_, _) => await ExecuteAsync(() => SaveMemberAsync(false));
        btnEdit.Click += async (_, _) => await ExecuteAsync(() => SaveMemberAsync(true));
        btnDelete.Click += async (_, _) => await ExecuteAsync(DeleteMemberAsync);
        btnSearch.Click += async (_, _) => await ExecuteAsync(LoadMembersAsync);
        btnReset.Click += async (_, _) => await ExecuteAsync(async () =>
        {
            txtSearch.Clear();
            cboFilter.SelectedIndex = 0;
            await LoadMembersAsync();
        });
        grid.SelectionChanged += (_, _) => ShowSelectedMember();
        ResetInputs();
    }

    private async Task ExecuteAsync(Func<Task> action)
    {
        if (busy) return;
        busy = true;
        inputPanel.Enabled = false;
        grid.Enabled = false;
        UseWaitCursor = true;
        try { await action(); }
        catch (Exception ex)
        {
            MessageBox.Show(this, "Không thể thực hiện thao tác. Kiểm tra kết nối SQL Server và bảng HoiVien.\n"
                + ex.GetBaseException().Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            busy = false;
            inputPanel.Enabled = true;
            grid.Enabled = true;
            UseWaitCursor = false;
        }
    }

    private async Task LoadMembersAsync()
    {
        string name = txtSearch.Text.Trim();
        string? rank = cboFilter.SelectedIndex > 0 ? cboFilter.SelectedItem?.ToString() : null;
        using var db = new FitZoneContext();
        // Kết hợp đồng thời hai điều kiện tìm kiếm bằng toán tử &&.
        grid.DataSource = await db.HoiViens.AsNoTracking()
            .Where(h => (name == "" || h.HoTen.Contains(name))
                && (rank == null || h.HangThanhVien == rank))
            .OrderBy(h => h.MaHV).ToListAsync();
        grid.ClearSelection();
        grid.CurrentCell = null;
        ResetInputs();
    }

    private void ResetInputs()
    {
        selectedId = null;
        txtName.Clear();
        txtPhone.Clear();
        txtEmail.Clear();
        rbMale.Checked = true;
        rbFemale.Checked = false;
        dtpBirth.Value = DateTime.Today.AddYears(-15);
        dtpBirth.Checked = true;
        cboRank.SelectedIndex = 0;
        chkActive.Checked = true;
        errors.Clear();
    }

    private void ShowSelectedMember()
    {
        if (grid.CurrentRow?.DataBoundItem is not HoiVien member || !grid.CurrentRow.Selected) return;
        selectedId = member.MaHV;
        errors.Clear();
        txtName.Text = member.HoTen;
        txtPhone.Text = member.SDT ?? "";
        txtEmail.Text = member.Email ?? "";
        rbMale.Checked = member.GioiTinh == true;
        rbFemale.Checked = member.GioiTinh == false;
        dtpBirth.Value = member.NgaySinh?.ToDateTime(TimeOnly.MinValue) ?? DateTime.Today.AddYears(-15);
        dtpBirth.Checked = member.NgaySinh.HasValue;
        cboRank.SelectedIndex = cboRank.FindStringExact(member.HangThanhVien ?? "");
        chkActive.CheckState = member.TrangThai.HasValue
            ? (member.TrangThai.Value ? CheckState.Checked : CheckState.Unchecked) : CheckState.Indeterminate;
    }

    private bool ValidateInputs()
    {
        errors.Clear();
        var messages = new List<string>();
        void Invalid(Control control, string message) { errors.SetError(control, message); messages.Add(message); }
        if (string.IsNullOrWhiteSpace(txtName.Text)) Invalid(txtName, "Họ tên không được để trống.");
        string phone = txtPhone.Text.Trim();
        if (phone.Length < 9 || phone.Length > 11 || phone.Any(c => c < '0' || c > '9'))
            Invalid(txtPhone, "SĐT chỉ được chứa chữ số từ 0 đến 9 và phải dài 9–11 ký tự.");
        if (!txtEmail.Text.Trim().Contains('@')) Invalid(txtEmail, "Email phải chứa ký tự '@'.");
        if (!dtpBirth.Checked || dtpBirth.Value.Date > DateTime.Today.AddYears(-15))
            Invalid(dtpBirth, "Phải chọn ngày sinh; hội viên phải đủ 15 tuổi tính đến hôm nay.");
        if (!rbMale.Checked && !rbFemale.Checked) Invalid(gbGender, "Vui lòng chọn giới tính Nam hoặc Nữ.");
        if (cboRank.SelectedIndex < 0) Invalid(cboRank, "Vui lòng chọn hạng Basic, VIP hoặc Premium.");
        if (chkActive.CheckState == CheckState.Indeterminate) Invalid(chkActive, "Vui lòng chọn trạng thái hoạt động.");
        if (messages.Count == 0) return true;
        MessageBox.Show(this, string.Join(Environment.NewLine, messages), "Dữ liệu chưa hợp lệ",
            MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return false;
    }

    private async Task SaveMemberAsync(bool editing)
    {
        if (editing && selectedId == null) { NotifySelect(); return; }
        if (!ValidateInputs()) return;
        using var db = new FitZoneContext();
        var member = editing ? await db.HoiViens.FindAsync(selectedId!.Value) : new HoiVien();
        if (member == null) { NotifyMissing(); await LoadMembersAsync(); return; }
        // Đọc các control và ghi đúng kiểu dữ liệu vào bảng.
        member.HoTen = txtName.Text.Trim();
        member.SDT = txtPhone.Text.Trim();
        member.Email = txtEmail.Text.Trim();
        member.GioiTinh = rbMale.Checked;
        member.NgaySinh = DateOnly.FromDateTime(dtpBirth.Value.Date);
        member.HangThanhVien = cboRank.SelectedItem!.ToString();
        member.TrangThai = chkActive.Checked;
        if (!editing) db.HoiViens.Add(member);
        await db.SaveChangesAsync();
        MessageBox.Show(this, editing ? "Cập nhật hội viên thành công." : "Thêm hội viên thành công.",
            "FitZone", MessageBoxButtons.OK, MessageBoxIcon.Information);
        await LoadMembersAsync();
    }

    private async Task DeleteMemberAsync()
    {
        if (selectedId == null) { NotifySelect(); return; }
        if (MessageBox.Show(this, $"Bạn có muốn xóa hội viên {txtName.Text} (mã {selectedId})?",
            "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
        using var db = new FitZoneContext();
        var member = await db.HoiViens.FindAsync(selectedId.Value);
        if (member == null) { NotifyMissing(); await LoadMembersAsync(); return; }
        db.HoiViens.Remove(member);
        await db.SaveChangesAsync();
        MessageBox.Show(this, "Xóa hội viên thành công.", "FitZone", MessageBoxButtons.OK, MessageBoxIcon.Information);
        await LoadMembersAsync();
    }

    private void NotifySelect() => MessageBox.Show(this, "Vui lòng chọn một hội viên trong danh sách.", "FitZone");
    private void NotifyMissing() => MessageBox.Show(this, "Hội viên đã bị xóa hoặc không còn tồn tại.", "FitZone");

    private void Grid_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        string property = grid.Columns[e.ColumnIndex].DataPropertyName;
        if (e.Value is bool value && (property == "GioiTinh" || property == "TrangThai"))
        {
            e.Value = property == "GioiTinh" ? (value ? "Nam" : "Nữ") : (value ? "Đang hoạt động" : "Tạm ngưng");
            e.FormattingApplied = true;
        }
    }
}
