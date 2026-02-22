namespace TrashMobMobile.Controls;

using System.Globalization;
using CommunityToolkit.Maui.Extensions;
using Newtonsoft.Json;
using TrashMob.Models;

public partial class LogPickupPopup : ContentView
{
    private int bags;

    public LogPickupPopup(int? currentBags = null, decimal? currentWeight = null,
        int? currentWeightUnitId = null, string? currentNotes = null)
    {
        InitializeComponent();

        bags = currentBags ?? 0;
        bagsLabel.Text = bags.ToString();

        if (currentWeight.HasValue)
        {
            weightEntry.Text = currentWeight.Value.ToString("F1");
        }

        unitPicker.SelectedIndex = currentWeightUnitId == (int)WeightUnitEnum.Kilogram ? 1 : 0;

        if (!string.IsNullOrWhiteSpace(currentNotes))
        {
            notesEntry.Text = currentNotes;
        }
    }

    private void OnBagsIncrement(object? sender, EventArgs e)
    {
        if (bags < 99)
        {
            bags++;
            bagsLabel.Text = bags.ToString();
        }
    }

    private void OnBagsDecrement(object? sender, EventArgs e)
    {
        if (bags > 0)
        {
            bags--;
            bagsLabel.Text = bags.ToString();
        }
    }

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        decimal? weight = null;
        if (!string.IsNullOrWhiteSpace(weightEntry.Text) &&
            decimal.TryParse(weightEntry.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out var parsedWeight))
        {
            weight = parsedWeight;
        }

        var weightUnitId = unitPicker.SelectedIndex == 1
            ? (int)WeightUnitEnum.Kilogram
            : (int)WeightUnitEnum.Pound;

        var result = new LogPickupResult
        {
            BagsCollected = bags > 0 ? bags : null,
            WeightCollected = weight,
            WeightUnitId = weight.HasValue ? weightUnitId : null,
            Notes = string.IsNullOrWhiteSpace(notesEntry.Text) ? null : notesEntry.Text.Trim(),
        };

        await Shell.Current.ClosePopupAsync(JsonConvert.SerializeObject(result));
    }

    private async void OnCancelClicked(object? sender, EventArgs e)
    {
        await Shell.Current.ClosePopupAsync();
    }
}

public class LogPickupResult
{
    public int? BagsCollected { get; set; }
    public decimal? WeightCollected { get; set; }
    public int? WeightUnitId { get; set; }
    public string? Notes { get; set; }
}
