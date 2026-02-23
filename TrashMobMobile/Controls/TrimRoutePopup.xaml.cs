namespace TrashMobMobile.Controls;

using CommunityToolkit.Maui.Extensions;

public partial class TrimRoutePopup : ContentView
{
    private readonly DateTimeOffset startTime;
    private readonly DateTimeOffset endTime;
    private readonly int totalMinutes;
    private int trimMinutes;

    public TrimRoutePopup(DateTimeOffset routeStartTime, DateTimeOffset routeEndTime)
    {
        InitializeComponent();

        startTime = routeStartTime;
        endTime = routeEndTime;
        totalMinutes = Math.Max((int)(endTime - startTime).TotalMinutes, 1);

        currentDurationLabel.Text = FormatDuration(totalMinutes);
        currentEndTimeLabel.Text = endTime.ToLocalTime().ToString("h:mm tt");

        trimSlider.Maximum = Math.Max(totalMinutes - 1, 1);
        trimSlider.Value = 0;
    }

    private void OnSliderValueChanged(object? sender, ValueChangedEventArgs e)
    {
        trimMinutes = (int)e.NewValue;
        trimLabel.Text = $"Remove from end: {trimMinutes} min";

        var showPreview = trimMinutes > 0;
        previewStack.IsVisible = showPreview;
        saveButton.IsEnabled = showPreview;

        if (showPreview)
        {
            var newEnd = startTime.AddMinutes(totalMinutes - trimMinutes);
            var newDuration = totalMinutes - trimMinutes;
            newEndTimeLabel.Text = $"New end time: {newEnd.ToLocalTime():h:mm tt}";
            newDurationLabel.Text = $"New duration: {FormatDuration(newDuration)}";
        }
    }

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        var newEnd = startTime.AddMinutes(totalMinutes - trimMinutes);
        await Shell.Current.ClosePopupAsync(newEnd.ToString("O"));
    }

    private async void OnCancelClicked(object? sender, EventArgs e)
    {
        await Shell.Current.ClosePopupAsync();
    }

    private static string FormatDuration(int minutes)
    {
        if (minutes >= 60)
        {
            var hours = minutes / 60;
            var mins = minutes % 60;
            return mins > 0 ? $"{hours} hr {mins} min" : $"{hours} hr";
        }

        return $"{minutes} min";
    }
}
