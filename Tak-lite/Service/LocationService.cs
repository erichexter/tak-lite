using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tak_lite
{
    public class LocationService
    {
        private bool simulate = true;
        private CancellationTokenSource _cancelTokenSource;
        private bool _isCheckingLocation;

        public async Task<Location> GetCurrentLocation()
        {
            try
            {
                _isCheckingLocation = true;


                var locationTaskCompletionSource = new TaskCompletionSource<Location>();

                Device.BeginInvokeOnMainThread(async () =>
                {
                    PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                    if (status != PermissionStatus.Granted)
                    {
                        status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                    }


                    GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(10));

                    _cancelTokenSource = new CancellationTokenSource();
                
                    Location location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);

                    if (simulate)
                    {
                        location.Latitude += (Random.Shared.Next(10)-5) * 0.00005;
                        location.Longitude += (Random.Shared.Next(10)-5) * 0.00005;
                    }

                    locationTaskCompletionSource.SetResult(location);
                });

                var location = await locationTaskCompletionSource.Task;


                if (location != null)
                {
                    Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
                    return location;
                }
            }
            // Catch one of the following exceptions:
            //   FeatureNotSupportedException
            //   FeatureNotEnabledException
            //   PermissionException
            catch (Exception ex)
            {
                Debug.WriteLine(ex );
            }
            finally
            {
                _isCheckingLocation = false;
            }

            return null;
        }

        public void CancelRequest()
        {
            if (_isCheckingLocation && _cancelTokenSource != null && _cancelTokenSource.IsCancellationRequested == false)
                _cancelTokenSource.Cancel();
        }
    }
}
