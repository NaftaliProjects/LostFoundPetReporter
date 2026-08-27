using LostFoundPetReporter.Mobile.ViewModels;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Projections;
using Mapsui.Styles;
using Mapsui.Tiling;
using Mapsui.UI.Maui;

namespace LostFoundPetReporter.Mobile.Views;

public partial class MapPage : ContentPage
{
    private readonly MapViewModel _viewModel;

    private readonly MapControl _mapControl;
    private readonly MyLocationLayer _myLocationLayer;
    private readonly MemoryLayer _reportsLayer;

    public MapPage(MapViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;

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

        // Lost / Found reports
        _reportsLayer = new MemoryLayer
        {
            Name = "Reports"
        };

        _mapControl.Map?.Layers.Add(_reportsLayer);

        MapContainer.Content = _mapControl;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await _viewModel.LoadAsync();

        ShowCurrentLocation();
        ShowReports();
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
                        group.Color,
                        "Found"));
            }
        }

        _reportsLayer.Features = features;
    }

    private IFeature CreateMarker(
        MPoint point,
        string color,
        string type)
    {
        var feature = new PointFeature(point);

        feature["Type"] = type;
        feature["Color"] = color;

        feature.Styles.Add(
            new SymbolStyle
            {
                SymbolScale = type == "Lost"
                    ? 1.5
                    : 1.0,

                Fill = new Mapsui.Styles.Brush(GetGroupColor(color))

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
}