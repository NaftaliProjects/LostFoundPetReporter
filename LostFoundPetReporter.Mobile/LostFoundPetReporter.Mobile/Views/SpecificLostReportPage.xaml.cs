using LostFoundPetReporter.Mobile.Models;
using LostFoundPetReporter.Mobile.ViewModels;

namespace LostFoundPetReporter.Mobile.Views;

public partial class SpecificLostReportPage : ContentPage
{
    private readonly SpecificLostReportViewModel _viewModel;

    public SpecificLostReportPage(
        SpecificLostReportViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is IQueryAttributable)
            return;
    }

    private void OnPhotoCollectionTapped(
        object? sender,
        TappedEventArgs e)
    {
        if (sender is not Border border)
            return;

        if (border.BindingContext is not FoundReport foundReport)
            return;

        _viewModel.OpenImageViewer(
            foundReport.PictureBase64List);
    }

    private void OnCloseImageViewerClicked(
        object? sender,
        EventArgs e)
    {
        if (_viewModel.CloseImageViewerCommand.CanExecute(null))
        {
            _viewModel.CloseImageViewerCommand.Execute(null);
        }
    }
}