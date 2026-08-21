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
}