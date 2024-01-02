namespace TrashMobMobile.Pages;

using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

public partial class ContactUsPage : ContentPage
{
    private readonly ContactUsViewModel _viewModel;

    public ContactUsPage(ContactUsViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        _viewModel.Navigation = Navigation;
        _viewModel.Notify = Notify;
        BindingContext = _viewModel;
    }

    private async Task Notify(string message)
    {
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        ToastDuration duration = ToastDuration.Short;
        double fontSize = 14;

        var toast = Toast.Make(message, duration, fontSize);
        await toast.Show(cancellationTokenSource.Token);
    }
}