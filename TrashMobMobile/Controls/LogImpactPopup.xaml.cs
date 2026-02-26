namespace TrashMobMobile.Controls;

using System.Text.Json;
using CommunityToolkit.Maui.Extensions;
using TrashMob.Models;

public partial class LogImpactPopup : ContentView
{
    // Weight unit IDs matching the WeightUnit lookup table
    private const int PoundsUnitId = 1;
    private const int KilogramsUnitId = 2;

    public LogImpactPopup(EventAttendeeMetrics? existing = null)
    {
        InitializeComponent();

        WeightUnitPicker.SelectedIndex = 0; // Default to lbs

        if (existing is not null)
        {
            if (existing.BagsCollected.HasValue)
            {
                BagsEntry.Text = existing.BagsCollected.Value.ToString();
            }

            if (existing.PickedWeight.HasValue)
            {
                WeightEntry.Text = existing.PickedWeight.Value.ToString("0.##");
            }

            if (existing.PickedWeightUnitId == KilogramsUnitId)
            {
                WeightUnitPicker.SelectedIndex = 1;
            }

            if (existing.DurationMinutes.HasValue)
            {
                DurationEntry.Text = existing.DurationMinutes.Value.ToString();
            }

            if (!string.IsNullOrEmpty(existing.Notes))
            {
                NotesEditor.Text = existing.Notes;
            }
        }
    }

    private async void OnSubmitClicked(object? sender, EventArgs e)
    {
        int.TryParse(BagsEntry.Text, out var bags);
        decimal.TryParse(WeightEntry.Text, out var weight);
        int.TryParse(DurationEntry.Text, out var duration);

        var weightUnitId = WeightUnitPicker.SelectedIndex == 1 ? KilogramsUnitId : PoundsUnitId;

        var result = new LogImpactResult
        {
            BagsCollected = bags,
            PickedWeight = weight,
            PickedWeightUnitId = weightUnitId,
            DurationMinutes = duration,
            Notes = NotesEditor.Text?.Trim()
        };

        var json = JsonSerializer.Serialize(result);
        await Shell.Current.ClosePopupAsync(json);
    }

    private async void OnCancelClicked(object? sender, EventArgs e)
    {
        await Shell.Current.ClosePopupAsync();
    }

    public class LogImpactResult
    {
        public int BagsCollected { get; set; }
        public decimal PickedWeight { get; set; }
        public int PickedWeightUnitId { get; set; }
        public int DurationMinutes { get; set; }
        public string? Notes { get; set; }
    }
}
