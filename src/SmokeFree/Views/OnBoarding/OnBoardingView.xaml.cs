
using Realms;
using SmokeFree.Abstraction.Services.General;
using SmokeFree.Abstraction.Utility.Logging;
using SmokeFree.Bootstrap;
using SmokeFree.Data.Models;
using SmokeFree.ViewModels.ErrorAndEmpty;
using SmokeFree.ViewModels.OnBoarding;
using SmokeFree.Views.AppSettings;
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

            //MessagingCenter.Subscribe<ColorSettingsView>(this, "ColorSettingsView", model => ChangeBarBackgroundColor());
        }

        //TODO: CLEAN
        private void InitializeDefaultColour()
        {
            //INavigationService navigationService = null;
            //IAppLogger appLogger = null;

            //try
            //{
            //    var userId = Globals.UserId;
            //    var realm = AppContainer.Resolve<Realm>();
            //    navigationService = AppContainer.Resolve<INavigationService>();
            //    appLogger = AppContainer.Resolve<IAppLogger>();

            //    var user = realm.Find<User>(userId);

            //    if (user != null)
            //    {

            //        var currentColorIndex = user.AppColorThemeIndex;

            //        var colorThemes = Globals.AppColorThemes;

            //        BackgroundColor = Color.FromHex(colorThemes[currentColorIndex]);
            //    }
            //    else
            //    {
            //        appLogger.LogWarning($"Can't find User! User id {userId}");

            //        var colorThemes = Globals.AppColorThemes;

            //        BackgroundColor = Color.FromHex(colorThemes[0]);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    appLogger.LogError(ex.Message);

            //    navigationService.NavigateToAsync<SomethingWentWrongViewModel>();
            //}
          

        }

        private void ChangeBarBackgroundColor()
        {
            //var colorThemes = Globals.AppColorThemes;

            //var colorIndex = SmokeFree.AppLayout.AppSettings.Instance.SelectedPrimaryColor;

            //BackgroundColor = Color.FromHex(colorThemes[colorIndex]);
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