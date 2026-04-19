namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMob.Models.Poco.V2;
using TrashMobMobile.Services;

public partial class ChildSignupViewModel(
    IPrivoConsentRestService privoConsentRestService,
    INotificationService notificationService)
    : BaseViewModel(notificationService)
{
    [ObservableProperty]
    private string parentEmail = string.Empty;

    [ObservableProperty]
    private string childFirstName = string.Empty;

    [ObservableProperty]
    private string childEmail = string.Empty;

    [ObservableProperty]
    private DateTime dateOfBirth = DateTime.Today.AddYears(-15);

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFormVisible))]
    private bool isParentNotFound;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFormVisible))]
    private bool isParentNotVerified;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFormVisible))]
    private bool isConsentPending;

    [ObservableProperty]
    private string resultParentEmail = string.Empty;

    public bool IsFormVisible => !IsParentNotFound && !IsParentNotVerified && !IsConsentPending;

    public void Init(string dateOfBirthString)
    {
        if (DateTime.TryParse(dateOfBirthString, out var dob))
        {
            DateOfBirth = dob;
        }

        // Reset state
        IsParentNotFound = false;
        IsParentNotVerified = false;
        IsConsentPending = false;
        ParentEmail = string.Empty;
        ChildFirstName = string.Empty;
        ChildEmail = string.Empty;
    }

    [RelayCommand]
    private async Task SubmitConsentRequest()
    {
        if (string.IsNullOrWhiteSpace(ParentEmail) || !ParentEmail.Contains('@'))
        {
            await NotificationService.NotifyError("Please enter a valid parent/guardian email address.");
            return;
        }

        if (string.IsNullOrWhiteSpace(ChildFirstName))
        {
            await NotificationService.NotifyError("Please enter your first name.");
            return;
        }

        if (string.IsNullOrWhiteSpace(ChildEmail) || !ChildEmail.Contains('@'))
        {
            await NotificationService.NotifyError("Please enter a valid email address.");
            return;
        }

        ResultParentEmail = ParentEmail;

        try
        {
            IsBusy = true;

            var request = new InitiateChildConsentRequest
            {
                ParentEmail = ParentEmail,
                ChildFirstName = ChildFirstName,
                ChildEmail = ChildEmail,
                ChildBirthDate = DateOnly.FromDateTime(DateOfBirth),
            };

            var result = await privoConsentRestService.InitiateChildInitiatedConsentAsync(request);

            if (result == null)
            {
                IsParentNotFound = true;
            }
            else
            {
                IsConsentPending = true;
            }
        }
        catch (InvalidOperationException ex) when (ex.Message == "PARENT_NOT_VERIFIED")
        {
            IsParentNotVerified = true;
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            await NotificationService.NotifyError("Unable to submit consent request. Please try again.");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void TryDifferentEmail()
    {
        IsParentNotFound = false;
        IsParentNotVerified = false;
        ParentEmail = string.Empty;
    }

    [RelayCommand]
    private async Task GoBack()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task GoHome()
    {
        await Shell.Current.GoToAsync("../..");
    }
}
