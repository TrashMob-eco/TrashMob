namespace TrashMobMobile.Controls;

using Maui.BindableProperty.Generator.Core;

public partial class TMTimePicker : ContentView
{
#pragma warning disable CS0169 // Field used by AutoBindable source generator
    [AutoBindable(OnChanged = nameof(TimePropertyChanged), DefaultBindingMode = nameof(BindingMode.TwoWay), HidesUnderlyingProperty = true)]
    private TimeSpan time;
#pragma warning restore CS0169

    public TMTimePicker()
    {
        InitializeComponent();
        WrappedTimePicker.PropertyChanged += WrappedTimePicker_PropertyChanged;
    }

    private void WrappedTimePicker_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(WrappedTimePicker.Time))
        {
            Time = WrappedTimePicker.Time ?? TimeSpan.Zero;
        }
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