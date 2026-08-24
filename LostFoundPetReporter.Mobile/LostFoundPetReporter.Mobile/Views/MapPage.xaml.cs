using Android.Graphics;
using Mapsui.Tiling;

namespace LostFoundPetReporter.Mobile.Views;

public partial class MapPage : ContentPage
{
    public MapPage()
    {
        InitializeComponent();

        PetMap.Map?.Layers.Add(
            OpenStreetMap.CreateTileLayer());

        AddTestPin();
    }

    private void AddTestPin()
    {
        // We'll add the marker in the next step.
    }
}