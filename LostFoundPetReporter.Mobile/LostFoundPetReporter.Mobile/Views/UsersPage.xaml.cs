using LostFoundPetReporter.Mobile.ViewModels;

namespace LostFoundPetReporter.Mobile.Views
{
    public partial class UsersPage : ContentPage
    {
        private readonly UsersViewModel _viewModel;

        public UsersPage(UsersViewModel viewModel)
        {
            InitializeComponent();

            _viewModel = viewModel;

            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await _viewModel.LoadUsersAsync();
        }

        private async void OnRefreshClicked(object? sender, EventArgs e)
        {
            await _viewModel.LoadUsersAsync();
        }
    }
}

