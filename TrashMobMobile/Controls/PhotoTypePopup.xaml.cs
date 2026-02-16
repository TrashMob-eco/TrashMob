namespace TrashMobMobile.Controls;

using CommunityToolkit.Maui.Extensions;

public partial class PhotoTypePopup : ContentView
{
    public const string Before = "Before";
    public const string During = "During";
    public const string After = "After";

    public string? SelectedType { get; private set; }

    public PhotoTypePopup()
    {
        InitializeComponent();
    }

    private async void OnBeforeClicked(object? sender, EventArgs e)
    {
        SelectedType = Before;
        await Shell.Current.ClosePopupAsync(SelectedType);
    }

    private async void OnDuringClicked(object? sender, EventArgs e)
    {
        SelectedType = During;
        await Shell.Current.ClosePopupAsync(SelectedType);
    }

    private async void OnAfterClicked(object? sender, EventArgs e)
    {
        SelectedType = After;
        await Shell.Current.ClosePopupAsync(SelectedType);
    }

    private async void OnCancelClicked(object? sender, EventArgs e)
    {
        await Shell.Current.ClosePopupAsync();
    }
}
