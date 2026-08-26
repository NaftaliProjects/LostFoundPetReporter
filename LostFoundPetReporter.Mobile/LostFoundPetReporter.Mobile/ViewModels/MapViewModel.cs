using LostFoundPetReporter.Mobile.Models;
using LostFoundPetReporter.Mobile.Models.Map;
using LostFoundPetReporter.Mobile.Services.Api;
using LostFoundPetReporter.Mobile.Services.Map;

namespace LostFoundPetReporter.Mobile.ViewModels;

public class MapViewModel
{
    private readonly IMapService _mapService;
    private readonly ILostReportApiService _lostReportApiService;

    public MapPoint? CurrentLocation { get; private set; }

    public List<MapReportGroup> ReportGroups { get; private set; } = new();

    public MapViewModel(
        IMapService mapService,
        ILostReportApiService lostReportApiService)
    {
        _mapService = mapService;
        _lostReportApiService = lostReportApiService;
    }

    public async Task LoadAsync()
    {
        CurrentLocation = await _mapService.GetCurrentLocationAsync();

        await LoadReportsAsync();
    }

    private async Task LoadReportsAsync()
    {
        var reports = await _lostReportApiService.GetLostReportsAsync();

        ReportGroups.Clear();

        var colors = new[]
        {
            "Red",
            "Blue",
            "Green",
            "Orange",
            "Purple",
            "Yellow",
            "Pink",
            "Cyan"
        };

        int colorIndex = 0;

        foreach (var lostReport in reports)
        {
            var group = new MapReportGroup
            {
                LostReportId = lostReport.Id,
                Color = colors[colorIndex % colors.Length]
            };

            if (lostReport.LostCoordinate != null)
            {
                group.LostPoint = new MapPoint
                {
                    Latitude = lostReport.LostCoordinate.Latitude,
                    Longitude = lostReport.LostCoordinate.Longitude
                };
            }

            foreach (var foundReport in lostReport.FoundReports)
            {
                if (foundReport.FoundCoordinate == null)
                    continue;

                group.FoundPoints.Add(new MapPoint
                {
                    Latitude = foundReport.FoundCoordinate.Latitude,
                    Longitude = foundReport.FoundCoordinate.Longitude
                });
            }

            ReportGroups.Add(group);

            colorIndex++;
        }
    }
}