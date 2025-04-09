namespace TrashMobMobile.Controls;

using System.Collections;
using Maui.BindableProperty.Generator.Core;

public partial class TMPicker : ContentView
{
    [AutoBindable(OnChanged = nameof(TitlePropertyChanged))]
    private readonly string title;
    
    [AutoBindable(OnChanged = nameof(ItemsSourcePropertyChanged))]
    private readonly IList itemsSource;

    [AutoBindable(OnChanged = nameof(SelectedItemPropertyChanged))]
    private readonly object selectedItem;
    

    public TMPicker()
    {
        InitializeComponent();
    }

    private void SelectedItemPropertyChanged(object newValue)
    {
        WrappedPicker.SelectedItem = newValue;
    }   
        
    private void ItemsSourcePropertyChanged(IList newValue)
    {
        WrappedPicker.ItemsSource = newValue;
    }    

    private void TitlePropertyChanged(string newValue)
    {
        WrappedPicker.Title = newValue;
    }

    private void WrappedPicker_OnSelectedIndexChanged(object? sender, EventArgs e)
    {
        SelectedItem = ((BorderlessPicker) sender).SelectedItem;
    }
}

public class BorderlessPicker : Picker
{
    public BorderlessPicker()
    {
        Microsoft.Maui.Handlers.PickerHandler.Mapper.AppendToMapping(nameof(BorderlessPicker), (handler, view) =>
        {
            if (view is BorderlessPicker)
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