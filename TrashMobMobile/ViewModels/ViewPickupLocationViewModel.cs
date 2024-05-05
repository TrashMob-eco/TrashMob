namespace TrashMobMobile.ViewModels;

using TrashMobMobile.Data;

public partial class ViewPickupLocationViewModel : BaseViewModel
{
    private readonly IPickupLocationManager pickupLocationManager;

    public ViewPickupLocationViewModel(IPickupLocationManager pickupLocationManager)
    {
        this.pickupLocationManager = pickupLocationManager;
    }

    public async Task Init(Guid pickupLocationId)
    {
        IsBusy = true;

        IsBusy = false;
    }
}
