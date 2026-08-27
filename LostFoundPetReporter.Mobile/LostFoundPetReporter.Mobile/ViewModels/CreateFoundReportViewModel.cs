
using LostFoundPetReporter.Mobile.Models;
using LostFoundPetReporter.Mobile.Services.Api;
using LostFoundPetReporter.Mobile.Services.Map;
using LostFoundPetReporter.Mobile.Services.Session;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
namespace LostFoundPetReporter.Mobile.ViewModels;



public class CreateFoundReportViewModel : INotifyPropertyChanged
{
    private readonly IFoundReportApiService _foundReportApiService;
    private readonly IUserSession _userSession;

    private readonly IMapService _mapService;
    public CreateFoundReportRequest Report { get; } = new();

    public AnimalDescription PetDescription => Report.PetDescription;



    public DateTime FoundDate { get; set; } = DateTime.Today;

    public TimeSpan FoundTime { get; set; } = DateTime.Now.TimeOfDay;

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

    private string? _picturePath;

    public string? PicturePath
    {
        get => _picturePath;
        set
        {
            if (_picturePath == value)
                return;

            _picturePath = value;

            OnPropertyChanged();
            OnPropertyChanged(nameof(HasPicture));
        }
    }

    public bool HasPicture => !string.IsNullOrEmpty(PicturePath);


    public ICommand TakePictureCommand { get; }
    public ICommand CreateReportCommand { get; }

    public ICommand UseCurrentLocationCommand { get; }

    public CreateFoundReportViewModel(IFoundReportApiService foundReportApiService, IUserSession userSession, IMapService mapService)
    {
        _foundReportApiService = foundReportApiService;
        _userSession = userSession;
        _mapService = mapService;

        TakePictureCommand = new Command(async () => await TakePictureAsync());

        UseCurrentLocationCommand = new Command(async () => await UseCurrentLocationAsync());


        CreateReportCommand = new Command(async () => await CreateReportAsync());

    }


    public bool HasLocation => Report.FoundCoordinate.Latitude != 0 || Report.FoundCoordinate.Longitude != 0;
    public string LocationText =>
    HasLocation
        ? $"{Report.FoundCoordinate.Latitude:F6}, " +
          $"{Report.FoundCoordinate.Longitude:F6}"
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

            Report.FoundCoordinate.Latitude = location.Latitude;
            Report.FoundCoordinate.Longitude = location.Longitude;

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

    private async Task TakePictureAsync()
    {
        try
        {
            if (!MediaPicker.Default.IsCaptureSupported)
            {
                await Shell.Current.DisplayAlert(
                    "Camera",
                    "Camera is not available on this device.",
                    "OK");

                return;
            }

            var photo = await MediaPicker.Default.CapturePhotoAsync();

            if (photo is null)
                return;

            PicturePath = photo.FullPath;

            using var stream = await photo.OpenReadAsync();
            using var memoryStream = new MemoryStream();

            await stream.CopyToAsync(memoryStream);

            var base64 = Convert.ToBase64String(memoryStream.ToArray());

            Report.PictureBase64List.Add(base64);

        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Camera Error",
                ex.Message,
                "OK");
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

            Report.dateTime = Report.dateTime + FoundTime;

            var result = await _foundReportApiService.CreateFoundReportAsync(Report);

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