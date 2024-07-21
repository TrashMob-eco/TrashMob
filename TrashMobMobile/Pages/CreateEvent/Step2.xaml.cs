using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

namespace TrashMobMobile.Pages.CreateEvent;

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