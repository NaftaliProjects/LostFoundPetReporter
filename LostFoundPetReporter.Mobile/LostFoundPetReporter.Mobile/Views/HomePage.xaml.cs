using LostFoundPetReporter.Mobile.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace LostFoundPetReporter.Mobile.Views
{
    public partial class HomePage : ContentPage
    {
        private readonly HomeViewModel _viewModel;

        public HomePage(HomeViewModel viewModel)
        {
            InitializeComponent();

            _viewModel = viewModel;

            BindingContext = _viewModel;

        }


        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await _viewModel.LoadLostReportsAsync();
        }

        private async void OnRefreshClicked(object? sender, EventArgs e)
        {
            await _viewModel.LoadLostReportsAsync();
        }

    }
}
