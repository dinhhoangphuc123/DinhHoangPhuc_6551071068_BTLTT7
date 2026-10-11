using Microsoft.EntityFrameworkCore;

namespace Cau4;

public class DoctorsForm : Form
{
    private readonly TextBox name = new() { MaxLength = 100 };
    private readonly TextBox specialty = new() { MaxLength = 100 };
    private readonly TextBox phone = new() { MaxLength = 15 };
    private readonly TextBox search = new() { MaxLength = 100 };
    private readonly DataGridView grid = FormUi.Grid();
    private int? selectedId;
    public DoctorsForm()
    {
        Text = "Quản lý Bác sĩ - An Khang Clinic"; ClientSize = new Size(940, 480); MinimumSize = new Size(900, 400); StartPosition = FormStartPosition.CenterParent;
        var fields = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 100, Padding = new Padding(8) };
        FormUi.Field(fields, "Họ tên", name); FormUi.Field(fields, "Chuyên khoa", specialty); FormUi.Field(fields, "SĐT", phone);
        FormUi.Button(fields, "Thêm", () => SaveAsync(false)); FormUi.Button(fields, "Sửa", () => SaveAsync(true)); FormUi.Button(fields, "Xóa", DeleteAsync);
        FormUi.Button(fields, "Làm mới", async () => { search.Clear(); await LoadAsync(); ClearEditor(); });
        FormUi.Field(fields, "Tìm tên / chuyên khoa", search); FormUi.Button(fields, "Tìm kiếm", async () => { await LoadAsync(); ClearEditor(); });
        FormUi.Column(grid, "MaBS", "Mã bác sĩ"); FormUi.Column(grid, "HoTen", "Họ tên"); FormUi.Column(grid, "ChuyenKhoa", "Chuyên khoa"); FormUi.Column(grid, "SDT", "SĐT");
        grid.SelectionChanged += (_, _) => {
            if (grid.CurrentRow?.DataBoundItem is not BacSi row || !grid.CurrentRow.Selected) return;
            selectedId = row.MaBS; name.Text = row.HoTen; specialty.Text = row.ChuyenKhoa; phone.Text = row.SDT;
        };
        Controls.Add(grid); Controls.Add(fields);
        Shown += async (_, _) => {
            fields.Enabled = false;
            try { await LoadAsync(); ClearEditor(); }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Lỗi tải bác sĩ"); }
            finally { fields.Enabled = true; }
        };
    }
    private async Task LoadAsync()
    {
        using var db = new ClinicContext(); var keyword = search.Text.Trim();
        grid.DataSource = await db.BacSis.AsNoTracking().Where(x => keyword == "" || x.HoTen.Contains(keyword) || (x.ChuyenKhoa != null && x.ChuyenKhoa.Contains(keyword))).OrderBy(x => x.HoTen).ToListAsync();
        grid.ClearSelection(); selectedId = null;
    }
    private void ClearEditor() { selectedId = null; grid.ClearSelection(); name.Clear(); specialty.Clear(); phone.Clear(); }
    private async Task SaveAsync(bool editing)
    {
        if (editing && selectedId is null) { MessageBox.Show("Hãy chọn bác sĩ cần sửa."); return; }
        if (string.IsNullOrWhiteSpace(name.Text)) { MessageBox.Show("Không được bỏ trống họ tên bác sĩ."); name.Focus(); return; }
        using var db = new ClinicContext(); var entity = editing ? await db.BacSis.FindAsync(selectedId!.Value) : new BacSi();
        if (entity is null) { MessageBox.Show("Bác sĩ không còn tồn tại."); await LoadAsync(); return; }
        entity.HoTen = name.Text.Trim(); entity.ChuyenKhoa = specialty.Text.Trim(); entity.SDT = phone.Text.Trim();
        if (!editing) db.BacSis.Add(entity);
        await db.SaveChangesAsync(); await LoadAsync(); ClearEditor();
    }
    private async Task DeleteAsync()
    {
        if (selectedId is not int id) { MessageBox.Show("Hãy chọn bác sĩ cần xóa."); return; }
        using var db = new ClinicContext();
        if (await db.LichKhams.AnyAsync(x => x.MaBS == id)) { MessageBox.Show("Bác sĩ đang có lịch khám. Hãy xử lý các lịch khám trước khi xóa bác sĩ."); return; }
        if (MessageBox.Show("Bạn có chắc muốn xóa bác sĩ đã chọn?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
        var entity = await db.BacSis.FindAsync(id);
        if (entity != null) { db.BacSis.Remove(entity); await db.SaveChangesAsync(); }
        await LoadAsync(); ClearEditor();
    }
}