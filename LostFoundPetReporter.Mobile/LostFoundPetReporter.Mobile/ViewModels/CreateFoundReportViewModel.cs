using LostFoundPetReporter.Mobile.Models;
using LostFoundPetReporter.Mobile.Services.Api;
using LostFoundPetReporter.Mobile.Services.Session;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace LostFoundPetReporter.Mobile.ViewModels;

public class CreateFoundReportViewModel : INotifyPropertyChanged
{
    private readonly IFoundReportApiService _foundReportApiService;
    private readonly IUserSession _userSession;

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

    public CreateFoundReportViewModel(IFoundReportApiService foundReportApiService, IUserSession userSession)
    {
        _foundReportApiService = foundReportApiService;
        _userSession = userSession;

        TakePictureCommand = new Command(async () => await TakePictureAsync());


        CreateReportCommand = new Command(async () => await CreateReportAsync());

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

            Report.PictureBase64 = Convert.ToBase64String(memoryStream.ToArray());

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