namespace TrashMobMobile.Controls;

using CommunityToolkit.Maui.Extensions;

public partial class ListSelectorPopup : ContentView
{
    public ListSelectorPopup(string title, IEnumerable<string> items)
    {
        InitializeComponent();
        titleLabel.Text = title;
        itemsList.ItemsSource = items.ToList();
    }

    private async void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.Count > 0 && e.CurrentSelection[0] is string selected)
        {
            await Shell.Current.ClosePopupAsync(selected);
        }
    }

    private async void OnCancelClicked(object? sender, EventArgs e)
    {
        await Shell.Current.ClosePopupAsync();
    }
}
