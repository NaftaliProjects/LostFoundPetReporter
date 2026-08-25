using LostFoundPetReporter.Mobile.Models;
using LostFoundPetReporter.Mobile.Services.Api;
using LostFoundPetReporter.Mobile.Services.Map;
using LostFoundPetReporter.Mobile.Services.Session;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace LostFoundPetReporter.Mobile.ViewModels;

public class CreateLostReportViewModel : INotifyPropertyChanged
{
    private readonly ILostReportApiService _lostReportApiService;
    private readonly IUserSession _userSession;

    private readonly IMapService _mapService;

    public CreateLostReportRequest Report { get; } = new();

    public AnimalDescription PetDescription => Report.PetDescription;

    public DateTime LostDate { get; set; } = DateTime.Today;

    public TimeSpan LostTime { get; set; } = DateTime.Now.TimeOfDay;

    private bool _isBusy;
    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            if (_isBusy == value)
                return;

            _isBusy = value;
            OnPropertyChanged();
        }
    }

    public ICommand UseCurrentLocationCommand { get; }
    public ICommand CreateReportCommand { get; }

    public CreateLostReportViewModel(ILostReportApiService lostReportApiService, IUserSession userSession, IMapService mapService)
    {
        _lostReportApiService = lostReportApiService;
        _userSession = userSession;
        _mapService = mapService;

        CreateReportCommand = new Command(async () => await CreateReportAsync());

        UseCurrentLocationCommand = new Command(async () => await UseCurrentLocationAsync());
    }

    public bool HasLocation => Report.LostCoordinate.Latitude != 0 || Report.LostCoordinate.Longitude != 0;
    public string LocationText =>
    HasLocation
        ? $"{Report.LostCoordinate.Latitude:F6}, " +
          $"{Report.LostCoordinate.Longitude:F6}"
        : "Location not selected";

    private async Task UseCurrentLocationAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            var location = await _mapService.GetCurrentLocationAsync();

            if (location is null)
            {
                await Shell.Current.DisplayAlert(
                    "Location",
                    "Unable to determine your current location.",
                    "OK");

                return;
            }

            Report.LostCoordinate.Latitude = location.Latitude;
            Report.LostCoordinate.Longitude = location.Longitude;

            OnPropertyChanged(nameof(HasLocation));
            OnPropertyChanged(nameof(LocationText));
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Location Error",
                ex.Message,
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task CreateReportAsync()
    {
        if (IsBusy)
            return;

        var user = _userSession.CurrentUser;

        if (user is null)
        {
            await Shell.Current.DisplayAlert(
                "Error",
                "You must be logged in to create a report.",
                "OK");

            return;
        }

        if (!HasLocation)
        {
            await Shell.Current.DisplayAlert(
                "Location Required",
                "Please select your current location before creating the report.",
                "OK");

            return;
        }

        try
        {
            IsBusy = true;

            Report.UserId = user.Id;

            Report.dateTime = Report.dateTime + LostTime;

            var result =
                await _lostReportApiService
                    .CreateLostReportAsync(Report);

            if (result is null)
            {
                await Shell.Current.DisplayAlert(
                    "Error",
                    "The report could not be created.",
                    "OK");

                return;
            }

            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Error",
                ex.Message,
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }
}