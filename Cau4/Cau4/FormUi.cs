namespace Cau4;

internal static class FormUi
{
    public static DataGridView Grid() => new() { Dock = DockStyle.Fill, ReadOnly = true, MultiSelect = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AllowUserToAddRows = false, AllowUserToDeleteRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, AutoGenerateColumns = false, RowHeadersVisible = true };
    public static void Column(DataGridView grid, string property, string title) => grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = property, HeaderText = title, Name = property });
    public static void Field(FlowLayoutPanel panel, string title, Control control)
    {
        panel.Controls.Add(new Label { Text = title, AutoSize = true, Margin = new Padding(8, 10, 3, 0) });
        control.Width = 175; panel.Controls.Add(control);
    }
    public static void Button(FlowLayoutPanel panel, string title, Func<Task> action)
    {
        var button = new Button { Text = title, AutoSize = true, Height = 30 };
        button.Click += async (_, _) => {
            if (!panel.Enabled) return;
            panel.Enabled = false;
            try { await action(); }
            catch (Exception ex) { MessageBox.Show(ex.InnerException?.Message ?? ex.Message, "An Khang Clinic", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            finally { panel.Enabled = true; }
        };
        panel.Controls.Add(button);
    }
}