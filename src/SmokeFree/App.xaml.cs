using SmokeFree.Bootstrap;
using Xamarin.Forms;

namespace SmokeFree
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            InitializeAppContainer();

            MainPage = new MainPage();
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
