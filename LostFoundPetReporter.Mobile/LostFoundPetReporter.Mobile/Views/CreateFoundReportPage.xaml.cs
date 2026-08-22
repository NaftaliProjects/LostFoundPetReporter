using LostFoundPetReporter.Mobile.ViewModels;

namespace LostFoundPetReporter.Mobile.Views;

public partial class CreateFoundReportPage : ContentPage
{
    public CreateFoundReportPage(CreateFoundReportViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }
}