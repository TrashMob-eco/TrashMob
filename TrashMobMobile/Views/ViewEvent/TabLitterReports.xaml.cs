namespace TrashMobMobile.Views.ViewEvent;

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

        if (BindingContext is ViewEventViewModel viewModel)
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
}