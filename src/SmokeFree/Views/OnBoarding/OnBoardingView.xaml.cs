
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

            MessagingCenter.Subscribe<ColorSettingsView>(this, "ColorSettingsView", model => ChangeBarBackgroundColor());
        }

        private void InitializeDefaultColour()
        {

            var bindingUser = this.BindingContext as OnBoardingViewModel;

            var currentColorIndex = bindingUser.AppUser.AppColorThemeIndex;

            var colorThemes = Globals.AppColorThemes;

            BackgroundColor = Color.FromHex(colorThemes[currentColorIndex]);

        }

        private void ChangeBarBackgroundColor()
        {
            var colorThemes = Globals.AppColorThemes;

            var colorIndex = SmokeFree.AppLayout.AppSettings.Instance.SelectedPrimaryColor;

            BackgroundColor = Color.FromHex(colorThemes[colorIndex]);
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