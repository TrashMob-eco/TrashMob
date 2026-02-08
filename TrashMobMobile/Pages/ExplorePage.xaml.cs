namespace TrashMobMobile.Pages;

using Microsoft.Maui.Controls.Maps;

public partial class ExplorePage : ContentPage
{
    private readonly ExploreViewModel viewModel;

    public ExplorePage(ExploreViewModel viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        BindingContext = this.viewModel;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await viewModel.Init();
    }

    private async void Pin_InfoWindowClicked(object? sender, PinClickedEventArgs e)
    {
        if (sender is not Pin p)
        {
            return;
        }

        var automationId = p.AutomationId;
        if (string.IsNullOrEmpty(automationId))
        {
            return;
        }

        var parts = automationId.Split(':');
        if (parts.Length < 2)
        {
            return;
        }

        var type = parts[0];
        var id = parts[1];

        if (type == "Event")
        {
            await Shell.Current.GoToAsync($"{nameof(ViewEventPage)}?EventId={id}");
        }
        else if (type == "LitterImage")
        {
            await Shell.Current.GoToAsync($"{nameof(ViewLitterReportPage)}?LitterReportId={id}");
        }
    }
}
