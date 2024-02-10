namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;

public partial class StatisticsViewModel : ObservableObject
{
    public StatisticsViewModel()
    {
    }

    [ObservableProperty]
    private int totalEvents = 0;

    [ObservableProperty]
    private int totalBags = 0;

    [ObservableProperty]
    private int totalAttendees = 0;

    [ObservableProperty]
    private int totalHours = 0;

    [ObservableProperty]
    private int totalLitterReportSubmitted = 0;

    [ObservableProperty]
    private int totalLitterReportClosed = 0;
}
