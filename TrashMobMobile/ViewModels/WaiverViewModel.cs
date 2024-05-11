namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Input;
using TrashMobMobile.Config;
using TrashMobMobile.Data;
using TrashMobMobile.Models;

public partial class WaiverViewModel : BaseViewModel
{
    public WaiverViewModel(IWaiverManager waiverManager)
    {
        SignWaiverCommand = new Command(async () => await SignWaiver());
        this.waiverManager = waiverManager;
    }

    [ObservableProperty]
    string name;
    private readonly IWaiverManager waiverManager;

    [ObservableProperty]
    double overlayOpacity;

    public ICommand SignWaiverCommand { get; set; } 

    private async Task SignWaiver()
    {
        IsBusy = true;
        OverlayOpacity = 0.25; // Workaround for: https://github.com/dotnet/maui/issues/18234

        var envelopeRequest = new EnvelopeRequest();
        envelopeRequest.SignerEmail = App.CurrentUser.Email;
        envelopeRequest.CreatedByUserId = App.CurrentUser.Id;
        envelopeRequest.SignerName = Name;
        envelopeRequest.ReturnUrl = $"{Settings.SiteBaseUrl}/waiversreturn";

        var response = await waiverManager.GetWaiverEnvelopeAsync(envelopeRequest);

        var uri = new Uri(response.RedirectUrl);
        await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);

        await Navigation.PopAsync();

        IsBusy = false;
    }
}
