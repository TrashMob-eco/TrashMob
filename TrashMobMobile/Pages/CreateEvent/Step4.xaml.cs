namespace TrashMobMobile.Pages.CreateEvent;

using Microsoft.Maui.Maps;

public partial class Step4 : BaseStepClass
{
    public Step4()
    {
        InitializeComponent();
    }

    public override void OnNavigated()
    {
        base.OnNavigated();

        if (ViewModel?.EventViewModel?.Address?.Location != null)
        {
            var mapSpan = new MapSpan(ViewModel.EventViewModel.Address.Location, 0.05, 0.05);
            eventLocationMap.MoveToRegion(mapSpan);
        }
    }
}