﻿using CommunityToolkit.Maui.Alerts;
using CoreLocation;
using System;
using System.Threading.Tasks;
using TrashMobMobile.Services;

namespace TrashMobMobile.iOSPlatform
{
    public class GeolocatorImplementation : IGeolocator
    {
        readonly CLLocationManager manager = new();

        public async Task StartListening(IProgress<Location> positionChangedProgress,
            CancellationToken cancellationToken)
        {
            var permission = await Permissions.CheckStatusAsync<Permissions.LocationAlways>();
            if (permission != PermissionStatus.Granted)
            {
                permission = await Permissions.RequestAsync<Permissions.LocationAlways>();
                if (permission != PermissionStatus.Granted)
                {
                    await Toast.Make("No permission").Show(CancellationToken.None);
                    return;
                }
            }

            var taskCompletionSource = new TaskCompletionSource();
            cancellationToken.Register(() =>
            {
                manager.LocationsUpdated -= PositionChanged;
                taskCompletionSource.TrySetResult();
            });
            manager.LocationsUpdated += PositionChanged;

            void PositionChanged(object? sender, CLLocationsUpdatedEventArgs args)
            {
                if (args.Locations.Length > 0)
                {
                    var coordinate = args.Locations[^1].Coordinate;
                    positionChangedProgress.Report(new Location(coordinate.Latitude, coordinate.Longitude));
                }
            }

            await taskCompletionSource.Task;
        }
    }
}