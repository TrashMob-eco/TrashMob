namespace TrashMobMobile.Controls;

using Maui.BindableProperty.Generator.Core;

public partial class TMEditor : ContentView
{
    [AutoBindable(DefaultBindingMode = "TwoWay", OnChanged = nameof(TextPropertyChanged))]
    private readonly string text;

    [AutoBindable(OnChanged = nameof(PlaceholderPropertyChanged))]
    private readonly string placeholder;

    public TMEditor()
    {
        InitializeComponent();
    }

    private void TextPropertyChanged(string newValue)
    {
        if (WrappedEditor.Text != newValue)
        {
            WrappedEditor.Text = newValue;
        }
    }

    private void Entry_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        if (Text != e.NewTextValue)
        {
            Text = e.NewTextValue;
        }
    }

    private void PlaceholderPropertyChanged(string newValue)
    {
        WrappedEditor.Placeholder = newValue;
    }
}

public class BorderlessEditor : Editor
{
    public BorderlessEditor()
    {
        Microsoft.Maui.Handlers.EditorHandler.Mapper.AppendToMapping(nameof(BorderlessEditor), (handler, view) =>
        {
            if (view is BorderlessEditor)
            {
#if ANDROID
                handler.PlatformView.Background = null;
                handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
                handler.PlatformView.BackgroundTintList =
                    Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
#elif IOS
                handler.PlatformView.BackgroundColor = UIKit.UIColor.Clear;
                handler.PlatformView.Layer.BorderWidth = 0;
#endif
            }
        });
    }
}