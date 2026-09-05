using LostFoundPetReporter.Mobile.Models;
using LostFoundPetReporter.Mobile.ViewModels;

namespace LostFoundPetReporter.Mobile.Views;

public partial class MyReportsPage : ContentPage
{
    private readonly MyReportsViewModel _viewModel;

    public MyReportsPage(MyReportsViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await _viewModel.LoadReportsAsync();
    }

    private async void OnReportSelected(object? sender, SelectionChangedEventArgs e)
    {
        var report = e.CurrentSelection.FirstOrDefault() as LostReport;

        if (report == null)
            return;

        if (sender is CollectionView collectionView)
        {
            collectionView.SelectedItem = null;
        }

        await Shell.Current.GoToAsync($"specificlostreport?id={report.Id}");
    }
}