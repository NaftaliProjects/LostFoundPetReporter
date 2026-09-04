using LostFoundPetReporter.Mobile.Models.Map;
using LostFoundPetReporter.Mobile.Services.Compass;
using LostFoundPetReporter.Mobile.Services.Maps.Routing;
using LostFoundPetReporter.Mobile.ViewModels;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Nts;
using Mapsui.Projections;
using Mapsui.Styles;
using Mapsui.Tiling;
using Mapsui.UI.Maui;
using NetTopologySuite.Geometries;
using System.Diagnostics;

namespace LostFoundPetReporter.Mobile.Views;

public partial class MapPage : ContentPage
{
    private readonly MapViewModel _viewModel;
    private readonly ICompassService _compassService;
    private readonly IRouteProgressService _routeProgressService;

    private readonly MapControl _mapControl;
    private readonly MyLocationLayer _myLocationLayer;
    private readonly MemoryLayer _reportsLayer;
    private readonly MemoryLayer _routeLayer;
    private MapPoint? _lastRouteDrawLocation;



    //Compass
    private readonly MemoryLayer _directionLayer;
    private double _currentHeading;
    private CancellationTokenSource? _directionUpdateCts;



    public MapPage(MapViewModel viewModel, ICompassService compassService, IRouteProgressService routeProgressService)
    {
        InitializeComponent();

        _viewModel = viewModel;
        _compassService = compassService;
        _routeProgressService = routeProgressService;

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

        //Direction Layer
        _directionLayer = new MemoryLayer
        {
            Name = "Direction"
        };

        _mapControl.Map?.Layers.Add(
            _directionLayer);

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

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_compassService.IsSupported)
        {
            _compassService.HeadingChanged += OnHeadingChanged;
            _compassService.Start();

            StartDirectionUpdates();
        }

        await _viewModel.LoadAsync();

        ShowCurrentLocation();
        ShowReports();
    }

    private void StartDirectionUpdates()
    {
        _directionUpdateCts?.Cancel();
        _directionUpdateCts = new CancellationTokenSource();

        _ = DirectionUpdateLoopAsync(
            _directionUpdateCts.Token);
    }

    private async Task DirectionUpdateLoopAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var location = await _viewModel.UpdateCurrentLocationAsync();


                if (location != null)
                {
                    await MainThread.InvokeOnMainThreadAsync(
                        () =>
                        {
                            UpdateMapLocation(location);
                            UpdateDirectionIndicator();
                        });
                }

                await Task.Delay(33, cancellationToken);

            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Location/direction loop error: {ex}");

                await Task.Delay(100, cancellationToken);
            }
        }
    }

    private void UpdateMapLocation(MapPoint location)
    {
        var point = ToMapPoint(location);

        _myLocationLayer.UpdateMyLocation(point);

        UpdateRouteProgress(location);

        _mapControl.Refresh();
    }


    


    private void UpdateRouteProgress(MapPoint location)
    {
        var route = _viewModel.CurrentRoute;

        if (route == null)
            return;

        if (_lastRouteDrawLocation != null && _routeProgressService.GetDistanceMeters(_lastRouteDrawLocation, location) < 1.0)
        {
            return;
        }

        ShowRoute();

        _lastRouteDrawLocation = location;
    }

    

    protected override void OnDisappearing()
    {
        _compassService.HeadingChanged -= OnHeadingChanged;
        _compassService.Stop();

        _directionUpdateCts?.Cancel();
        _directionUpdateCts = null;

        base.OnDisappearing();
    }


    private void OnHeadingChanged(
       object? sender,
       double heading)
    {
        _currentHeading = heading;
    }


    private void UpdateDirectionIndicator()
    {
        if (_mapControl.Map == null)
            return;

        var location = _viewModel.CurrentLocation;

        if (location == null)
            return;

        const double lengthMeters = 80;
        const double widthMeters = 35;

        var heading = _currentHeading;

        var center = SphericalMercator.FromLonLat(
            new MPoint(
                location.Longitude,
                location.Latitude));

        var headingRadians =
            heading * Math.PI / 180.0;

        var leftRadians =
            (heading - 90) * Math.PI / 180.0;

        var rightRadians =
            (heading + 90) * Math.PI / 180.0;

        // Approximate meters -> latitude/longitude.
        const double metersPerDegreeLatitude = 111_320;

        var metersPerDegreeLongitude =
            111_320 *
            Math.Cos(location.Latitude * Math.PI / 180.0);

        var tipLatitude =
            location.Latitude +
            Math.Cos(headingRadians) *
            lengthMeters /
            metersPerDegreeLatitude;

        var tipLongitude =
            location.Longitude +
            Math.Sin(headingRadians) *
            lengthMeters /
            metersPerDegreeLongitude;

        var leftLatitude =
            location.Latitude +
            Math.Cos(leftRadians) *
            widthMeters /
            metersPerDegreeLatitude;

        var leftLongitude =
            location.Longitude +
            Math.Sin(leftRadians) *
            widthMeters /
            metersPerDegreeLongitude;

        var rightLatitude =
            location.Latitude +
            Math.Cos(rightRadians) *
            widthMeters /
            metersPerDegreeLatitude;

        var rightLongitude =
            location.Longitude +
            Math.Sin(rightRadians) *
            widthMeters /
            metersPerDegreeLongitude;

        var tip = SphericalMercator.FromLonLat(
            new MPoint(
                tipLongitude,
                tipLatitude));

        var left = SphericalMercator.FromLonLat(
            new MPoint(
                leftLongitude,
                leftLatitude));

        var right = SphericalMercator.FromLonLat(
            new MPoint(
                rightLongitude,
                rightLatitude));

        var ring =
    new NetTopologySuite.Geometries.LinearRing(
        new[]
        {
            new Coordinate(center.X, center.Y),
            new Coordinate(left.X, left.Y),
            new Coordinate(tip.X, tip.Y),
            new Coordinate(right.X, right.Y),
            new Coordinate(center.X, center.Y)
        });

        var polygon = new NetTopologySuite.Geometries.Polygon(ring);


        var feature = new GeometryFeature(polygon);

        _directionLayer.Features = new List<IFeature>
        {
            feature
        };
        _mapControl.Refresh();
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




    private void ShowCurrentLocation()
    {
        var location = _viewModel.CurrentLocation;

        Debug.WriteLine("========== CURRENT LOCATION ==========");

        if (location == null)
        {
            Debug.WriteLine("CurrentLocation is NULL");
            return;
        }

        Debug.WriteLine(
            $"Latitude: {location.Latitude}, Longitude: {location.Longitude}");

        var point = ToMapPoint(location);

        Debug.WriteLine(
            $"Map point: X={point.X}, Y={point.Y}");

        _myLocationLayer.UpdateMyLocation(point);

        _mapControl.Map?.Navigator.CenterOnAndZoomTo(
            point,
            _mapControl.Map.Navigator.Resolutions[16]);

        Debug.WriteLine("Location sent to MyLocationLayer");
        Debug.WriteLine("======================================");
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
                var point = ToMapPoint(group.LostPoint);


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
                var point = ToMapPoint(foundPoint);

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

        var currentLocation = _viewModel.CurrentLocation;

        if (currentLocation == null)
            return;


        var progress = _routeProgressService.GetProgress(currentLocation, route.Points);

        var remainingRoute = progress.RemainingRoute;


        if (remainingRoute.Count < 2)
        {
            _routeLayer.Features = new List<IFeature>();
            return;
        }


        var coordinates = remainingRoute
        .Select(point =>
            SphericalMercator.FromLonLat(
                new MPoint(
                    point.Longitude,
                    point.Latitude)))
        .ToList();

        var lineString =
       new NetTopologySuite.Geometries.LineString(
           coordinates
               .Select(p =>
                   new NetTopologySuite.Geometries.Coordinate(
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

        _lastRouteDrawLocation = null;

        ShowRoute();

        ZoomToRoute();
    }

    private void ZoomToRoute()
    {
        var route = _viewModel.CurrentRoute;

        if (route == null || route.Points.Count == 0)
            return;

        var points = route.Points
            .Select(point => ToMapPoint(point))
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




    //Helper Functions 
    private static MPoint ToMapPoint(MapPoint point)
    {
        return SphericalMercator.FromLonLat(new MPoint(point.Longitude, point.Latitude));
    }

    
    

    

}