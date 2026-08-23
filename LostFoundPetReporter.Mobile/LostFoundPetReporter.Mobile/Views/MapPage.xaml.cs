using Microsoft.Maui.Controls.Maps;

namespace LostFoundPetReporter.Mobile.Views;

public partial class MapPage : ContentPage
{
    public MapPage()
    {
        InitializeComponent();

        AddTestPin();
    }

    private void AddTestPin()
    {
        var pin = new Pin
        {
            Label = "Lost Dog",
            Location = new Location(32.0853, 34.7818)
        };

        PetMap.Pins.Add(pin);
    }
}