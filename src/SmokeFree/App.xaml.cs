using SmokeFree.Abstraction.Services.General;
using SmokeFree.Bootstrap;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SmokeFree
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

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
