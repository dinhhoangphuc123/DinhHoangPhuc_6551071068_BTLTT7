namespace Cau2
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            errors = new ErrorProvider(components);
            errors.ContainerControl = this;
            inputPanel = new Panel { Dock = DockStyle.Top, Height = 285 };
            txtName = new TextBox { Location = new Point(135, 20), Width = 250, MaxLength = 100 };
            txtPhone = new TextBox { Location = new Point(135, 57), Width = 250, MaxLength = 15 };
            txtEmail = new TextBox { Location = new Point(135, 94), Width = 250, MaxLength = 100 };
            dtpBirth = new DateTimePicker { Location = new Point(135, 131), Width = 250, Format = DateTimePickerFormat.Short, ShowCheckBox = true };
            cboRank = new ComboBox { Location = new Point(135, 168), Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };
            cboRank.Items.AddRange(new object[] { "Basic", "VIP", "Premium" });
            chkActive = new CheckBox { Location = new Point(20, 210), AutoSize = true, Text = "Đang hoạt động" };
            gbGender = new GroupBox { Location = new Point(415, 20), Size = new Size(210, 85), Text = "Giới tính" };
            rbMale = new RadioButton { Location = new Point(20, 35), AutoSize = true, Text = "Nam" };
            rbFemale = new RadioButton { Location = new Point(115, 35), AutoSize = true, Text = "Nữ" };
            gbGender.Controls.AddRange(new Control[] { rbMale, rbFemale });
            btnAdd = new Button { Location = new Point(705, 20), Size = new Size(130, 32), Text = "Thêm" };
            btnEdit = new Button { Location = new Point(705, 62), Size = new Size(130, 32), Text = "Sửa" };
            btnDelete = new Button { Location = new Point(705, 104), Size = new Size(130, 32), Text = "Xóa" };
            btnReset = new Button { Location = new Point(705, 146), Size = new Size(130, 32), Text = "Làm mới" };
            txtSearch = new TextBox { Location = new Point(20, 248), Width = 310, PlaceholderText = "Tìm theo họ tên" };
            cboFilter = new ComboBox { Location = new Point(460, 248), Width = 165, DropDownStyle = ComboBoxStyle.DropDownList };
            cboFilter.Items.AddRange(new object[] { "Tất cả", "Basic", "VIP", "Premium" });
            cboFilter.SelectedIndex = 0;
            btnSearch = new Button { Location = new Point(705, 244), Size = new Size(130, 32), Text = "Tìm kiếm" };
            inputPanel.Controls.AddRange(new Control[] {
                new Label { Text = "Họ tên", Location = new Point(20, 23), AutoSize = true },
                new Label { Text = "Số điện thoại", Location = new Point(20, 60), AutoSize = true },
                new Label { Text = "Email", Location = new Point(20, 97), AutoSize = true },
                new Label { Text = "Ngày sinh", Location = new Point(20, 134), AutoSize = true },
                new Label { Text = "Hạng thành viên", Location = new Point(20, 171), AutoSize = true },
                new Label { Text = "Hạng thành viên", Location = new Point(345, 251), AutoSize = true },
                txtName, txtPhone, txtEmail, dtpBirth, cboRank, chkActive, gbGender,
                btnAdd, btnEdit, btnDelete, btnReset, txtSearch, cboFilter, btnSearch
            });
            grid = new DataGridView {
                Dock = DockStyle.Fill, AutoGenerateColumns = false, ReadOnly = true,
                AllowUserToAddRows = false, AllowUserToDeleteRows = false,
                MultiSelect = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White, RowHeadersWidth = 30
            };
            grid.Columns.AddRange(new DataGridViewColumn[] {
                new DataGridViewTextBoxColumn { DataPropertyName = "MaHV", HeaderText = "Mã HV", FillWeight = 50 },
                new DataGridViewTextBoxColumn { DataPropertyName = "HoTen", HeaderText = "Họ tên", FillWeight = 140 },
                new DataGridViewTextBoxColumn { DataPropertyName = "GioiTinh", HeaderText = "Giới tính", FillWeight = 65 },
                new DataGridViewTextBoxColumn { DataPropertyName = "NgaySinh", HeaderText = "Ngày sinh", DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" } },
                new DataGridViewTextBoxColumn { DataPropertyName = "SDT", HeaderText = "SĐT" },
                new DataGridViewTextBoxColumn { DataPropertyName = "Email", HeaderText = "Email", FillWeight = 150 },
                new DataGridViewTextBoxColumn { DataPropertyName = "HangThanhVien", HeaderText = "Hạng thành viên" },
                new DataGridViewTextBoxColumn { DataPropertyName = "NgayDangKy", HeaderText = "Ngày đăng ký", DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy HH:mm" }, FillWeight = 125 },
                new DataGridViewTextBoxColumn { DataPropertyName = "TrangThai", HeaderText = "Trạng thái", FillWeight = 115 }
            });
            grid.CellFormatting += Grid_CellFormatting;
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(870, 560);
            MinimumSize = new Size(886, 599);
            Padding = new Padding(12);
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Quản Lý Hội Viên Phòng Gym FitZone";
            Controls.Add(grid);
            Controls.Add(inputPanel);
        }

        private Panel inputPanel;
        private TextBox txtName, txtPhone, txtEmail, txtSearch;
        private DateTimePicker dtpBirth;
        private ComboBox cboRank, cboFilter;
        private CheckBox chkActive;
        private GroupBox gbGender;
        private RadioButton rbMale, rbFemale;
        private Button btnAdd, btnEdit, btnDelete, btnReset, btnSearch;
        private DataGridView grid;
        private ErrorProvider errors;
        #endregion
    }
}
