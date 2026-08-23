using LostFoundPetReporter.Mobile.ViewModels;

namespace LostFoundPetReporter.Mobile.Views;

public partial class CreateLostReportPage : ContentPage
{
    public CreateLostReportPage(CreateLostReportViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }

    private async void OnReturnClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}