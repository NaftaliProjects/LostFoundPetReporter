using LostFoundPetReporter.Mobile.Models;
using LostFoundPetReporter.Mobile.Models.Map;
using LostFoundPetReporter.Mobile.Services.Api;
using LostFoundPetReporter.Mobile.Services.Map;
using LostFoundPetReporter.Mobile.Services.Maps.Routing;
using LostFoundPetReporter.Mobile.Services.Session;
using System.Diagnostics;

namespace LostFoundPetReporter.Mobile.ViewModels;

public class MapViewModel
{
    private readonly IMapService _mapService;
    private readonly IRouteService _routeService;
    private readonly ILostReportApiService _lostReportApiService;
    private readonly IUserSession _userSession;

    public MapPoint? CurrentLocation { get; private set; }
    public RouteResult? CurrentRoute { get; private set; }

    public List<MapReportGroup> ReportGroups { get; private set; } = new();

    public MapViewModel(
        IMapService mapService,
        ILostReportApiService lostReportApiService,
        IRouteService routeService,
        IUserSession userSession)
    {
        _mapService = mapService;
        _lostReportApiService = lostReportApiService;
        _userSession = userSession;
        _routeService = routeService;
    }

    public async Task LoadAsync()
    {
        // Get current device location
        CurrentLocation = await _mapService.GetCurrentLocationAsync();

        // Get reports belonging to the current user
        await LoadReportsAsync();
    }

    private async Task LoadReportsAsync()
    {
        var user = _userSession.CurrentUser;

        if (user is null)
        {
            ReportGroups.Clear();
            return;
        }

        var reports = await _lostReportApiService
            .GetLostReportByUserIdAsync(user.Id);

        ReportGroups.Clear();

        if (reports is null)
            return;

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

            // Lost report location
            if (lostReport.LostCoordinate != null)
            {
                group.LostPoint = new MapPoint(
                    lostReport.LostCoordinate.Latitude,
                    lostReport.LostCoordinate.Longitude);
            }

            // Found report locations
            foreach (var foundReport in lostReport.FoundReports)
            {
                if (foundReport.FoundCoordinate == null)
                    continue;

                group.FoundPoints.Add(
                    new MapPoint(
                        foundReport.FoundCoordinate.Latitude,
                        foundReport.FoundCoordinate.Longitude));
            }

            ReportGroups.Add(group);

            colorIndex++;
        }
    }

    public async Task ShowRouteAsync(MapPoint destination)
    {
        if (CurrentLocation == null)
            return;

        CurrentRoute = await _routeService.GetRouteAsync(
            CurrentLocation,
            destination);
    }

    public async Task<bool> CalculateRouteAsync(
    MapPoint destination)
    {
        if (CurrentLocation == null)
        {
            Debug.WriteLine(
                "ROUTE: Current location is null.");

            return false;
        }

        Debug.WriteLine(
            $"ROUTE: Start = " +
            $"{CurrentLocation.Latitude}, " +
            $"{CurrentLocation.Longitude}");

        Debug.WriteLine(
            $"ROUTE: Destination = " +
            $"{destination.Latitude}, " +
            $"{destination.Longitude}");

        CurrentRoute =
            await _routeService.GetRouteAsync(
                CurrentLocation,
                destination);

        if (CurrentRoute == null)
        {
            Debug.WriteLine(
                "ROUTE: RouteService returned NULL.");

            return false;
        }

        Debug.WriteLine(
            $"ROUTE: Distance = " +
            $"{CurrentRoute.DistanceMeters} meters");

        Debug.WriteLine(
            $"ROUTE: Duration = " +
            $"{CurrentRoute.DurationSeconds} seconds");

        Debug.WriteLine(
            $"ROUTE: Points = " +
            $"{CurrentRoute.Points.Count}");

        return CurrentRoute.Points.Count >= 2;
    }

    public async Task<MapPoint?> UpdateCurrentLocationAsync()
    {
        CurrentLocation = await _mapService.GetCurrentLocationAsync();

        return CurrentLocation;
    }
}