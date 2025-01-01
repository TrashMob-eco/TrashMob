namespace TrashMobMobile.Pages.CreateEvent;

using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

public partial class Step5 : BaseStepClass
{
    public Step5()
    {
        InitializeComponent();
    }
    public override void OnNavigated()
    {
        base.OnNavigated();

        if (ViewModel?.EventViewModel?.Address?.Location != null)
        {
            var mapSpan = new MapSpan(ViewModel.EventViewModel.Address.Location, 0.01, 0.01);
            litterImagesMap.InitialMapSpanAndroid = mapSpan;
            litterImagesMap.MoveToRegion(mapSpan);
        }
    }

    private async void Pin_InfoWindowClicked(object sender, PinClickedEventArgs e)
    {
        var p = (Pin)sender;

        var litterReportId = p.AutomationId;
        await Shell.Current.GoToAsync($"{nameof(ViewLitterReportPage)}?LitterReportId={litterReportId}");
    }
}