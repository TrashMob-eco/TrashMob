namespace TrashMobMobile.Controls;

using Maui.BindableProperty.Generator.Core;

public partial class TMEntry : ContentView
{
    [AutoBindable(OnChanged = nameof(OnKeyboardChanged))]
    private readonly Keyboard _keyboard;

    [AutoBindable(DefaultBindingMode = "TwoWay", OnChanged = nameof(TextPropertyChanged))]
    private readonly string _text;

    [AutoBindable(OnChanged = nameof(HorizontalTextAlignmentPropertyChanged))]
    private readonly TextAlignment horizontalTextAlignment;

    [AutoBindable(OnChanged = nameof(IsPasswordPropertyChanged))]
    private readonly bool isPassword;

    [AutoBindable(OnChanged = nameof(PlaceholderPropertyChanged))]
    private readonly string placeholder;

    public TMEntry()
    {
        InitializeComponent();
    }

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

    private void PlaceholderPropertyChanged(string newValue)
    {
        WrappedEntry.Placeholder = newValue;
    }

    private void IsPasswordPropertyChanged(bool newValue)
    {
        WrappedEntry.IsPassword = newValue;
    }

    private void HorizontalTextAlignmentPropertyChanged(TextAlignment newValue)
    {
        WrappedEntry.HorizontalTextAlignment = newValue;
    }

    private void OnKeyboardChanged(Keyboard kb)
    {
        WrappedEntry.Keyboard = kb;
    }
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
            Value = Colors.Transparent,
        };

        var focusedTrigger = new Trigger(typeof(Entry));
        focusedTrigger.Property = IsFocusedProperty;
        focusedTrigger.Value = true;
        focusedTrigger.Setters.Add(transparentBackgroundSetter);

        Triggers.Add(focusedTrigger);
    }
}