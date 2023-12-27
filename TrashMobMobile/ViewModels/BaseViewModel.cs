namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;

public abstract partial class BaseViewModel : ObservableObject
{
    public BaseViewModel()
    {
    }

    [ObservableProperty]
    bool isBusy = false;

    [ObservableProperty]
    bool isError = false;

    [ObservableProperty]
    bool isValid = false;

    [ObservableProperty]
    bool isErrorMessageVisible = false;

    [ObservableProperty]
    string errorMessage = string.Empty;

    public INavigation Navigation { get; set; }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        Validate();
    }

    protected virtual void Validate()
    {
        IsValid = true;
        ErrorMessage = string.Empty;
        IsErrorMessageVisible = false;
    }
}
