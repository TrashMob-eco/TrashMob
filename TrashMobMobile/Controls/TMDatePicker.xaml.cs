namespace TrashMobMobile.Controls;

using Maui.BindableProperty.Generator.Core;

public partial class TMDatePicker : ContentView
{
#pragma warning disable CS0169 // Field used by AutoBindable source generator
    [AutoBindable(OnChanged = nameof(DateTimePropertyChanged), DefaultBindingMode = nameof(BindingMode.TwoWay), HidesUnderlyingProperty = true)]
    private DateTime date;
#pragma warning restore CS0169

    public TMDatePicker()
    {
        InitializeComponent();
        WrappedDatePicker.DateSelected += WrappedDatePicker_DateSelected;
    }

    private void WrappedDatePicker_DateSelected(object? sender, DateChangedEventArgs e)
    {
        Date = WrappedDatePicker.Date ?? default;
    }

    private void DateTimePropertyChanged(DateTime newValue)
    {
        WrappedDatePicker.Date = newValue;
    }
}

public class BorderlessDatePicker : DatePicker
{
    public BorderlessDatePicker()
    {
        Microsoft.Maui.Handlers.DatePickerHandler.Mapper.AppendToMapping(nameof(BorderlessPicker), (handler, view) =>
        {
            if (view is BorderlessDatePicker)
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