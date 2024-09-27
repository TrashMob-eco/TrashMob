using System.Collections;

namespace TrashMobMobile.Controls;

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
#elif!IOS
                handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
#endif
            }
        });
    }
}