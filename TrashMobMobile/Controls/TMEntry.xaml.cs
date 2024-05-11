using Maui.BindableProperty.Generator.Core;

namespace TrashMobMobile.Controls;

public partial class TMEntry : ContentView
{
	public TMEntry()
	{
		InitializeComponent();
	}

    [AutoBindable(DefaultBindingMode = "TwoWay", OnChanged = nameof(TextPropertyChanged))]
    private string _text;
    private void TextPropertyChanged(string newValue)
    {
        if (WrappedEntry.Text != newValue)
        {
            WrappedEntry.Text = newValue;
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
        WrappedEntry.Placeholder = newValue;
    }

    [AutoBindable(OnChanged = nameof(IsPasswordPropertyChanged))]
    private bool isPassword;
    private void IsPasswordPropertyChanged(bool newValue)
    {
        WrappedEntry.IsPassword = newValue;
    }

    [AutoBindable(OnChanged = nameof(HorizontalTextAlignmentPropertyChanged))]
    private TextAlignment horizontalTextAlignment;
    private void HorizontalTextAlignmentPropertyChanged(TextAlignment newValue)
    {
        WrappedEntry.HorizontalTextAlignment = newValue;
    }

    [AutoBindable(DefaultBindingMode = "OneWayToSource")]
    private bool _isValid;
}

public class BorderlessEntry : Entry
{
    public BorderlessEntry()
    {
        Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("Borderless", (handler, view) =>
        {
            if (view is BorderlessEntry)
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

        var transparentBackgroundSetter = new Setter
        {
            Property = BackgroundColorProperty,
            Value = Colors.Transparent
        };

        var focusedTrigger = new Trigger(typeof(Entry));
        focusedTrigger.Property = IsFocusedProperty;
        focusedTrigger.Value = true;
        focusedTrigger.Setters.Add(transparentBackgroundSetter);

        Triggers.Add(focusedTrigger);

    }
}