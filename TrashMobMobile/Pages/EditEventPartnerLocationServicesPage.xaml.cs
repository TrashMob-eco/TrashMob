namespace TrashMobMobile.Pages;

using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

[QueryProperty(nameof(EventId), nameof(EventId))]
[QueryProperty(nameof(PartnerLocationId), nameof(PartnerLocationId))]
public partial class EditEventPartnerLocationServicesPage : ContentPage
{
    private readonly EditEventPartnerLocationServicesViewModel viewModel;

    public EditEventPartnerLocationServicesPage(EditEventPartnerLocationServicesViewModel viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        this.viewModel.Navigation = Navigation;
        BindingContext = this.viewModel;
    }

    public string EventId { get; set; }

    public string PartnerLocationId { get; set; }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await viewModel.Init(new Guid(EventId), new Guid(PartnerLocationId));
    }
}