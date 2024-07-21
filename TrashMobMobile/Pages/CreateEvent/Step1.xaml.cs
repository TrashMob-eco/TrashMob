using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrashMobMobile.Pages.CreateEvent;

public partial class Step1 : BaseStepClass
{
    public Step1()
    {
        InitializeComponent();
        
        // if (viewModel?.UserLocation?.Location != null)
        // {
        //     var mapSpan =
        //         new MapSpan(
        //             new Location(viewModel.UserLocation.Location.Latitude, viewModel.UserLocation.Location.Longitude),
        //             0.05, 0.05);
        //     eventLocationMap.MoveToRegion(mapSpan);
        // }
    }
}