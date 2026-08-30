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

    public CreateAnimalDescription PetDescription => Report.PetDescription;



    // ---------------------------------------------------------
    // Date / Time
    // ---------------------------------------------------------

    private DateTime _lostDate = DateTime.Today;

    public DateTime LostDate
    {
        get => _lostDate;
        set
        {
            if (_lostDate == value)
                return;

            _lostDate = value;

            UpdateReportDateTime();

            OnPropertyChanged();
        }
    }


    private TimeSpan _lostTime = DateTime.Now.TimeOfDay;

    public TimeSpan LostTime
    {
        get => _lostTime;
        set
        {
            if (_lostTime == value)
                return;

            _lostTime = value;

            UpdateReportDateTime();

            OnPropertyChanged();
        }
    }


    private void UpdateReportDateTime()
    {
        Report.dateTime =
            _lostDate.Date + _lostTime;
    }


    // ---------------------------------------------------------
    // Busy
    // ---------------------------------------------------------

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


    // ---------------------------------------------------------
    // Picture
    // ---------------------------------------------------------

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

    public bool HasPicture =>
        !string.IsNullOrEmpty(PicturePath);


    // ---------------------------------------------------------
    // Additional details
    // ---------------------------------------------------------

    private bool _isAdditionalDetailsExpanded;

    public bool IsAdditionalDetailsExpanded
    {
        get => _isAdditionalDetailsExpanded;
        set
        {
            if (_isAdditionalDetailsExpanded == value)
                return;

            _isAdditionalDetailsExpanded = value;

            OnPropertyChanged();
            OnPropertyChanged(nameof(AdditionalDetailsButtonText));
        }
    }

    public string AdditionalDetailsButtonText =>
        IsAdditionalDetailsExpanded
            ? "▲ Hide Additional Details"
            : "▼ Add More Animal Details";


    // ---------------------------------------------------------
    // Commands
    // ---------------------------------------------------------

    public ICommand UseCurrentLocationCommand { get; }

    public ICommand CreateReportCommand { get; }

    public ICommand TakePictureCommand { get; }

    public ICommand PickPictureCommand { get; }

    public ICommand AutoFillAnimalDescriptionCommand { get; }

    public ICommand ToggleAdditionalDetailsCommand { get; }


    // ---------------------------------------------------------
    // Constructor
    // ---------------------------------------------------------

    public CreateLostReportViewModel(
        ILostReportApiService lostReportApiService,
        IUserSession userSession,
        IMapService mapService)
    {
        _lostReportApiService = lostReportApiService;
        _userSession = userSession;
        _mapService = mapService;

        UseCurrentLocationCommand =
            new Command(async () =>
                await UseCurrentLocationAsync());

        CreateReportCommand =
            new Command(async () =>
                await CreateReportAsync());

        TakePictureCommand =
            new Command(async () =>
                await TakePictureAsync());

        PickPictureCommand =
            new Command(async () =>
                await PickPictureAsync());

        AutoFillAnimalDescriptionCommand =
            new Command(async () =>
                await AutoFillAnimalDescriptionAsync());

        ToggleAdditionalDetailsCommand =
            new Command(() =>
            {
                IsAdditionalDetailsExpanded =
                    !IsAdditionalDetailsExpanded;
            });

        UpdateReportDateTime();
    }


    // ---------------------------------------------------------
    // Location
    // ---------------------------------------------------------

    public bool HasLocation =>
        Report.LostCoordinate.Latitude != 0 ||
        Report.LostCoordinate.Longitude != 0;


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

            var location =
                await _mapService.GetCurrentLocationAsync();

            if (location is null)
            {
                await Shell.Current.DisplayAlert(
                    "Location",
                    "Unable to determine your current location.",
                    "OK");

                return;
            }

            Report.LostCoordinate.Latitude =
                location.Latitude;

            Report.LostCoordinate.Longitude =
                location.Longitude;

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


    // ---------------------------------------------------------
    // Take Picture
    // ---------------------------------------------------------

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

            var photo =
                await MediaPicker.Default.CapturePhotoAsync();

            if (photo is null)
                return;

            PicturePath = photo.FullPath;

            using var stream =
                await photo.OpenReadAsync();

            using var memoryStream =
                new MemoryStream();

            await stream.CopyToAsync(memoryStream);

            var base64 =
                Convert.ToBase64String(
                    memoryStream.ToArray());

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


    // ---------------------------------------------------------
    // Pick Picture
    // ---------------------------------------------------------

    private async Task PickPictureAsync()
    {
        try
        {
            var options = new PickOptions
            {
                PickerTitle = "Select an animal picture",
                FileTypes = FilePickerFileType.Images
            };

            var result =
                await FilePicker.Default.PickAsync(options);

            if (result is null)
                return;

            PicturePath = result.FullPath;

            using var stream =
                await result.OpenReadAsync();

            using var memoryStream =
                new MemoryStream();

            await stream.CopyToAsync(memoryStream);

            var base64 =
                Convert.ToBase64String(
                    memoryStream.ToArray());

            Report.PictureBase64List.Add(base64);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Picture Error",
                ex.Message,
                "OK");
        }
    }


    // ---------------------------------------------------------
    // AI Auto Fill
    // ---------------------------------------------------------

    private async Task AutoFillAnimalDescriptionAsync()
    {
        if (IsBusy)
            return;

        if (Report.PictureBase64List == null ||
            Report.PictureBase64List.Count == 0)
        {
            await Shell.Current.DisplayAlert(
                "Animal Description",
                "Please take a picture of the animal first.",
                "OK");

            return;
        }

        try
        {
            IsBusy = true;

            var result =
                await _lostReportApiService
                    .ImageToAnimalDescriptionAsync(
                        Report.PictureBase64List);

            if (result is null)
            {
                await Shell.Current.DisplayAlert(
                    "Animal Description",
                    "Could not identify the animal.",
                    "OK");

                return;
            }

            var pet = Report.PetDescription;

            pet.Name = result.Name;
            pet.Colors = result.Colors;
            pet.Type = result.Type;
            pet.Breed = result.Breed;

            pet.Sex = result.Sex;
            pet.Age = result.Age;
            pet.Size = result.Size;
            pet.WeightKg = result.WeightKg;

            pet.CoatLength = result.CoatLength;
            pet.CoatType = result.CoatType;
            pet.Pattern = result.Pattern;

            pet.DistinctiveMarkings =
                result.DistinctiveMarkings;

            pet.EyeColor = result.EyeColor;

            pet.EarDescription =
                result.EarDescription;

            pet.TailDescription =
                result.TailDescription;

            pet.CollarPresent =
                result.CollarPresent;

            pet.CollarColor =
                result.CollarColor;

            pet.CollarType =
                result.CollarType;

            pet.HarnessPresent =
                result.HarnessPresent;

            pet.HarnessColor =
                result.HarnessColor;

            await Shell.Current.DisplayAlert(
                "Animal Description",
                "Animal information was filled automatically.",
                "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "AI Error",
                ex.Message,
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }


    // ---------------------------------------------------------
    // Create Report
    // ---------------------------------------------------------

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
                "Please select the location where the animal was lost.",
                "OK");

            return;
        }

        try
        {
            IsBusy = true;

            Report.UserId = user.Id;

            // Report.dateTime is already maintained
            // by LostDate / LostTime.
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


    // ---------------------------------------------------------
    // Property Changed
    // ---------------------------------------------------------

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }
}