using LostFoundPetReporter.Mobile.ViewModels;
using Microsoft.Maui.Controls;

namespace LostFoundPetReporter.Mobile.Views;

public partial class SpecificLostReportPage : ContentPage
{
    private readonly SpecificLostReportViewModel _viewModel;

    public SpecificLostReportPage(SpecificLostReportViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Skips custom loading logic because the ViewModel handles it via IQueryAttributable
        if (BindingContext is IQueryAttributable)
            return;
    }
}