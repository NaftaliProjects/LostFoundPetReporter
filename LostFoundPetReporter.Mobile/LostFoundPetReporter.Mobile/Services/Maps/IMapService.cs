using LostFoundPetReporter.Mobile.Models.Map;

namespace LostFoundPetReporter.Mobile.Services.Map;

public interface IMapService
{
    Task<MapPoint?> GetCurrentLocationAsync();
}