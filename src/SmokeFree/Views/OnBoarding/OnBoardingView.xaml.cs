using SmokeFree.Abstraction.Services.General;
using SmokeFree.Abstraction.Utility.Logging;
using SmokeFree.Bootstrap;
using SmokeFree.ViewModels.ErrorAndEmpty;
using SmokeFree.ViewModels.OnBoarding;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SmokeFree.Views.OnBoarding
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OnBoardingView : ContentPage
    {
        public OnBoardingView()
        {
            InitializeComponent();

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

        /// <summary>
        /// Changes Corousel Item On Button Click - Next
        /// </summary>
        private void OnChangeCarouselItem_Clicked(object sender, EventArgs e)
        {
            // Get Binding Context Carousel Items Collection
            var bindingContext = this.BindingContext as OnBoardingViewModel;
            var totalCarouselElements = bindingContext.OnBoardingItems.Count;

            // Check if element is not the last
            var newPosition = CarouselView.Position + 1;
            if (newPosition != totalCarouselElements)
            {
                // Increment and display next element 
                CarouselView.Position++;
            }

        }
    }
}