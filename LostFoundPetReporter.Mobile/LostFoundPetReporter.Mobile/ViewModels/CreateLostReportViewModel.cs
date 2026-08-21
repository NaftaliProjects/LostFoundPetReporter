using LostFoundPetReporter.Mobile.Models;
using LostFoundPetReporter.Mobile.Services.Api;
using LostFoundPetReporter.Mobile.Services.Session;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace LostFoundPetReporter.Mobile.ViewModels;

public class CreateLostReportViewModel : INotifyPropertyChanged
{
    private readonly ILostReportApiService _lostReportApiService;
    private readonly IUserSession _userSession;

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

    public ICommand CreateReportCommand { get; }

    public CreateLostReportViewModel(
        ILostReportApiService lostReportApiService,
        IUserSession userSession)
    {
        _lostReportApiService = lostReportApiService;
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

            Report.dateTime = LostDate.Date + LostTime;

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