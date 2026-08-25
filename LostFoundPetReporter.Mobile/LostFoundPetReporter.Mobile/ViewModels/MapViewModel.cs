using LostFoundPetReporter.Mobile.Models.Map;
using LostFoundPetReporter.Mobile.Services.Map;

namespace LostFoundPetReporter.Mobile.ViewModels;

public class MapViewModel
{
    private readonly IMapService _mapService;

    public MapPoint? CurrentLocation { get; private set; }

    public MapViewModel(IMapService mapService)
    {
        _mapService = mapService;
    }

    public async Task LoadLocationAsync()
    {
        CurrentLocation = await _mapService.GetCurrentLocationAsync();
    }
}