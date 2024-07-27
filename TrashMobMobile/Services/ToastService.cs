namespace TrashMobMobile.Services;

using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

public interface IToastService
{
     Task Notify(string message, double fontSize = 14, ToastDuration toastDuration = ToastDuration.Short);
}

public class ToastService : IToastService
{
    public async Task Notify(string message, double fontSize = 14, ToastDuration toastDuration = ToastDuration.Short)
    {
        var toast = Toast.Make(message, toastDuration, fontSize);
        await toast.Show();
    }
}