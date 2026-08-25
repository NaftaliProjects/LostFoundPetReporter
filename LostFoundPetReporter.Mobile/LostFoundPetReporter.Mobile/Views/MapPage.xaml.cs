using LostFoundPetReporter.Mobile.ViewModels;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Projections;
using Mapsui.Tiling;
using Mapsui.UI.Maui;

namespace LostFoundPetReporter.Mobile.Views;

public partial class MapPage : ContentPage
{
    private readonly MapViewModel _viewModel;

    private readonly MapControl _mapControl;
    private readonly MyLocationLayer _myLocationLayer;

    public MapPage(MapViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;

        BindingContext = _viewModel;

        _mapControl = new MapControl();

        _mapControl.Map?.Layers.Add(
            OpenStreetMap.CreateTileLayer());

        _myLocationLayer = new MyLocationLayer(
            _mapControl.Map!);

        _mapControl.Map?.Layers.Add(
            _myLocationLayer);

        MapContainer.Content = _mapControl;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await _viewModel.LoadLocationAsync();

        ShowCurrentLocation();
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
}