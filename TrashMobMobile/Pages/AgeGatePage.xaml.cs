namespace TrashMobMobile.Pages;

using TrashMobMobile.Authentication;
using TrashMobMobile.ViewModels;

public partial class AgeGatePage : ContentPage
{
    private readonly AgeGateViewModel viewModel;
    private readonly IAuthService authService;

    public AgeGatePage(AgeGateViewModel viewModel, IAuthService authService)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        this.authService = authService;
        BindingContext = viewModel;

        viewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    private async void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(AgeGateViewModel.IsAgeVerified) && viewModel.IsAgeVerified)
        {
            viewModel.IsAgeVerified = false;

            var result = await authService.SignUpInteractiveAsync();

            if (result.Succeeded)
            {
                await Task.Delay(100);
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await Shell.Current.GoToAsync($"//{nameof(MainTabsPage)}");
                });
            }
            else
            {
                await Shell.Current.GoToAsync("..");
            }
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        viewModel.PropertyChanged -= OnViewModelPropertyChanged;
    }
}
