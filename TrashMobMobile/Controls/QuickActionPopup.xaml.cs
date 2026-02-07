namespace TrashMobMobile.Controls;

using CommunityToolkit.Maui.Views;

public partial class QuickActionPopup : Popup
{
    public const string CreateEvent = "CreateEvent";
    public const string ReportLitter = "ReportLitter";

    public string? SelectedAction { get; private set; }

    public QuickActionPopup()
    {
        InitializeComponent();
    }

    private async void OnCreateEventClicked(object? sender, EventArgs e)
    {
        SelectedAction = CreateEvent;
        await CloseAsync();
    }

    private async void OnReportLitterClicked(object? sender, EventArgs e)
    {
        SelectedAction = ReportLitter;
        await CloseAsync();
    }

    private async void OnCancelClicked(object? sender, EventArgs e)
    {
        await CloseAsync();
    }
}
