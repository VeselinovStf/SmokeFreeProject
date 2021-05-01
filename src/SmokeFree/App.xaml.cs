using Realms;
using SmokeFree.Abstraction.Services.General;
using SmokeFree.Bootstrap;
using SmokeFree.Data.Models;
using SmokeFree.Resx;
using System.Globalization;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.Forms;


namespace SmokeFree
{
    public partial class App : Application
    {
        public App()
        {
            InitializeAppContainer();

            // TODO: AA: Remove in Release
            if (Globals.MockRun)
            {
                DevelopmentDatabaseClearing();
            }

            InitializeComponent();

            InitializeLocalization();

            InitializeNavigation();
        }

        private void InitializeLocalization()
        {
            LocalizationResourceManager.Current.PropertyChanged += (sender, e)
                => AppResources.Culture = LocalizationResourceManager.Current.CurrentCulture;


            LocalizationResourceManager.Current.Init(AppResources.ResourceManager);

            var appPreferences = AppContainer.Resolve<IAppPreferencesService>();
            var appLanguage = appPreferences.LanguageValue;

            // TODO: C: Change from settings in the future and initiate current culture
            LocalizationResourceManager.Current.CurrentCulture = appLanguage == string.Empty ? CultureInfo.CurrentCulture : new CultureInfo(appLanguage);
        }

        private void DevelopmentDatabaseClearing()
        {
            var realm = AppContainer.Resolve<Realm>();

            realm.Write(() =>
            {
                realm.RemoveAll<User>();
                realm.RemoveAll<Test>();
                realm.RemoveAll<Challenge>();
                realm.RemoveAll<TestResult>();
                realm.RemoveAll<Smoke>();
                realm.RemoveAll<DayChallengeSmoke>();
            });
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
