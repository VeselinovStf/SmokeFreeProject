using SmokeFree.Abstraction.Services.General;
using SmokeFree.Abstraction.Utility.Logging;
using SmokeFree.Bootstrap;
using SmokeFree.ViewModels.ErrorAndEmpty;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SmokeFree.Views.Challenge
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CompletedChallengeView : ContentPage
    {
        public CompletedChallengeView()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            InitializeDefaultColour();
        }

        private void InitializeDefaultColour()
        {
            try
            {
                var appPreferences = AppContainer.Resolve<IAppPreferencesService>();

                var currentColorIndex = appPreferences.ColorKey;

                var colorThemes = Globals.AppColorThemes;

                BackgroundColor = Color.FromHex(colorThemes[currentColorIndex]);
            }
            catch (Exception ex)
            {
                var navigationService = AppContainer.Resolve<INavigationService>();
                var appLogger = AppContainer.Resolve<IAppLogger>();

                appLogger.LogError(ex.Message);

                navigationService.NavigateToAsync<SomethingWentWrongViewModel>();
            }
        }

        private void OnCloseDescriptionButton_Clicked(object sender, EventArgs e)
        {
            this.Description.IsVisible = false;
            this.ViewContent.IsVisible = true;
        }
    }
}