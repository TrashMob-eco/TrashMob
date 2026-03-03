namespace TrashMobMobile.Controls;

using CommunityToolkit.Maui.Extensions;
using TrashMob.Models;

public partial class ReviewMetricsPopup : ContentView
{
    public const string ApprovedResult = "Approved";
    public const string RejectedPrefix = "Rejected:";

    private bool isRejecting;

    public ReviewMetricsPopup(EventAttendeeMetrics metrics)
    {
        InitializeComponent();

        attendeeLabel.Text = metrics.User?.UserName ?? "Unknown";
        bagsLabel.Text = (metrics.BagsCollected ?? 0).ToString();
        durationLabel.Text = (metrics.DurationMinutes ?? 0).ToString();

        if (metrics.PickedWeight.HasValue && metrics.PickedWeight.Value > 0)
        {
            var unit = metrics.PickedWeightUnitId == 2 ? "kg" : "lbs";
            weightLabel.Text = $"{metrics.PickedWeight.Value:0.##} {unit}";
        }
        else
        {
            weightLabel.Text = "—";
        }

        if (!string.IsNullOrWhiteSpace(metrics.Notes))
        {
            notesSection.IsVisible = true;
            notesLabel.Text = metrics.Notes;
        }
    }

    private async void OnApproveClicked(object? sender, EventArgs e)
    {
        await Shell.Current.ClosePopupAsync(ApprovedResult);
    }

    private async void OnRejectClicked(object? sender, EventArgs e)
    {
        if (!isRejecting)
        {
            // First click: show the reason editor
            isRejecting = true;
            rejectionSection.IsVisible = true;
            rejectButton.Text = "Confirm Rejection";
            return;
        }

        var reason = rejectionEditor.Text?.Trim();
        if (string.IsNullOrEmpty(reason))
        {
            rejectionEditor.Placeholder = "Please enter a reason";
            return;
        }

        await Shell.Current.ClosePopupAsync(RejectedPrefix + reason);
    }

    private async void OnCancelClicked(object? sender, EventArgs e)
    {
        await Shell.Current.ClosePopupAsync();
    }
}
