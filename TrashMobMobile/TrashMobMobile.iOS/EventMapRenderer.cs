using System;
using System.Collections.Generic;
using CoreGraphics;
using MapKit;
using TrashMobMobile.Controls;
using TrashMobMobile.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.iOS;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(EventMap), typeof(EventMapRenderer))]
namespace TrashMobMobile.iOS
{
    public class EventMapRenderer : MapRenderer
    {
        UIView eventPinView;
        List<EventPin> eventPins;

        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                var nativeMap = Control as MKMapView;
                nativeMap.GetViewForAnnotation = null;
                nativeMap.CalloutAccessoryControlTapped -= OnCalloutAccessoryControlTapped;
                nativeMap.DidSelectAnnotationView -= OnDidSelectAnnotationView;
                nativeMap.DidDeselectAnnotationView -= OnDidDeselectAnnotationView;
            }

            if (e.NewElement != null)
            {
                var formsMap = (EventMap)e.NewElement;
                var nativeMap = Control as MKMapView;
                eventPins = formsMap.EventPins;

                nativeMap.GetViewForAnnotation = GetViewForAnnotation;
                nativeMap.CalloutAccessoryControlTapped += OnCalloutAccessoryControlTapped;
                nativeMap.DidSelectAnnotationView += OnDidSelectAnnotationView;
                nativeMap.DidDeselectAnnotationView += OnDidDeselectAnnotationView;
            }
        }

        protected override MKAnnotationView GetViewForAnnotation(MKMapView mapView, IMKAnnotation annotation)
        {
            if (annotation is MKUserLocation)
            {
                return null;
            }

            var eventPin = GetEventPin(annotation as MKPointAnnotation);
            if (eventPin == null)
            {
                throw new Exception("Custom pin not found");
            }

            MKAnnotationView annotationView = mapView.DequeueReusableAnnotation(eventPin.Name);
            if (annotationView == null)
            {
                annotationView = new EventMapMKAnnotationView(annotation, eventPin.Name)
                {
                    Image = UIImage.FromFile("pin.png"),
                    CalloutOffset = new CGPoint(0, 0),
                    LeftCalloutAccessoryView = new UIImageView(UIImage.FromFile("monkey.png")),
                    RightCalloutAccessoryView = UIButton.FromType(UIButtonType.DetailDisclosure)
                };
                ((EventMapMKAnnotationView)annotationView).Id = eventPin.EventId;
            }

            annotationView.CanShowCallout = true;

            return annotationView;
        }

        void OnCalloutAccessoryControlTapped(object sender, MKMapViewAccessoryTappedEventArgs e)
        {
            EventMapMKAnnotationView eventView = e.View as EventMapMKAnnotationView;

            if (!string.IsNullOrWhiteSpace(eventView.Url))
            {
                UIApplication.SharedApplication.OpenUrl(new Foundation.NSUrl(eventView.Url));
            }
        }

        void OnDidSelectAnnotationView(object sender, MKAnnotationViewEventArgs e)
        {
            EventMapMKAnnotationView customView = e.View as EventMapMKAnnotationView;
            eventPinView = new UIView();

            if (customView.Name.Equals("Xamarin"))
            {
                eventPinView.Frame = new CGRect(0, 0, 200, 84);
                var image = new UIImageView(new CGRect(0, 0, 200, 84))
                {
                    Image = UIImage.FromFile("xamarin.png")
                };
                eventPinView.AddSubview(image);
                eventPinView.Center = new CGPoint(0, -(e.View.Frame.Height + 75));
                e.View.AddSubview(eventPinView);
            }
        }

        void OnDidDeselectAnnotationView(object sender, MKAnnotationViewEventArgs e)
        {
            if (!e.View.Selected)
            {
                eventPinView.RemoveFromSuperview();
                eventPinView.Dispose();
                eventPinView = null;
            }
        }

        EventPin GetEventPin(MKPointAnnotation annotation)
        {
            var position = new Position(annotation.Coordinate.Latitude, annotation.Coordinate.Longitude);
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
