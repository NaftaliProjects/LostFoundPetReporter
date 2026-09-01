using LostFoundPetReporter.Mobile.Models;
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

            #if ANDROID
            var status = await Permissions.CheckStatusAsync<Permissions.PostNotifications>();

            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.PostNotifications>();
            }
            #endif

            await _viewModel.LoadLostReportsAsync();
        }

        private async void OnRefreshClicked(object? sender, EventArgs e)
        {
            await _viewModel.LoadLostReportsAsync();
        }

        private async void OnLostReportSelected(object? sender, SelectionChangedEventArgs e)
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
}
