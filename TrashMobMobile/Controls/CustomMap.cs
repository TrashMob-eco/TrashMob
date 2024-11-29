namespace TrashMobMobile.Controls;

using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

public class CustomMap : Map
{
    public CustomMap() : base() { }

    public CustomMap(MapSpan initialMapSpan) : base(initialMapSpan) { }

    #region Dependency/Bindable Properties

    public static readonly BindableProperty InitialMapSpanAndroidProperty = BindableProperty.Create(
        "InitialMapSpanAndroid", typeof(MapSpan), typeof(CustomMap));

    #endregion

    #region Properties

    // map centering does not work on Android if not yet loaded, thus use custom handler
    // https://github.com/dotnet/maui/issues/12342
    public MapSpan InitialMapSpanAndroid
    {
        get => (MapSpan)GetValue(InitialMapSpanAndroidProperty);
        set => SetValue(InitialMapSpanAndroidProperty, value);
    }

    #endregion
}