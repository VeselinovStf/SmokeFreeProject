using SmokeFree.Abstraction.Services.General;
using SmokeFree.Bootstrap;
using SmokeFree.Resx;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.Forms;


namespace SmokeFree
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            LocalizationResourceManager.Current.PropertyChanged += (sender, e)
                => AppResources.Culture = LocalizationResourceManager.Current.CurrentCulture;

            LocalizationResourceManager.Current.Init(AppResources.ResourceManager);

            // TODO: C: Change from settings in the future and initiate current culture
            LocalizationResourceManager.Current.CurrentCulture = new System.Globalization.CultureInfo("uk");

            InitializeAppContainer();

            InitializeNavigation();
        }

        private Task InitializeNavigation()
        {
            var navigationService = AppContainer.Resolve<INavigationService>();
            return navigationService.InitializeAsync();
        }

        private void InitializeAppContainer()
        {
            AppContainer.RegisterDependencies();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
