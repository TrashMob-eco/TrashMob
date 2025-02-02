namespace TrashMobMobile.Views.ViewEvent;

using Microsoft.Maui.Maps;

public partial class TabDetails : ContentView
{
	public TabDetails()
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

            eventLocationMap.InitialMapSpanAndroid = mapSpan;
            eventLocationMap.MoveToRegion(mapSpan);
        }
    }
}