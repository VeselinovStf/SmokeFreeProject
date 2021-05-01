using Acr.UserDialogs.Infrastructure;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using System.Threading.Tasks;

namespace SmokeFree.Droid
{
    [Activity(
       Theme = "@style/Theme.Splash",
       MainLauncher = true,
       NoHistory = true,
       Icon = "@drawable/icon")]
    public class SplashScreenActivity : Activity
    {
        static readonly string TAG = "X:" + typeof(SplashScreenActivity).Name;

        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            Window.DecorView.SystemUiVisibility = (StatusBarVisibility)((int)Window.DecorView.SystemUiVisibility ^ (int)SystemUiFlags.LayoutStable ^ (int)SystemUiFlags.LayoutFullscreen);
            Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
            Window.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);
            base.OnCreate(savedInstanceState, persistentState);
            this.StartActivity(typeof(MainActivity));
        }

        // Launches the startup task
        protected override void OnResume()
        {
            base.OnResume();
            Task startupWork = new Task(() => { SimulateStartup(); });
            startupWork.Start();
        }

        // Simulates background work that happens behind the splash screen
        async void SimulateStartup()
        {
            Log.Debug(TAG, "Performing some startup work that takes a bit of time.");
            await Task.Delay(1000); // Simulate a bit of startup work.
            Log.Debug(TAG, "Startup work is finished - starting MainActivity.");
            StartActivity(new Intent(Application.Context, typeof(MainActivity)));
        }
    }
}