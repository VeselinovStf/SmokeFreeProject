using Realms;
using SmokeFree.Abstraction.Services.General;
using SmokeFree.Abstraction.Utility.Logging;
using SmokeFree.Bootstrap;
using SmokeFree.Data.Models;
using SmokeFree.ViewModels.AppSettings;
using SmokeFree.ViewModels.ErrorAndEmpty;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SmokeFree.Views.AppSettings
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ColorSettingsView : ContentView
    {
        public ColorSettingsView()
        {

            InitializeComponent();

            // Bind To App Settings
            BindingContext = SmokeFree.AppLayout.AppSettings.Instance;

            // Create Collection Of Collors
            var globalThemeColors = Globals.AppColorThemes;

            foreach (var color in globalThemeColors)
            {
                var button = new Button
                {
                    Margin = new Thickness(3),
                    HorizontalOptions = LayoutOptions.Center,
                    CornerRadius = 100,
                    BackgroundColor = Color.FromHex(color),

                };

                button.Clicked += (sender, e) =>
                {
                    ApplySettings(sender, e);
                };

                ColorsDisplay.Children.Add(button);
            }

        }

        public void Show()
        {
            IsVisible = true;
            MainContent.FadeTo(1);
            MainContent.TranslateTo(MainContent.TranslationX, 0);
            ShadowView.IsVisible = true;
        }

        public void Hide()
        {
            ShadowView.IsVisible = false;
            var fadeAnimation = new Animation(v => MainContent.Opacity = v, 1, 0);
            var translateAnimation = new Animation(v => MainContent.TranslationY = v, 0, MainContent.Height, null, () => { IsVisible = false; });

            var parentAnimation = new Animation { { 0.5, 1, fadeAnimation }, { 0, 1, translateAnimation } };
            parentAnimation.Commit(this, "HideSettings");
        }

        private void ApplySettings(object sender, EventArgs e)
        {
            INavigationService navigationService = null;
            IAppLogger appLogger = null;

            try
            {
                var currentButton = sender as Button;
                var backgroundColorSelected = currentButton.BackgroundColor.ToHex().ToLower();
                var backgroundColorIndex = Globals.AppColorThemes.IndexOf(backgroundColorSelected.Replace("ff", ""));

                SmokeFree.AppLayout.AppSettings.Instance.SelectedPrimaryColor = backgroundColorIndex;

                MessagingCenter.Send(this, "ColorSettingsView");

                this.Hide();

                var realm = AppContainer.Resolve<Realm>();
                navigationService = AppContainer.Resolve<INavigationService>();
                appLogger = AppContainer.Resolve<IAppLogger>();

                var userId = Globals.UserId;
                var user = realm.Find<User>(userId);

                if (user != null)
                {
                    realm.Write(() =>
                    {
                        user.AppColorThemeIndex = backgroundColorIndex;
                    });
  
                    navigationService.NavigateToAsync<AppSettingsViewModel>();
                }
                else
                {
                    appLogger.LogError($"Can't find User: User Id: {userId}");

                    navigationService.NavigateToAsync<SomethingWentWrongViewModel>();
                }
            }
            catch (Exception ex)
            {
                appLogger.LogError(ex.Message);

                navigationService.NavigateToAsync<SomethingWentWrongViewModel>();
            }
           
        }

        private void Button_OnClicked(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void CloseSettings(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}