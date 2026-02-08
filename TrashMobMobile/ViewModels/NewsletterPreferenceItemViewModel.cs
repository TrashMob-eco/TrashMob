namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;

public partial class NewsletterPreferenceItemViewModel : ObservableObject
{
    [ObservableProperty]
    private int categoryId;

    [ObservableProperty]
    private string categoryName = string.Empty;

    [ObservableProperty]
    private string categoryDescription = string.Empty;

    [ObservableProperty]
    private bool isSubscribed;
}
