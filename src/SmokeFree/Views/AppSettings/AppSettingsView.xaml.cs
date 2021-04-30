
using System;
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

            MessagingCenter.Subscribe<ColorSettingsView>(this, "ColorSettingsView", model => ChangeBarBackgroundColor());
        }

        private void ChangeBarBackgroundColor()
        {
            var colorThemes = Globals.AppColorThemes;

            var colorIndex = SmokeFree.AppLayout.AppSettings.Instance.SelectedPrimaryColor;

            BackgroundColor = Color.FromHex(colorThemes[colorIndex]);
        }

        private void ShowSettings(object sender, EventArgs e)
        {
            //ColorSettings.Show();
        }

        private void ShowLanguages(object sender, EventArgs e)
        {
           this.picker.Focus();
        }
    }
}