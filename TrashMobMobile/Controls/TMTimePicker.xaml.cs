namespace TrashMobMobile.Controls;

using Maui.BindableProperty.Generator.Core;

public partial class TMTimePicker : ContentView
{
    [AutoBindable(OnChanged = nameof(TimePropertyChanged))]
    private readonly TimeSpan time;

    public TMTimePicker()
    {
        InitializeComponent();
    }

    private void TimePropertyChanged(TimeSpan newValue)
    {
        WrappedTimePicker.Time = newValue;
    }   
}

public class BorderlessTimePicker : TimePicker
{
    public BorderlessTimePicker()
    {
        Microsoft.Maui.Handlers.TimePickerHandler.Mapper.AppendToMapping(nameof(BorderlessPicker), (handler, view) =>
        {
            if (view is BorderlessTimePicker)
            {
#if ANDROID
                handler.PlatformView.Background = null;
#elif IOS
                handler.PlatformView.BackgroundColor = UIKit.UIColor.Clear;
                handler.PlatformView.Layer.BorderWidth = 0;
                handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
#endif
            }
        });
    }
}