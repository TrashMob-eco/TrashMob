using System;
using System.Collections.Generic;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Widget;
using TrashMobMobile.Controls;
using TrashMobMobile.Droid;
using TrashMobMobile.Views;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.Android;

[assembly: ExportRenderer(typeof(EventMap), typeof(EventMapRenderer))]
namespace TrashMobMobile.Droid
{
    public class EventMapRenderer : MapRenderer, GoogleMap.IInfoWindowAdapter
    {
        List<EventPin> eventPins;

        public EventMapRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Map> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                NativeMap.InfoWindowClick -= OnInfoWindowClick;
            }

            if (e.NewElement != null)
            {
                var formsMap = (EventMap)e.NewElement;
                eventPins = formsMap.EventPins;
            }
        }

        protected override void OnMapReady(GoogleMap map)
        {
            base.OnMapReady(map);

            NativeMap.InfoWindowClick += OnInfoWindowClick;
            NativeMap.SetInfoWindowAdapter(this);
        }

        protected override MarkerOptions CreateMarker(Pin pin)
        {
            var marker = new MarkerOptions();
            marker.SetPosition(new LatLng(pin.Position.Latitude, pin.Position.Longitude));
            marker.SetTitle(pin.Label);
            marker.SetSnippet(pin.Address);

            marker.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.pin));
            return marker;
        }

        void OnInfoWindowClick(object sender, GoogleMap.InfoWindowClickEventArgs e)
        {
            var eventPin = GetEventPin(e.Marker);
            if (eventPin == null)
            {
                throw new Exception("Event pin not found");
            }

            // Todo: Figure out how to redirect back to the Event Details / Event Edit page from Android
            if (!string.IsNullOrWhiteSpace(eventPin.Url))
            {
                var url = Android.Net.Uri.Parse(eventPin.Url);
                var intent = new Intent(Intent.ActionView, url);
                intent.AddFlags(ActivityFlags.NewTask);
                Android.App.Application.Context.StartActivity(intent);
            }
        }

        public Android.Views.View GetInfoContents(Marker marker)
        {
            var inflater = Android.App.Application.Context.GetSystemService(Context.LayoutInflaterService) as Android.Views.LayoutInflater;

            if (inflater != null)
            {
                var eventPin = GetEventPin(marker);
                if (eventPin == null)
                {
                    throw new Exception("Event pin not found");
                }

                Android.Views.View view = inflater.Inflate(Resource.Layout.MapInfoWindow, null);

                var infoTitle = view.FindViewById<TextView>(Resource.Id.EventName);
                var infoSubtitle = view.FindViewById<TextView>(Resource.Id.EventDate);
                var infoAddress = view.FindViewById<TextView>(Resource.Id.EventAddress);

                if (infoTitle != null)
                {
                    infoTitle.Text = eventPin.Name;
                }

                if (infoSubtitle != null)
                {
                    infoSubtitle.Text = eventPin.EventDate.ToString("MMMM dd, yyyy H:mm tt");
                }

                if (infoAddress != null)
                {
                    infoAddress.Text = eventPin.Address;
                }

                return view;
            }

            return null;
        }

        public Android.Views.View GetInfoWindow(Marker marker)
        {
            return null;
        }

        EventPin GetEventPin(Marker annotation)
        {
            var position = new Position(annotation.Position.Latitude, annotation.Position.Longitude);
            foreach (var pin in eventPins)
            {
                if (pin.Position == position)
                {
                    return pin;
                }
            }

            return null;
        }
    }
}

