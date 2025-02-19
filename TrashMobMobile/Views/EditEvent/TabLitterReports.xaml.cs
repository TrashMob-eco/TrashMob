namespace TrashMobMobile.Views.EditEvent;

using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

public partial class TabLitterReports : ContentView
{
	public TabLitterReports()
	{
		InitializeComponent();
	}

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();

        if (BindingContext is EditEventViewModel viewModel)
        {
            MapSpan mapSpan = new MapSpan(new Location(0, 0), 0.05, 0.05);

            if (viewModel?.EventViewModel?.Address?.Location != null)
            {
                mapSpan = new MapSpan(viewModel.EventViewModel.Address.Location, 0.05, 0.05);
            }

            litterImagesMap.InitialMapSpanAndroid = mapSpan;
            litterImagesMap.MoveToRegion(mapSpan);
        }
    }

    private async void Pin_InfoWindowClicked(object sender, PinClickedEventArgs e)
    {
        var p = (Pin)sender;
        var litterReportId = new Guid(p.AutomationId);

        if (BindingContext is EditEventViewModel viewModel)
        {
            await viewModel.UpdateLitterAssignment(litterReportId);
        }
    }
}