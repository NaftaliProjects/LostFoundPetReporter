namespace LostFoundPetReporter.Mobile.Controls;

public partial class BottomNavigationBar : ContentView
{
    public BottomNavigationBar()
    {
        InitializeComponent();
    }

    private async void OnProfileClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//profile");
    }

    private async void OnCameraClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//camera");
    }

    private async void OnHomeClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//home");
    }

    private async void OnMapClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//map");
    }

    private async void OnReportsClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//myreports");
    }
}