namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using TrashMob.Models;

public partial class EventSummaryViewModel : ObservableObject
{
    [ObservableProperty]
    private int actualNumberOfAttendees;

    [ObservableProperty]
    private int durationInMinutes;

    [ObservableProperty]
    private Guid eventId;

    [ObservableProperty]
    private string notes = string.Empty;

    [ObservableProperty]
    private int numberOfBags;

    [ObservableProperty]
    private int numberOfBuckets;

    [ObservableProperty]
    private decimal pickedWeight;

    [ObservableProperty]
    private int pickedWeightUnitId;

    /// <summary>
    /// Gets the display string for weight with unit (e.g., "25 lbs" or "12 kg"), rounded to nearest whole number.
    /// </summary>
    public string WeightDisplay
    {
        get
        {
            if (PickedWeight <= 0)
            {
                return "N/A";
            }

            var unit = PickedWeightUnitId == (int)WeightUnitEnum.Kilogram ? "kg" : "lbs";
            return $"{Math.Round(PickedWeight)} {unit}";
        }
    }
}