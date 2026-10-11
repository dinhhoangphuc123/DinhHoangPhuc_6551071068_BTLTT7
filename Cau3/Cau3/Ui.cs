namespace Cau3;

internal static class Ui
{
    public static Button Button(string text, Action action)
    {
        var button = new Button { Text = text, AutoSize = true, Height = 32, Margin = new Padding(5) };
        button.Click += (_, _) => action();
        return button;
    }

    public static void Field(FlowLayoutPanel panel, string label, Control control)
    {
        var group = new FlowLayoutPanel { FlowDirection = FlowDirection.TopDown, Width = control.Width + 12, Height = 65 };
        group.Controls.Add(new Label { Text = label, AutoSize = true });
        group.Controls.Add(control);
        panel.Controls.Add(group);
    }

    public static void Run(Action action)
    {
        try { action(); }
        catch (Exception ex) { MessageBox.Show(ex.GetBaseException().Message, "Sunrise Homestay", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }

    public static DataGridView Grid() => new()
    {
        Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false,
        AllowUserToDeleteRows = false, MultiSelect = false,
        SelectionMode = DataGridViewSelectionMode.FullRowSelect,
        AutoGenerateColumns = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
        BackgroundColor = Color.White, RowHeadersVisible = false
    };

    public static void Column(DataGridView grid, string property, string title)
        => grid.Columns.Add(new DataGridViewTextBoxColumn { Name = property, DataPropertyName = property, HeaderText = title });
}