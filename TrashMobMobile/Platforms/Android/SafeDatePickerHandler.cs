namespace TrashMobMobile.Platforms.Android;

using global::Android.App;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;

/// <summary>
/// Workaround for MAUI bug where DatePickerHandler.OnDialogDismiss fires after the
/// handler has been disconnected, causing "VirtualView cannot be null here"
/// (System.InvalidOperationException) crashes on Android.
///
/// We dismiss any open dialog during DisconnectHandler so the dismiss listener
/// cannot fire on a torn-down handler.
/// </summary>
internal class SafeDatePickerHandler : DatePickerHandler
{
    protected override void DisconnectHandler(MauiDatePicker platformView)
    {
        try
        {
            // Reflectively grab the private _dialog field MAUI uses internally
            // and dismiss it before the base disconnects the virtual view.
            var dialogField = typeof(DatePickerHandler).GetField(
                "_dialog",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

            if (dialogField?.GetValue(this) is DatePickerDialog dialog && dialog.IsShowing)
            {
                dialog.SetOnDismissListener(null);
                dialog.Dismiss();
            }
        }
        catch
        {
            // Best-effort: never let cleanup throw.
        }

        base.DisconnectHandler(platformView);
    }
}
