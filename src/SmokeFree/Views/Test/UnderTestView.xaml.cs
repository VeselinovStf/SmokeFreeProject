﻿using SmokeFree.Abstraction.Services.General;
using SmokeFree.Abstraction.Utility.Logging;
using SmokeFree.Bootstrap;
using SmokeFree.Constants.Messages;
using SmokeFree.ViewModels.ErrorAndEmpty;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SmokeFree.Views.Test
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UnderTestView : ContentPage
    {
        public UnderTestView()
        {
            InitializeComponent();
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

        protected override void OnAppearing()
        {
            InitializeDefaultColour();
            MessagingCenter.Send<UnderTestView>(this, MessagingCenterConstant.UnderTestViewAppearing);
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            MessagingCenter.Send<UnderTestView>(this, MessagingCenterConstant.UnderTestViewDisappearing);
            base.OnDisappearing();
        }

        private void OnCloseDescriptionButton_Clicked(object sender, EventArgs e)
        {
            this.Description.IsVisible = false;
            this.ViewContent.IsVisible = true;
        }
    }
}