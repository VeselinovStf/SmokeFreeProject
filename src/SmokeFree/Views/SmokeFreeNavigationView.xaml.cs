
using SmokeFree.ViewModels.AppSettings;
using SmokeFree.Views.AppSettings;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SmokeFree.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SmokeFreeNavigationView : NavigationPage
    {
        public SmokeFreeNavigationView() : base()
        {
            InitializeComponent();

            MessagingCenter.Subscribe<AppSettingsViewModel>(this, "ColorSettingsView", model => ChangeBarBackgroundColor());
        }

        public SmokeFreeNavigationView(Page root) : base(root)
        {
            InitializeComponent();
        }

        private void ChangeBarBackgroundColor()
        {
            var colorThemes = Globals.AppColorThemes;

            var colorIndex = SmokeFree.AppLayout.AppSettings.Instance.SelectedPrimaryColor;

            var colorHex = colorThemes[colorThemes.Keys.ToList()[colorIndex]];

            //TODO: On Main Thread
            BackgroundColor = Color.FromHex(colorThemes[colorHex]);
        }
    }
}