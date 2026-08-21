using LostFoundPetReporter.Mobile.Models;
using LostFoundPetReporter.Mobile.Services.Api;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Controls;

namespace LostFoundPetReporter.Mobile.ViewModels;

public class SpecificLostReportViewModel : INotifyPropertyChanged, IQueryAttributable
{
    private readonly ILostReportApiService _lostReportApiService;

    private LostReport? _lostReport;
    private string _errorMessage = string.Empty;
    private bool _isLoading;

    public LostReport? LostReport
    {
        get => _lostReport;
        private set
        {
            if (_lostReport == value)
                return;

            _lostReport = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<FoundReport> FoundReports { get; } = new();


    public string ErrorMessage
    {
        get => _errorMessage;
        private set
        {
            if (_errorMessage == value)
                return;

            _errorMessage = value;
            OnPropertyChanged();
        }
    }

    public bool IsLoading
    {
        get => _isLoading;
        private set
        {
            if (_isLoading == value)
                return;

            _isLoading = value;
            OnPropertyChanged();
        }
    }

    public SpecificLostReportViewModel(ILostReportApiService lostReportApiService)
    {
        _lostReportApiService = lostReportApiService;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (!query.TryGetValue("id", out var value))
            return;

        if (!int.TryParse(value?.ToString(), out int lostReportId))
            return;

        _ = LoadAsync(lostReportId);
    }

    public async Task LoadAsync(int lostReportId)
    {
        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            var report = await _lostReportApiService.GetLostReportAsync(lostReportId);

            if (report == null)
            {
                ErrorMessage = "Lost report not found.";
                return;
            }

            LostReport = report;

            FoundReports.Clear();

            if (report.FoundReports != null)
            {
                foreach (var foundReport in report.FoundReports)
                {
                    FoundReports.Add(foundReport);
                }
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }
}