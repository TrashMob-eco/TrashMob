namespace TrashMobMobile.Pages;

using Microsoft.Maui.Maps;

[QueryProperty(nameof(LitterReportId), nameof(LitterReportId))]
public partial class EditLitterReportPage : ContentPage
{
    private readonly EditLitterReportViewModel viewModel;

    public EditLitterReportPage(EditLitterReportViewModel viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        this.viewModel.Navigation = Navigation;
        BindingContext = this.viewModel;
    }

    public string LitterReportId { get; set; } = string.Empty;

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await viewModel.Init(new Guid(LitterReportId));

        if (viewModel?.LitterImageViewModels?.FirstOrDefault()?.Address?.Location != null)
        {
            var mapSpan = new MapSpan(viewModel!.LitterImageViewModels!.First().Address.Location!, 0.05,
                0.05);
            litterReportLocationMap.InitialMapSpanAndroid = mapSpan;
            litterReportLocationMap.MoveToRegion(mapSpan);
        }
    }
}
