﻿namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;

public abstract partial class BaseViewModel : ObservableObject
{
    public BaseViewModel()
    {
    }

    [ObservableProperty]
    bool isBusy = false;

    [ObservableProperty]
    bool isError = false;

    public INavigation Navigation { get; set; }

    public Func<string, Task> Notify { get; set; }

    public Func<string, Task> NotifyError { get; set; }
}
