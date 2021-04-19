using Realms;
using SmokeFree.Abstraction.Services.General;
using SmokeFree.Bootstrap;
using SmokeFree.Data.Models;
using SmokeFree.Resx;
using System;
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
#if DEBUG
            DevelopmentDatabaseClearing();
#endif

            InitializeComponent();

            LocalizationResourceManager.Current.PropertyChanged += (sender, e)
                => AppResources.Culture = LocalizationResourceManager.Current.CurrentCulture;

            LocalizationResourceManager.Current.Init(AppResources.ResourceManager);

            // TODO: C: Change from settings in the future and initiate current culture
            LocalizationResourceManager.Current.CurrentCulture = new System.Globalization.CultureInfo("uk");   

            InitializeNavigation();
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
