namespace TrashMobMobile.Pages;

using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using TrashMobMobile.Views.ViewEvent;

[QueryProperty(nameof(EventId), nameof(EventId))]
public partial class ViewEventPage : ContentPage
{
    private readonly ViewEventViewModel viewModel;

    public ViewEventPage(ViewEventViewModel viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        this.viewModel.Navigation = Navigation;

        BindingContext = this.viewModel;
    }

    public string EventId { get; set; }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await viewModel.Init(new Guid(EventId), UpdateRoutes);
        Switcher.SelectedIndex = 0;
    }

    private void UpdateRoutes()
    {
        //eventLocationMap.MapElements.Clear();

        //foreach (var route in viewModel.EventAttendeeRoutes)
        //{
        //    var polyline = new Polyline
        //    {
        //        StrokeColor = Color.FromArgb("c7d762"),
        //        StrokeWidth = 5,
        //    };

        //    foreach (var location in route.Locations)
        //    {
        //        polyline.Geopath.Add(new Location(location.Latitude, location.Longitude));
        //    }

        //    eventLocationMap.MapElements.Add(polyline);
        //}
    }
}