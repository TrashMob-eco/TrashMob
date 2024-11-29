namespace TrashMobMobile;

using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics.Drawables;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using Microsoft.Maui.Maps.Handlers;
using Microsoft.Maui.Platform;
using System.ComponentModel;
using TrashMobMobile.Controls;
using IMap = Microsoft.Maui.Maps.IMap;

public class CustomMapHandler : MapHandler
{
    private bool _setInitialMapSpan;

    public static readonly IPropertyMapper<IMap, IMapHandler> CustomMapper =
        new PropertyMapper<IMap, IMapHandler>(Mapper)
        {
            [nameof(IMap.Pins)] = MapPins,
        };

    public CustomMapHandler() : base(CustomMapper, CommandMapper)
    {
    }

    public CustomMapHandler(IPropertyMapper? mapper = null, CommandMapper? commandMapper = null) : base(
        mapper ?? CustomMapper, commandMapper ?? CommandMapper)
    {
    }

    public List<Marker> Markers { get; } = new();

    protected override void ConnectHandler(MapView platformView)
    {
        base.ConnectHandler(platformView);
        (VirtualView as Map).PropertyChanged += OnPropertyChanged;
        var mapReady = new MapCallbackHandler(this);
        PlatformView.GetMapAsync(mapReady);
    }

    protected override void DisconnectHandler(MapView platformView)
    {
        base.DisconnectHandler(platformView);
        (VirtualView as Map).PropertyChanged -= OnPropertyChanged;
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        // when map is ready, visible region is initially set by base handler
        if (e.PropertyName == "VisibleRegion")
        {
            UpdateInitialMapSpan();
        }
    }

    private void UpdateInitialMapSpan()
    {
        if (_setInitialMapSpan)
        {
            return;
        }

        if (Map == null || Map.Projection.VisibleRegion.LatLngBounds.Center.Latitude == 0)
        {
            return;
        }

        if (VirtualView is not CustomMap map)
        {
            return;
        }

        var span = map.InitialMapSpanAndroid;
        var ne = new LatLng(span.Center.Latitude + span.LatitudeDegrees / 2,
            span.Center.Longitude + span.LongitudeDegrees / 2);
        var sw = new LatLng(span.Center.Latitude - span.LatitudeDegrees / 2,
            span.Center.Longitude - span.LongitudeDegrees / 2);
        var update = CameraUpdateFactory.NewLatLngBounds(new LatLngBounds(sw, ne), 0);

        try
        {
            Map.MoveCamera(update);
            _setInitialMapSpan = true;
        }
        catch (Exception)
        {
            // ignored
        }
    }

    private new static void MapPins(IMapHandler handler, IMap map)
    {
        if (handler is CustomMapHandler mapHandler)
        {
            foreach (var marker in mapHandler.Markers)
            {
                marker.Remove();
            }

            mapHandler.AddPins(map.Pins);
        }
    }

    private void AddPins(IEnumerable<IMapPin> mapPins)
    {
        if (Map is null || MauiContext is null)
        {
            return;
        }

        foreach (var pin in mapPins)
        {
            var pinHandler = pin.ToHandler(MauiContext);
            if (pinHandler is IMapPinHandler mapPinHandler)
            {
                var markerOption = mapPinHandler.PlatformView;
                if (pin is CustomPin cp)
                {
                    cp.ImageSource.LoadImage(MauiContext, result =>
                    {
                        if (result?.Value is BitmapDrawable bitmapDrawable)
                        {
                            markerOption.SetIcon(BitmapDescriptorFactory.FromBitmap(bitmapDrawable.Bitmap));
                        }

                        AddMarker(Map, pin, Markers, markerOption);
                    });
                }
                else
                {
                    AddMarker(Map, pin, Markers, markerOption);
                }
            }
        }
    }

    private static void AddMarker(GoogleMap map, IMapPin pin, List<Marker> markers, MarkerOptions markerOption)
    {
        var marker = map.AddMarker(markerOption);
        pin.MarkerId = marker.Id;
        markers.Add(marker);
    }
    
    class MapCallbackHandler : Java.Lang.Object, IOnMapReadyCallback
    {
        private readonly IMapHandler mapHandler;
        public MapCallbackHandler(IMapHandler mapHandler)
        {
            this.mapHandler = mapHandler;
        }
        public void OnMapReady(GoogleMap googleMap)
        {
            mapHandler.UpdateValue(nameof(IMap.Pins));
        }
    }
}