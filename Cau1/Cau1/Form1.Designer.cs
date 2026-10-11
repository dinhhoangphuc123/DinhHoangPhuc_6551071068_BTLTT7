namespace Cau1;

partial class Form1
{
    private System.ComponentModel.IContainer? components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        inputPanel = new Panel();
        lblCategoryId = new Label();
        lblCategoryName = new Label();
        lblDescription = new Label();
        lblCreatedAtCaption = new Label();
        lblCreatedAt = new Label();
        lblSearch = new Label();
        txtCategoryId = new TextBox();
        txtCategoryName = new TextBox();
        txtDescription = new TextBox();
        txtSearch = new TextBox();
        btnAdd = new Button();
        btnEdit = new Button();
        btnDelete = new Button();
        btnRefresh = new Button();
        btnSearch = new Button();
        dgvCategories = new DataGridView();
        colCategoryId = new DataGridViewTextBoxColumn();
        colCategoryName = new DataGridViewTextBoxColumn();
        colDescription = new DataGridViewTextBoxColumn();
        colBookCount = new DataGridViewTextBoxColumn();
        colCreatedAt = new DataGridViewTextBoxColumn();
        inputPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvCategories).BeginInit();
        SuspendLayout();
        // Vùng nhập liệu và các nút thao tác theo ảnh mẫu.
        inputPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        inputPanel.Location = new Point(16, 12);
        inputPanel.Name = "inputPanel";
        inputPanel.Size = new Size(928, 248);
        inputPanel.TabIndex = 0;
        inputPanel.Controls.AddRange(new Control[] { lblCategoryId, lblCategoryName, lblDescription,
            lblCreatedAtCaption, lblCreatedAt, lblSearch, txtCategoryId, txtCategoryName,
            txtDescription, txtSearch, btnAdd, btnEdit, btnDelete, btnRefresh, btnSearch });
        lblCategoryId.AutoSize = true;
        lblCategoryId.Location = new Point(4, 14);
        lblCategoryId.Name = "lblCategoryId";
        lblCategoryId.Text = "Mã thể loại";
        lblCategoryName.AutoSize = true;
        lblCategoryName.Location = new Point(4, 51);
        lblCategoryName.Name = "lblCategoryName";
        lblCategoryName.Text = "Tên thể loại";
        lblDescription.AutoSize = true;
        lblDescription.Location = new Point(4, 88);
        lblDescription.Name = "lblDescription";
        lblDescription.Text = "Mô tả";
        lblCreatedAtCaption.AutoSize = true;
        lblCreatedAtCaption.Location = new Point(4, 177);
        lblCreatedAtCaption.Name = "lblCreatedAtCaption";
        lblCreatedAtCaption.Text = "Ngày tạo";
        lblCreatedAt.AutoSize = true;
        lblCreatedAt.Location = new Point(110, 177);
        lblCreatedAt.Name = "lblCreatedAt";
        lblCreatedAt.Text = "Chưa tạo";
        lblSearch.AutoSize = true;
        lblSearch.Location = new Point(4, 214);
        lblSearch.Name = "lblSearch";
        lblSearch.Text = "Tìm theo tên";
        txtCategoryId.Location = new Point(110, 10);
        txtCategoryId.Name = "txtCategoryId";
        txtCategoryId.ReadOnly = true;
        txtCategoryId.TabStop = false;
        txtCategoryId.Size = new Size(270, 27);
        txtCategoryName.Location = new Point(110, 47);
        txtCategoryName.Name = "txtCategoryName";
        txtCategoryName.Size = new Size(270, 27);
        txtCategoryName.TabIndex = 0;
        txtDescription.Location = new Point(110, 84);
        txtDescription.Name = "txtDescription";
        txtDescription.Multiline = true;
        txtDescription.ScrollBars = ScrollBars.Vertical;
        txtDescription.Size = new Size(270, 78);
        txtDescription.TabIndex = 1;
        txtSearch.Location = new Point(110, 210);
        txtSearch.Name = "txtSearch";
        txtSearch.Size = new Size(270, 27);
        txtSearch.TabIndex = 6;
        txtSearch.KeyDown += txtSearch_KeyDown;
        btnAdd.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnAdd.Location = new Point(490, 10);
        btnAdd.Name = "btnAdd";
        btnAdd.Size = new Size(100, 32);
        btnAdd.Text = "Thêm";
        btnAdd.TabIndex = 2;
        btnAdd.UseVisualStyleBackColor = true;
        btnAdd.Click += btnAdd_Click;
        btnEdit.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnEdit.Location = new Point(600, 10);
        btnEdit.Name = "btnEdit";
        btnEdit.Size = new Size(100, 32);
        btnEdit.Text = "Sửa";
        btnEdit.TabIndex = 3;
        btnEdit.UseVisualStyleBackColor = true;
        btnEdit.Click += btnEdit_Click;
        btnDelete.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnDelete.Location = new Point(710, 10);
        btnDelete.Name = "btnDelete";
        btnDelete.Size = new Size(100, 32);
        btnDelete.Text = "Xóa";
        btnDelete.TabIndex = 4;
        btnDelete.UseVisualStyleBackColor = true;
        btnDelete.Click += btnDelete_Click;
        btnRefresh.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnRefresh.Location = new Point(820, 10);
        btnRefresh.Name = "btnRefresh";
        btnRefresh.Size = new Size(100, 32);
        btnRefresh.Text = "Làm mới";
        btnRefresh.TabIndex = 5;
        btnRefresh.UseVisualStyleBackColor = true;
        btnRefresh.Click += btnRefresh_Click;
        btnSearch.Location = new Point(392, 207);
        btnSearch.Name = "btnSearch";
        btnSearch.Size = new Size(110, 32);
        btnSearch.Text = "Tìm kiếm";
        btnSearch.TabIndex = 7;
        btnSearch.UseVisualStyleBackColor = true;
        btnSearch.Click += btnSearch_Click;
        // Các cột chỉ hiển thị dữ liệu, không cho sửa trực tiếp trên lưới.
        colCategoryId.DataPropertyName = "MaTL";
        colCategoryId.HeaderText = "Mã TL";
        colCategoryId.Name = "colCategoryId";
        colCategoryId.FillWeight = 55;
        colCategoryName.DataPropertyName = "TenTheLoai";
        colCategoryName.HeaderText = "Tên thể loại";
        colCategoryName.Name = "colCategoryName";
        colCategoryName.FillWeight = 130;
        colDescription.DataPropertyName = "MoTa";
        colDescription.HeaderText = "Mô tả";
        colDescription.Name = "colDescription";
        colDescription.FillWeight = 220;
        colBookCount.DataPropertyName = "SoLuongSach";
        colBookCount.HeaderText = "Số lượng sách";
        colBookCount.Name = "colBookCount";
        colBookCount.FillWeight = 90;
        colCreatedAt.DataPropertyName = "NgayTao";
        colCreatedAt.HeaderText = "Ngày tạo";
        colCreatedAt.Name = "colCreatedAt";
        colCreatedAt.FillWeight = 150;
        colCreatedAt.DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";
        dgvCategories.AllowUserToAddRows = false;
        dgvCategories.AllowUserToDeleteRows = false;
        dgvCategories.AutoGenerateColumns = false;
        dgvCategories.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvCategories.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        dgvCategories.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvCategories.Columns.AddRange(new DataGridViewColumn[] { colCategoryId, colCategoryName,
            colDescription, colBookCount, colCreatedAt });
        dgvCategories.Location = new Point(16, 270);
        dgvCategories.MultiSelect = false;
        dgvCategories.Name = "dgvCategories";
        dgvCategories.ReadOnly = true;
        dgvCategories.RowHeadersWidth = 35;
        dgvCategories.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvCategories.Size = new Size(928, 294);
        dgvCategories.TabIndex = 1;
        dgvCategories.SelectionChanged += dgvCategories_SelectionChanged;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(960, 580);
        Controls.Add(inputPanel);
        Controls.Add(dgvCategories);
        Font = new Font("Segoe UI", 9F);
        MinimumSize = new Size(976, 500);
        Name = "Form1";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Quản Lý Thể Loại Sách - Tri Thức Books";
        Load += Form1_Load;
        inputPanel.ResumeLayout(false);
        inputPanel.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)dgvCategories).EndInit();
        ResumeLayout(false);
    }

    #endregion

    private Panel inputPanel;
    private Label lblCategoryId;
    private Label lblCategoryName;
    private Label lblDescription;
    private Label lblCreatedAtCaption;
    private Label lblCreatedAt;
    private Label lblSearch;
    private TextBox txtCategoryId;
    private TextBox txtCategoryName;
    private TextBox txtDescription;
    private TextBox txtSearch;
    private Button btnAdd;
    private Button btnEdit;
    private Button btnDelete;
    private Button btnRefresh;
    private Button btnSearch;
    private DataGridView dgvCategories;
    private DataGridViewTextBoxColumn colCategoryId;
    private DataGridViewTextBoxColumn colCategoryName;
    private DataGridViewTextBoxColumn colDescription;
    private DataGridViewTextBoxColumn colBookCount;
    private DataGridViewTextBoxColumn colCreatedAt;
}