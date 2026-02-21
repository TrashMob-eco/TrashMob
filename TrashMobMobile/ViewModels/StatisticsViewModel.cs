namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;

public partial class StatisticsViewModel : ObservableObject
{
    [ObservableProperty]
    private int totalAttendees;

    [ObservableProperty]
    private int totalBags;

    [ObservableProperty]
    private int totalEvents;

    [ObservableProperty]
    private int totalHours;

    [ObservableProperty]
    private int totalLitterReportsClosed;

    [ObservableProperty]
    private int totalLitterReportsSubmitted;

    [ObservableProperty]
    private decimal totalWeightInPounds;

    public string TotalWeightDisplay => TotalWeightInPounds > 0
        ? $"{TotalWeightInPounds:N0} lbs"
        : "0 lbs";

    partial void OnTotalWeightInPoundsChanged(decimal value)
    {
        OnPropertyChanged(nameof(TotalWeightDisplay));
    }
}
