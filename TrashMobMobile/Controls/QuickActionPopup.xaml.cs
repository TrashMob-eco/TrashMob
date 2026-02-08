namespace TrashMobMobile.Controls;

public partial class QuickActionPopup : ContentView
{
    public const string CreateEvent = "CreateEvent";
    public const string ReportLitter = "ReportLitter";

    public string? SelectedAction { get; private set; }

    public event EventHandler? ActionSelected;

    public QuickActionPopup()
    {
        InitializeComponent();
    }

    private void OnCreateEventClicked(object? sender, EventArgs e)
    {
        SelectedAction = CreateEvent;
        ActionSelected?.Invoke(this, EventArgs.Empty);
    }

    private void OnReportLitterClicked(object? sender, EventArgs e)
    {
        SelectedAction = ReportLitter;
        ActionSelected?.Invoke(this, EventArgs.Empty);
    }

    private void OnCancelClicked(object? sender, EventArgs e)
    {
        ActionSelected?.Invoke(this, EventArgs.Empty);
    }
}
