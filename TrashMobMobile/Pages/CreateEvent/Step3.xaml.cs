namespace TrashMobMobile.Pages.CreateEvent;

using Microsoft.Maui.Maps;

public partial class Step3 : BaseStepClass
{
    public Step3()
    {
        InitializeComponent();
    }

    public override void OnNavigated()
    {
        base.OnNavigated();

        if (ViewModel?.EventViewModel?.Address?.Location != null)
        {
            var mapSpan = new MapSpan(ViewModel.EventViewModel.Address.Location, 0.05, 0.05);
            eventLocationMap.InitialMapSpanAndroid = mapSpan;
            eventLocationMap.MoveToRegion(mapSpan);
        }
    }
}