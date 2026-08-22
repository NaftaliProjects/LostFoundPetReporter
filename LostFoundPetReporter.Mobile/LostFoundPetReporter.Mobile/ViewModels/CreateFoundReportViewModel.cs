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

    public ICommand CreateReportCommand { get; }

    public CreateFoundReportViewModel(
        IFoundReportApiService foundReportApiService,
        IUserSession userSession)
    {
        _foundReportApiService = foundReportApiService;
        _userSession = userSession;

        CreateReportCommand = new Command(
            async () => await CreateReportAsync());
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

            Report.dateTime = FoundDate.Date + FoundTime;

            var result =
                await _foundReportApiService
                    .CreateFoundReportAsync(Report);

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