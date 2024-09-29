using System.Collections;

namespace TrashMobMobile.Controls;

using Maui.BindableProperty.Generator.Core;

public partial class TMDatePicker : ContentView
{
    [AutoBindable(OnChanged = nameof(DateTimePropertyChanged))]
    private readonly DateTime date;

    public TMDatePicker()
    {
        InitializeComponent();
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