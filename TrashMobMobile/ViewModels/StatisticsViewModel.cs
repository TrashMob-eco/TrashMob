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
    private int totalLitterReportClosed;

    [ObservableProperty]
    private int totalLitterReportSubmitted;
}