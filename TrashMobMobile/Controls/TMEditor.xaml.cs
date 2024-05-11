using Maui.BindableProperty.Generator.Core;
namespace TrashMobMobile.Controls;

public partial class TMEditor : ContentView
{
	public TMEditor()
	{
		InitializeComponent();
	}

    [AutoBindable(DefaultBindingMode = "TwoWay", OnChanged = nameof(TextPropertyChanged))]
    private string _text;
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

    [AutoBindable(OnChanged = nameof(PlaceholderPropertyChanged))]
    private string placeholder;
    private void PlaceholderPropertyChanged(string newValue)
    {
        WrappedEditor.Placeholder = newValue;
    }
}

public class BorderlessEditor : Editor
{
    public BorderlessEditor()
    {
        Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("Borderless", (handler, view) =>
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
                handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
#endif
            }
        });
    }
}