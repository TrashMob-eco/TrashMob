namespace TrashMobMobile.Pages.CreateEvent;

using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

public partial class Step2 : BaseStepClass
{
    public Step2()
    {
        InitializeComponent();

        NavigatedEvent += (sender, args) =>
        {
            if (ViewModel?.UserLocation?.Location != null)
            {
                var mapSpan =
                    new MapSpan(
                        new Location(ViewModel.UserLocation.Location.Latitude, ViewModel.UserLocation.Location.Longitude),
                        0.05, 0.05);
                eventLocationMap.MoveToRegion(mapSpan);
            }
        };
    }
    
    private async void OnMapClicked(object sender, MapClickedEventArgs e)
    {
        await ViewModel.ChangeLocation(e.Location);
    }
}