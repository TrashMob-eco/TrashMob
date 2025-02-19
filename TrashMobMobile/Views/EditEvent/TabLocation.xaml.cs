namespace TrashMobMobile.Views.EditEvent;

using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

public partial class TabLocation : ContentView
{
	public TabLocation()
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

            eventLocationMap.InitialMapSpanAndroid = mapSpan;
            eventLocationMap.MoveToRegion(mapSpan);
        }
    }

    private async void OnMapClicked(object sender, MapClickedEventArgs e)
    {
        if (BindingContext is EditEventViewModel viewModel)
        {
            await viewModel.ChangeLocation(e.Location);
        }
    }
}