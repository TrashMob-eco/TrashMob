using System.Windows.Input;
using TrashMobMobile.Pages.CreateEvent;

namespace TrashMobMobile.Pages;

using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

public partial class CreateEventPageNew : ContentPage
{
    private readonly CreateEventViewModelNew viewModel;

    public CreateEventPageNew(CreateEventViewModelNew viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        this.viewModel.Notify = Notify;
        this.viewModel.NotifyError = NotifyError;
        this.viewModel.Navigation = Navigation;

        viewModel.Steps = new IContentView[]
        {
            new Step1(),
            new Step2(),
            new Step3(),
            new Step4(),
            new Step5(),
            new Step6()
        };

        BindingContext = this.viewModel;
    }
    

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await viewModel.Init();
    }

// TODO: These can be moved to a service/abstraction
    private async Task Notify(string message)
    {
        var cancellationTokenSource = new CancellationTokenSource();

        var duration = ToastDuration.Short;
        double fontSize = 14;

        var toast = Toast.Make(message, duration, fontSize);
        await toast.Show(cancellationTokenSource.Token);
    }

    private async Task NotifyError(string message)
    {
        var cancellationTokenSource = new CancellationTokenSource();

        var snackbarOptions = new SnackbarOptions
        {
            BackgroundColor = Colors.Red,
            TextColor = Colors.White,
            CornerRadius = new CornerRadius(10),
            Font = Microsoft.Maui.Font.SystemFontOfSize(14),
        };

        var text = message;
        var duration = TimeSpan.FromSeconds(3);

        var snackbar = Snackbar.Make(text, duration: duration, visualOptions: snackbarOptions);

        await snackbar.Show(cancellationTokenSource.Token);
    }
}