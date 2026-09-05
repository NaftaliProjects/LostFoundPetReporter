using LostFoundPetReporter.Mobile.Models;
using LostFoundPetReporter.Mobile.Services.Api;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace LostFoundPetReporter.Mobile.ViewModels;

public class SpecificLostReportViewModel : INotifyPropertyChanged, IQueryAttributable
{
    private readonly ILostReportApiService _lostReportApiService;


    private LostReport? _lostReport;
    private string _errorMessage = string.Empty;
    private bool _isLoading;
    private bool _isImageViewerVisible;

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

    public ObservableCollection<string> SelectedImages { get; } = new();

    public bool IsImageViewerVisible
    {
        get => _isImageViewerVisible;
        private set
        {
            if (_isImageViewerVisible == value)
                return;

            _isImageViewerVisible = value;
            OnPropertyChanged();
        }
    }

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

    public ICommand CloseImageViewerCommand { get; }
    public ICommand RemoveMatchCommand { get; }

    public SpecificLostReportViewModel(
        ILostReportApiService lostReportApiService)
    {
        _lostReportApiService = lostReportApiService;

        CloseImageViewerCommand = new Command(CloseImageViewer);
        RemoveMatchCommand = new Command<FoundReport>(async (foundReport) => await RemoveMatchAsync(foundReport));

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

        var report = await _lostReportApiService
            .GetLostReportAsync(lostReportId);

        if (report == null)
        {
            ErrorMessage = "Lost report not found.";
            return;
        }

        System.Diagnostics.Debug.WriteLine(
            $"LOST REPORT ID: {report.Id}");

        System.Diagnostics.Debug.WriteLine(
            $"FOUND REPORT COUNT: {report.FoundReports?.Count ?? 0}");

            if (report.FoundReports != null)
            {
                foreach (var foundReport in report.FoundReports)
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"FoundReport {foundReport.Id}: " +
                        $"Images = {foundReport.PictureBase64List?.Count ?? 0}");

                    if (foundReport.PictureBase64List?.Count > 0)
                    {
                        var firstImage = foundReport.PictureBase64List[0];

                        System.Diagnostics.Debug.WriteLine(
                            $"First image length = {firstImage.Length}");

                        System.Diagnostics.Debug.WriteLine(
                            $"First 50 chars = {firstImage[..Math.Min(50, firstImage.Length)]}");
                    }
                }
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

        System.Diagnostics.Debug.WriteLine(
            $"ERROR: {ex}");
    }
    finally
    {
        IsLoading = false;
    }
}

    public void OpenImageViewer(IEnumerable<string> images)
    {
        SelectedImages.Clear();

        foreach (var image in images)
        {
            if (!string.IsNullOrWhiteSpace(image))
                SelectedImages.Add(image);
        }

        if (SelectedImages.Count == 0)
            return;

        IsImageViewerVisible = true;
    }

    private void CloseImageViewer()
    {
        IsImageViewerVisible = false;
        SelectedImages.Clear();
    }


    private async Task RemoveMatchAsync(FoundReport? foundReport)
    {
        if (foundReport == null || LostReport == null)
            return;

        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            await _lostReportApiService.RemoveLostFoundMatchAsync(
                LostReport.Id,
                foundReport.Id);

            FoundReports.Remove(foundReport);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;

            System.Diagnostics.Debug.WriteLine(
                $"ERROR removing match: {ex}");
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