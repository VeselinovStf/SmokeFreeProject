using SmokeFree.Abstraction.Services.General;
using SmokeFree.Abstraction.Utility.Logging;
using SmokeFree.Bootstrap;
using SmokeFree.ViewModels.ErrorAndEmpty;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SmokeFree.Views.AppSettings
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppSettingsView : ContentPage
    {
        public AppSettingsView()
        {
            InitializeComponent();
            InitializeDefaultColour();
            InitializeColorPicker();
        }

        //private void InitializeDefaultColour()
        //{
        //    try
        //    {
        //        var appPreferences = AppContainer.Resolve<IAppPreferencesService>();

        //        var currentColorIndex = appPreferences.ColorKey;

        //        var colorThemes = Globals.AppColorThemes;

        //        BackgroundColor = Color.FromHex(colorThemes[currentColorIndex]);
        //    }
        //    catch (Exception ex)
        //    {
        //        var navigationService = AppContainer.Resolve<INavigationService>();
        //        var appLogger = AppContainer.Resolve<IAppLogger>();

        //        appLogger.LogError(ex.Message);

        //        navigationService.NavigateToAsync<SomethingWentWrongViewModel>();
        //    }
        //}

        //private void InitializeColorPicker()
        //{
        //    var colors = Globals.AppColorThemes;

        //    var colorKeys = new ObservableCollection<string>();
        //    foreach (var c in colors)
        //    {
        //        colorKeys.Add(c.Key);
        //    }

        //    ColorPicker.ItemsSource = colorKeys;

        //    ColorPicker.PropertyChanged += (sender, args) =>
        //    {
        //        var selectedIndeElement = (string)ColorPicker.SelectedItem;

        //        if (!string.IsNullOrWhiteSpace(selectedIndeElement))
        //        {
        //            var selectedIndex = colors.Keys.ToList().IndexOf(selectedIndeElement);

        //            try
        //            {
        //                SmokeFree.AppLayout.AppSettings.Instance.SelectedPrimaryColor = selectedIndex;

        //                var appPreferencesService = AppContainer.Resolve<IAppPreferencesService>();

        //                appPreferencesService.ColorKey = selectedIndeElement;

        //                var colorHex = colors[colors.Keys.ToList()[selectedIndex]];

        //                BackgroundColor = Color.FromHex(colorHex);

        //            }
        //            catch (Exception ex)
        //            {
        //                var appLoger = AppContainer.Resolve<IAppLogger>();


        //                appLoger.LogError(ex.Message);

        //                var navigationService = AppContainer.Resolve<INavigationService>();
        //                navigationService.NavigateToAsync<SomethingWentWrongViewModel>();
        //            }
        //        }


        //    };
        //}

        //private void ChangeBarBackgroundColor()
        //{
        //    //var colorThemes = Globals.AppColorThemes;

        //    //var colorIndex = SmokeFree.AppLayout.AppSettings.Instance.SelectedPrimaryColor;

        //    //BackgroundColor = Color.FromHex(colorThemes[colorIndex]);
        //}

        //private void ShowColorSettings(object sender, EventArgs e)
        //{
        //    // ColorPicker.Focus();
        //}

        //private void ShowLanguages(object sender, EventArgs e)
        //{
        //    // this.picker.Focus();
        //}

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

        private void InitializeColorPicker()
        {
            var colors = Globals.AppColorThemes;

            foreach (var c in colors)
            {
                ColorPicker.Items.Add(c.Key);
            }

            ColorPicker.SelectedIndexChanged += (sender, args) =>
            {
                var selectedIndex = ColorPicker.SelectedIndex;

                if (selectedIndex != -1)
                {
                    try
                    {
                        SmokeFree.AppLayout.AppSettings.Instance.SelectedPrimaryColor = selectedIndex;

                        var appPreferencesService = AppContainer.Resolve<IAppPreferencesService>();

                        appPreferencesService.ColorKey = ColorPicker.Items[selectedIndex];

                        var colorHex = colors[colors.Keys.ToList()[selectedIndex]];

                        BackgroundColor = Color.FromHex(colorHex);

                    }
                    catch (Exception ex)
                    {
                        var appLoger = AppContainer.Resolve<IAppLogger>();


                        appLoger.LogError(ex.Message);

                        var navigationService = AppContainer.Resolve<INavigationService>();
                        navigationService.NavigateToAsync<SomethingWentWrongViewModel>();
                    }
                }
            };
        }

        private void ChangeBarBackgroundColor()
        {
            var colorThemes = Globals.AppColorThemes;

            var colorIndex = SmokeFree.AppLayout.AppSettings.Instance.SelectedPrimaryColor;

            BackgroundColor = Color.FromHex(colorThemes[colorThemes.Keys.ToList()[colorIndex]]);
        }

        private void ShowColorSettings(object sender, EventArgs e)
        {
            ColorPicker.Focus();
        }

        private void ShowLanguages(object sender, EventArgs e)
        {
            this.picker.Focus();
        }
    }
}