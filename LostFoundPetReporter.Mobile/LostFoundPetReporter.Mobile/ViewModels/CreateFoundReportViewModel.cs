
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

    public CreateAnimalDescription PetDescription => Report.PetDescription;



    private DateTime _foundDate = DateTime.Today;
    public DateTime FoundDate
    {
        get => _foundDate;
        set
        {
            if (_foundDate == value)
                return;

            _foundDate = value;

            UpdateReportDateTime();

            OnPropertyChanged();
        }
    }

    private void UpdateReportDateTime()
    {
        Report.dateTime = _foundDate.Date + _foundTime;
    }

    private TimeSpan _foundTime = DateTime.Now.TimeOfDay;
    public TimeSpan FoundTime
    {
        get => _foundTime;
        set
        {
            if (_foundTime == value)
                return;

            _foundTime = value;

            UpdateReportDateTime();

            OnPropertyChanged();
        }
    }

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
    public ICommand AutoFillAnimalDescriptionCommand { get; }
    public ICommand PickPictureCommand { get; }

    public ICommand ToggleAdditionalDetailsCommand { get; }



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



    public CreateFoundReportViewModel(IFoundReportApiService foundReportApiService, IUserSession userSession, IMapService mapService)
    {
        _foundReportApiService = foundReportApiService;
        _userSession = userSession;
        _mapService = mapService;

        TakePictureCommand = new Command(async () => await TakePictureAsync());

        UseCurrentLocationCommand = new Command(async () => await UseCurrentLocationAsync());

        UpdateReportDateTime();


        CreateReportCommand = new Command(async () => await CreateReportAsync());

        AutoFillAnimalDescriptionCommand = new Command(async () => await AutoFillAnimalDescriptionAsync());

        PickPictureCommand = new Command(async () => await PickPictureAsync());

        ToggleAdditionalDetailsCommand = new Command(() => {IsAdditionalDetailsExpanded = !IsAdditionalDetailsExpanded; });


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
                await _foundReportApiService.ImageToAnimalDescriptionAsync(
                    Report.PictureBase64List);

            if (result is null)
            {
                await Shell.Current.DisplayAlert(
                    "Animal Description",
                    "Could not identify the animal.",
                    "OK");

                return;
            }
            Report.PetDescription.Name = result.Name;
            Report.PetDescription.Colors = result.Colors;
            Report.PetDescription.Type = result.Type;
            Report.PetDescription.Breed = result.Breed;

            Report.PetDescription.Sex = result.Sex;
            Report.PetDescription.Age = result.Age;
            Report.PetDescription.Size = result.Size;
            Report.PetDescription.WeightKg = result.WeightKg;

            Report.PetDescription.CoatLength = result.CoatLength;
            Report.PetDescription.CoatType = result.CoatType;
            Report.PetDescription.Pattern = result.Pattern;

            Report.PetDescription.DistinctiveMarkings =
                result.DistinctiveMarkings;

            Report.PetDescription.EyeColor =
                result.EyeColor;

            Report.PetDescription.EarDescription =
                result.EarDescription;

            Report.PetDescription.TailDescription =
                result.TailDescription;

            Report.PetDescription.CollarPresent =
                result.CollarPresent;

            Report.PetDescription.CollarColor =
                result.CollarColor;

            Report.PetDescription.CollarType =
                result.CollarType;

            Report.PetDescription.HarnessPresent =
                result.HarnessPresent;

            Report.PetDescription.HarnessColor =
                result.HarnessColor;

            // Tell the UI that the PetDescription properties changed.
            OnPropertyChanged(nameof(PetDescription));

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

    private async Task PickPictureAsync()
    {
        try
        {
            var options = new PickOptions
            {
                PickerTitle = "Select an animal picture",
                FileTypes = FilePickerFileType.Images
            };

            var result = await FilePicker.Default.PickAsync(options);

            if (result is null)
                return;

            PicturePath = result.FullPath;

            using var stream = await result.OpenReadAsync();
            using var memoryStream = new MemoryStream();

            await stream.CopyToAsync(memoryStream);

            var base64 = Convert.ToBase64String(
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


    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }
}