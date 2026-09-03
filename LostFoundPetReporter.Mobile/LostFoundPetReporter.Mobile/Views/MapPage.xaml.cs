using LostFoundPetReporter.Mobile.Models.Map;
using LostFoundPetReporter.Mobile.ViewModels;
using LostFoundPetReporter.Mobile.Services.Compass;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Nts;
using Mapsui.Projections;
using Mapsui.Styles;
using Mapsui.Tiling;
using Mapsui.UI.Maui;
using System.Diagnostics;

namespace LostFoundPetReporter.Mobile.Views;

public partial class MapPage : ContentPage
{
    private readonly MapViewModel _viewModel;
    private readonly ICompassService _compassService;
    private readonly MapControl _mapControl;
    private readonly MyLocationLayer _myLocationLayer;
    private readonly MemoryLayer _reportsLayer;
    private readonly MemoryLayer _routeLayer;

    public MapPage(MapViewModel viewModel, ICompassService compassService)
    {
        InitializeComponent();

        _viewModel = viewModel;
        _compassService = compassService;

        BindingContext = _viewModel;

        _mapControl = new MapControl();

        // OpenStreetMap
        _mapControl.Map?.Layers.Add(
            OpenStreetMap.CreateTileLayer());

        // Current device location
        _myLocationLayer = new MyLocationLayer(
            _mapControl.Map!);

        _mapControl.Map?.Layers.Add(
            _myLocationLayer);

        // Route
        _routeLayer = new MemoryLayer
        {
            Name = "Route"
        };

        _mapControl.Map?.Layers.Add(
            _routeLayer);

        // Lost / Found reports
        _reportsLayer = new MemoryLayer
        {
            Name = "Reports"
        };

        _mapControl.Map?.Layers.Add(
            _reportsLayer);

        // Handle map taps
        _mapControl.Map!.Tapped += OnMapTapped;

        MapContainer.Content = _mapControl;
    }

    private async void OnMapTapped(
    object? sender,
    MapEventArgs e)
    {
        Debug.WriteLine("=================================");
        Debug.WriteLine("MAP TAPPED");
        Debug.WriteLine($"World position: {e.WorldPosition}");
        Debug.WriteLine($"Screen position: {e.ScreenPosition}");
        Debug.WriteLine("=================================");

        var mapInfo = e.GetMapInfo(
            new[] { _reportsLayer });

        if (mapInfo == null)
        {
            Debug.WriteLine("No MapInfo returned.");
            return;
        }

        Debug.WriteLine("MapInfo returned.");

        var feature = mapInfo.Feature;

        if (feature == null)
        {
            Debug.WriteLine("MapInfo contains no Feature.");
            return;
        }

        Debug.WriteLine("FEATURE TAPPED!");

        Debug.WriteLine(
            $"Feature type: {feature.GetType().FullName}");

        Debug.WriteLine("Feature fields:");

        foreach (var field in feature.Fields)
        {
            Debug.WriteLine($"  Field: {field}");
        }

        if (!feature.Fields.Contains("Latitude") ||
            !feature.Fields.Contains("Longitude"))
        {
            Debug.WriteLine(
                "Feature does NOT contain Latitude/Longitude.");

            return;
        }

        Debug.WriteLine(
            "Feature contains Latitude and Longitude.");

        var latitude =
            Convert.ToDouble(feature["Latitude"]);

        var longitude =
            Convert.ToDouble(feature["Longitude"]);

        Debug.WriteLine(
            $"REPORT LOCATION: {latitude}, {longitude}");

        var type =
            feature.Fields.Contains("Type")
                ? feature["Type"]?.ToString()
                : "Unknown";

        Debug.WriteLine(
            $"REPORT TYPE: {type}");

        var destination =
            new MapPoint(
                latitude,
                longitude);

        Debug.WriteLine(
            "Starting route calculation...");

        await ShowRouteToAsync(destination);

        Debug.WriteLine(
            "Route calculation finished.");

        e.Handled = true;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (!_compassService.IsSupported)
        {
            Debug.WriteLine("COMPASS: Not supported.");
            return;
        }

        Debug.WriteLine("COMPASS: Starting...");

        _compassService.HeadingChanged += OnHeadingChanged;
        _compassService.Start();


        await _viewModel.LoadAsync();


        
        ShowCurrentLocation();
        ShowReports();


    }

    protected override void OnDisappearing()
    {
        _compassService.HeadingChanged -= OnHeadingChanged;
        _compassService.Stop();

        base.OnDisappearing();
    }

    private void OnHeadingChanged(
    object? sender,
    double heading)
    {
        Debug.WriteLine(
            $"COMPASS: {heading:F1}°");
    }

    private void ShowCurrentLocation()
    {
        var location = _viewModel.CurrentLocation;

        if (location == null)
            return;

        var point = SphericalMercator.FromLonLat(
            new MPoint(
                location.Longitude,
                location.Latitude));

        _myLocationLayer.UpdateMyLocation(point);

        _mapControl.Map?.Navigator.CenterOnAndZoomTo(
            point,
            _mapControl.Map.Navigator.Resolutions[16]);
    }

    private void ShowReports()
    {
        if (_mapControl.Map == null)
            return;

        var features = new List<IFeature>();

        foreach (var group in _viewModel.ReportGroups)
        {
            // -------------------------
            // Lost report marker
            // -------------------------
            if (group.LostPoint != null)
            {
                var point = SphericalMercator.FromLonLat(
                    new MPoint(
                        group.LostPoint.Longitude,
                        group.LostPoint.Latitude));

                features.Add(
                    CreateMarker(
                        point,
                        group.LostPoint,
                        group.Color,
                        "Lost"));
            }

            // -------------------------
            // Found report markers
            // -------------------------
            foreach (var foundPoint in group.FoundPoints)
            {
                var point = SphericalMercator.FromLonLat(
                    new MPoint(
                        foundPoint.Longitude,
                        foundPoint.Latitude));



                features.Add(
                    CreateMarker(
                        point,
                        foundPoint,
                        group.Color,
                        "Found"));
            }
        }

        _reportsLayer.Features = features;
    }

    private IFeature CreateMarker(
    MPoint point,
    MapPoint geographicPoint,
    string color,
    string type)
    {
        Debug.WriteLine(
        $"Creating {type} marker: " +
        $"{geographicPoint.Latitude}, " +
        $"{geographicPoint.Longitude}");

        var feature = new PointFeature(point);

        feature["Type"] = type;
        feature["Color"] = color;

        feature["Latitude"] =
            geographicPoint.Latitude;

        feature["Longitude"] =
            geographicPoint.Longitude;

        feature.Styles.Add(
            new SymbolStyle
            {
                SymbolScale = type == "Lost"
                    ? 1.5
                    : 1.0,

                Fill = new Mapsui.Styles.Brush(
                    GetGroupColor(color))
            });

        return feature;
    }

    private static Mapsui.Styles.Color GetGroupColor(string color)
    {
        return color switch
        {
            "Red" => Mapsui.Styles.Color.Red,
            "Blue" => Mapsui.Styles.Color.Blue,
            "Green" => Mapsui.Styles.Color.Green,
            "Orange" => Mapsui.Styles.Color.Orange,
            "Purple" => Mapsui.Styles.Color.Purple,
            "Yellow" => Mapsui.Styles.Color.Yellow,
            "Pink" => Mapsui.Styles.Color.Pink,
            "Cyan" => Mapsui.Styles.Color.Cyan,
            _ => Mapsui.Styles.Color.Gray
        };
    }

    private void ShowRoute()
    {
        if (_mapControl.Map == null)
            return;

        var route = _viewModel.CurrentRoute;

        if (route == null || route.Points.Count < 2)
        {
            _routeLayer.Features = new List<IFeature>();
            return;
        }

        var coordinates = route.Points
            .Select(point =>
                SphericalMercator.FromLonLat(
                    new MPoint(
                        point.Longitude,
                        point.Latitude)))
            .ToList();

        var lineString = new NetTopologySuite.Geometries.LineString(
            coordinates
                .Select(p => new NetTopologySuite.Geometries.Coordinate(
                    p.X,
                    p.Y))
                .ToArray());

        var feature = new GeometryFeature(lineString);

        feature.Styles.Add(
            new VectorStyle
            {
                Line = new Pen(
                    Mapsui.Styles.Color.Blue,
                    width: 5)
            });

        _routeLayer.Features = new List<IFeature>
            {
                feature
            };
    }

    private async Task ShowRouteToAsync(
    MapPoint destination)
    {
        var success =
            await _viewModel.CalculateRouteAsync(
                destination);

        if (!success)
            return;

        ShowRoute();

        ZoomToRoute();
    }

    private void ZoomToRoute()
    {
        var route = _viewModel.CurrentRoute;

        if (route == null || route.Points.Count == 0)
            return;

        var points = route.Points
            .Select(point =>
                SphericalMercator.FromLonLat(
                    new MPoint(
                        point.Longitude,
                        point.Latitude)))
            .ToList();

        if (points.Count == 0)
            return;

        var minX = points.Min(p => p.X);
        var maxX = points.Max(p => p.X);
        var minY = points.Min(p => p.Y);
        var maxY = points.Max(p => p.Y);

        var center = new MPoint(
            (minX + maxX) / 2,
            (minY + maxY) / 2);

        _mapControl.Map?.Navigator.CenterOnAndZoomTo(
            center,
            _mapControl.Map.Navigator.Resolutions[14]);
    }


}