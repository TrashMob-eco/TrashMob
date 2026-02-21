namespace TrashMobMobile.Controls;

using CommunityToolkit.Maui.Extensions;

public partial class QuickActionPopup : ContentView
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
        await Shell.Current.ClosePopupAsync(SelectedAction);
    }

    private async void OnReportLitterClicked(object? sender, EventArgs e)
    {
        SelectedAction = ReportLitter;
        await Shell.Current.ClosePopupAsync(SelectedAction);
    }

    private async void OnCancelClicked(object? sender, EventArgs e)
    {
        await Shell.Current.ClosePopupAsync();
    }
}
