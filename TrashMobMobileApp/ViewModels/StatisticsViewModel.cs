namespace TrashMobMobileApp.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;

public partial class StatisticsViewModel : ObservableObject
{
    private int totalEvents = 0;
    private int totalBags = 0;
    private int totalAttendees = 0;
    private int totalHours = 0;
    private int totalLitterReportSubmitted = 0;
    private int totalLitterReportClosed = 0;

    public StatisticsViewModel()
    {
    }

    [ObservableProperty]
    bool isBusy = false;

    [ObservableProperty]
    bool isError = false;

    public int TotalEvents
    {
        get => totalEvents;
        
        set {
            if (value != totalEvents)
            {
                totalEvents = value;
                OnPropertyChanged();
            }
        }
    }

    public int TotalBags
    {
        get => totalBags;

        set
        {
            if (value != totalBags)
            {
                totalBags = value;
                OnPropertyChanged();
            }
        }
    }

    public int TotalAttendees
    {
        get => totalAttendees;

        set
        {
            if (value != totalAttendees)
            {
                totalAttendees = value;
                OnPropertyChanged();
            }
        }
    }

    public int TotalHours
    {
        get => totalHours;

        set
        {
            if (value != totalHours)
            {
                totalHours = value;
                OnPropertyChanged();
            }
        }
    }

    public int TotalLitterReportSubmitted
    {
        get => totalLitterReportSubmitted;

        set
        {
            if (value != totalLitterReportSubmitted)
            {
                totalLitterReportSubmitted = value;
                OnPropertyChanged();
            }
        }
    }

    public int TotalLitterReportClosed
    {
        get => totalLitterReportClosed;

        set
        {
            if (value != totalLitterReportClosed)
            {
                totalLitterReportClosed = value;
                OnPropertyChanged();
            }
        }
    }
}
