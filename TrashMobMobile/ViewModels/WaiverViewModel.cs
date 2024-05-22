namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMobMobile.Config;
using TrashMobMobile.Data;
using TrashMobMobile.Models;

public partial class WaiverViewModel : BaseViewModel
{
    public WaiverViewModel(IWaiverManager waiverManager)
    {
        this.waiverManager = waiverManager;
    }

    [ObservableProperty]
    string name;
    private readonly IWaiverManager waiverManager;

    [RelayCommand]
    private async Task SignWaiver()
    {
        IsBusy = true;
        
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
