using Cau1.Data;
using Cau1.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Cau1;

public partial class Form1 : Form
{
    private bool isBusy;
    private string activeSearch = string.Empty;

    public Form1() => InitializeComponent();

    // Khóa giao diện khi chờ database để tránh các thao tác chồng nhau.
    private async Task RunOperationAsync(Func<Task> operation)
    {
        if (isBusy) return;
        isBusy = true;
        inputPanel.Enabled = false;
        dgvCategories.Enabled = false;
        UseWaitCursor = true;
        try { await operation(); }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException is SqlException sql && (sql.Number == 2601 || sql.Number == 2627))
                ShowMessage("Tên thể loại đã tồn tại. Vui lòng nhập tên khác.", MessageBoxIcon.Warning);
            else
                ShowMessage("Không thể lưu dữ liệu. " + ex.GetBaseException().Message, MessageBoxIcon.Error);
        }
        catch (Exception ex)
        {
            ShowMessage("Không thể thực hiện thao tác. " + ex.GetBaseException().Message, MessageBoxIcon.Error);
        }
        finally
        {
            isBusy = false;
            inputPanel.Enabled = true;
            dgvCategories.Enabled = true;
            UseWaitCursor = false;
        }
    }

    private void ShowMessage(string message, MessageBoxIcon icon = MessageBoxIcon.Information)
        => MessageBox.Show(this, message, "Tri Thức Books", MessageBoxButtons.OK, icon);

    // Lọc trên SQL Server bằng LINQ, chỉ cập nhật dữ liệu trên lưới.
    private async Task LoadCategoriesAsync()
    {
        using var context = new TriThucBooksContext();
        var query = context.TheLoaiSachs.AsNoTracking();
        if (!string.IsNullOrEmpty(activeSearch))
            query = query.Where(category => category.TenTheLoai.Contains(activeSearch));
        dgvCategories.DataSource = await query.OrderBy(category => category.MaTL).ToListAsync();
        ClearInputs();
    }

    private void ClearInputs()
    {
        dgvCategories.ClearSelection();
        dgvCategories.CurrentCell = null;
        txtCategoryId.Clear();
        txtCategoryName.Clear();
        txtDescription.Clear();
        lblCreatedAt.Text = "Chưa tạo";
    }

    private async void Form1_Load(object? sender, EventArgs e)
        => await RunOperationAsync(LoadCategoriesAsync);

    // Dùng đối tượng của dòng được chọn để tự điền thông tin lên vùng nhập.
    private void dgvCategories_SelectionChanged(object? sender, EventArgs e)
    {
        if (dgvCategories.SelectedRows.Count == 0) return;
        if (dgvCategories.SelectedRows[0].DataBoundItem is not TheLoaiSach category) return;
        txtCategoryId.Text = category.MaTL.ToString();
        txtCategoryName.Text = category.TenTheLoai;
        txtDescription.Text = category.MoTa ?? string.Empty;
        lblCreatedAt.Text = category.NgayTao?.ToString("dd/MM/yyyy HH:mm:ss") ?? "Chưa có dữ liệu";
    }

    // Kiểm tra dữ liệu và tên trùng theo quy tắc so sánh của database.
    private async Task<bool> ValidateInputAsync(TriThucBooksContext context, int? currentId = null)
    {
        var name = txtCategoryName.Text.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            ShowMessage("Vui lòng nhập tên thể loại.", MessageBoxIcon.Warning);
            return false;
        }
        if (name.Length > 100 || txtDescription.Text.Trim().Length > 255)
        {
            ShowMessage("Tên thể loại tối đa 100 ký tự; mô tả tối đa 255 ký tự.", MessageBoxIcon.Warning);
            return false;
        }
        if (await context.TheLoaiSachs.AnyAsync(category => category.TenTheLoai == name
            && (!currentId.HasValue || category.MaTL != currentId)))
        {
            ShowMessage("Tên thể loại đã tồn tại. Vui lòng nhập tên khác.", MessageBoxIcon.Warning);
            return false;
        }
        return true;
    }

    private void SetEditableValues(TheLoaiSach category)
    {
        category.TenTheLoai = txtCategoryName.Text.Trim();
        var description = txtDescription.Text.Trim();
        category.MoTa = description.Length == 0 ? null : description;
    }

    // SQL Server tự cấp mã, số lượng mặc định và ngày tạo khi thêm.
    private async void btnAdd_Click(object? sender, EventArgs e)
    {
        await RunOperationAsync(async () =>
        {
            using var context = new TriThucBooksContext();
            if (!await ValidateInputAsync(context)) return;
            var category = new TheLoaiSach();
            SetEditableValues(category);
            context.Add(category);
            await context.SaveChangesAsync();
            ShowMessage("Thêm thể loại thành công.");
            await LoadCategoriesAsync();
        });
    }

    private bool TryGetSelectedId(out int id)
    {
        if (int.TryParse(txtCategoryId.Text, out id)) return true;
        ShowMessage("Vui lòng chọn thể loại trên danh sách.", MessageBoxIcon.Warning);
        return false;
    }

    private async void btnEdit_Click(object? sender, EventArgs e)
    {
        if (!TryGetSelectedId(out var id)) return;
        await RunOperationAsync(async () =>
        {
            using var context = new TriThucBooksContext();
            if (!await ValidateInputAsync(context, id)) return;
            var category = await context.TheLoaiSachs.FindAsync(id);
            if (category == null)
            {
                ShowMessage("Thể loại này không còn tồn tại. Danh sách sẽ được tải lại.", MessageBoxIcon.Warning);
                await LoadCategoriesAsync();
                return;
            }
            SetEditableValues(category);
            await context.SaveChangesAsync();
            ShowMessage("Cập nhật thể loại thành công.");
            await LoadCategoriesAsync();
        });
    }

    // Xóa sau xác nhận; nhận diện lỗi ràng buộc tham chiếu của SQL Server.
    private async void btnDelete_Click(object? sender, EventArgs e)
    {
        if (!TryGetSelectedId(out var id)) return;
        var categoryName = dgvCategories.SelectedRows.Count > 0
            ? (dgvCategories.SelectedRows[0].DataBoundItem as TheLoaiSach)?.TenTheLoai : string.Empty;
        if (MessageBox.Show(this, $"Bạn có chắc muốn xóa thể loại \"{categoryName}\" (mã {id})?",
            "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question,
            MessageBoxDefaultButton.Button2) != DialogResult.Yes) return;
        await RunOperationAsync(async () =>
        {
            using var context = new TriThucBooksContext();
            var category = await context.TheLoaiSachs.FindAsync(id);
            if (category == null)
            {
                ShowMessage("Thể loại này không còn tồn tại. Danh sách sẽ được tải lại.", MessageBoxIcon.Warning);
                await LoadCategoriesAsync();
                return;
            }
            context.Remove(category);
            try { await context.SaveChangesAsync(); }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 547 })
            {
                ShowMessage("Không thể xóa vì thể loại đang được sách khác tham chiếu hoặc có ràng buộc dữ liệu.", MessageBoxIcon.Warning);
                return;
            }
            ShowMessage("Xóa thể loại thành công.");
            await LoadCategoriesAsync();
        });
    }

    private async void btnRefresh_Click(object? sender, EventArgs e)
    {
        await RunOperationAsync(async () =>
        {
            txtSearch.Clear();
            activeSearch = string.Empty;
            ClearInputs();
            await LoadCategoriesAsync();
        });
        txtCategoryName.Focus();
    }

    private async void btnSearch_Click(object? sender, EventArgs e) => await SearchAsync();

    private async Task SearchAsync()
    {
        await RunOperationAsync(async () =>
        {
            activeSearch = txtSearch.Text.Trim();
            await LoadCategoriesAsync();
        });
    }

    private async void txtSearch_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode != Keys.Enter) return;
        e.SuppressKeyPress = true;
        await SearchAsync();
    }
}
