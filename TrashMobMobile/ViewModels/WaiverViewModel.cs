namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMobMobile.Config;
using TrashMobMobile.Models;
using TrashMobMobile.Services;

public partial class WaiverViewModel(IWaiverManager waiverManager, INotificationService notificationService) : BaseViewModel(notificationService)
{
    private readonly IWaiverManager waiverManager = waiverManager;

    [ObservableProperty]
    private string name;

    [RelayCommand]
    private async Task SignWaiver()
    {
        IsBusy = true;

        try
        {
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
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            IsBusy = false;
            await NotificationService.NotifyError("An error occurred while opening the waiver page. Please try again.");
        }
    }
}