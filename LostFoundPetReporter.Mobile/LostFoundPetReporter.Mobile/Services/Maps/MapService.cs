using LostFoundPetReporter.Mobile.Models.Map;

namespace LostFoundPetReporter.Mobile.Services.Map;

public class MapService : IMapService
{
    public async Task<MapPoint?> GetCurrentLocationAsync()
    {
        try
        {
            var location = await Geolocation.Default.GetLocationAsync(
                new GeolocationRequest(
                    GeolocationAccuracy.Medium,
                    TimeSpan.FromSeconds(10)));

            if (location == null)
                return null;

            return new MapPoint(
                location.Latitude,
                location.Longitude);
        }
        catch (PermissionException)
        {
            return null;
        }
        catch (FeatureNotEnabledException)
        {
            return null;
        }
    }
}