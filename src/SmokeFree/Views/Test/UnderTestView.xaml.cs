
using Realms;
using SmokeFree.Abstraction.Services.Data.Test;
using SmokeFree.Abstraction.Services.General;
using SmokeFree.Abstraction.Utility.Logging;
using SmokeFree.Bootstrap;
using SmokeFree.Data.Models;
using SmokeFree.Resx;
using SmokeFree.ViewModels.ErrorAndEmpty;
using SmokeFree.ViewModels.Test;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SmokeFree.Views.Test
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UnderTestView : ContentPage
    {
        /// <summary>
        /// Stop Testing Timer Cancelation Token
        /// </summary>
        public CancellationTokenSource stopTestingTimerCancellation;

        /// <summary>
        /// Smoking Timer Cancelation Token
        /// </summary>
        public CancellationTokenSource stopSmokingTimerCancellation;

        /// <summary>
        /// TimeSenceLastSmoke Timer Cancelation Token
        /// </summary>
        public CancellationTokenSource stopLastTimeSmokedTimerCancellation;

      
        /// <summary>
        /// Database
        /// </summary>
        private TimeSpan TestLeftTime;

        /// <summary>
        /// Current Smoke Time Display
        /// </summary>
        private TimeSpan CurrentSmokeTime;

        /// <summary>
        /// Timer From last Smoked
        /// </summary>
        private TimeSpan TimeSenceLastSmoke;

        private const string ZeroTimerStr = "00:00:00";

        /// <summary>
        /// Smoking State
        /// </summary>
        private bool IsSmoking;


        private ITestCalculationService _testCalculationService;

        private IAppLogger _appLogger;

        private INavigationService _navigationService;

        private IDialogService _dialogService;

        public UnderTestView()
        {
            InitializeComponent();

           
            _testCalculationService = AppContainer.Resolve<ITestCalculationService>();
            _appLogger = AppContainer.Resolve<IAppLogger>();
            _navigationService = AppContainer.Resolve<INavigationService>();
            _dialogService = AppContainer.Resolve<IDialogService>();
        }

       

        protected override void OnAppearing()
        {
           
            try
            {
                Realm _realm = AppContainer.Resolve<Realm>();
                var user = _realm.Find<User>(Globals.UserId);

                var currentTest = user.Tests.FirstOrDefault(e => e.UserId == user.Id);
                var testCalculation = _testCalculationService
                              .GetCurrentTestDataCalculation(DateTime.Now, currentTest);

                TestLeftTime = testCalculation.TestTimeLeft;
              
                CurrentSmokeTime = testCalculation.CurrentSmokeTime;
                Debug.WriteLine(string.Format("{0:hh\\:mm\\:ss}", CurrentSmokeTime));
                Debug.WriteLine(testCalculation.CurrentSmokedCount);
                Device.BeginInvokeOnMainThread(() =>
                {
                    SmokeCount.Text = testCalculation.CurrentSmokedCount.ToString();
                });

                // Stop Testing Timer Cancelation Token
                this.stopTestingTimerCancellation = new CancellationTokenSource();
                this.stopLastTimeSmokedTimerCancellation = new CancellationTokenSource();
                this.stopSmokingTimerCancellation = new CancellationTokenSource();

                StartTestingTimer();

                if (testCalculation.IsSmoking)
                {
                    IsSmoking = true;

                    StartSmokingTimer(false);
                }
                else
                {
                    if (testCalculation.CurrentSmokedCount > 0)
                    {
                        TimeSenceLastSmoke = _testCalculationService
                            .TimeSinceLastSmoke(currentTest, DateTime.Now);

                        StartLastTimeSmokedTimer();
                    }
                }

                UpdateUI();

            }
            catch (Exception ex)
            {             
                _appLogger.LogCritical(ex);

                _navigationService.NavigateToAsync<SomethingWentWrongViewModel>();
            }
            
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
           
            try
            {
                StopAllTimers();
            }
            catch (Exception ex)
            {
                _appLogger.LogCritical(ex);

                _navigationService.NavigateToAsync<SomethingWentWrongViewModel>();
            }
            

            base.OnDisappearing();
        }

        #region StartTimers

        private void StartSmokingTimer(bool resetUiTimer)
        {
            if (resetUiTimer)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    this.SmokeDateTimeDisplayAtSmokeDateTime.Text = ZeroTimerStr;
                    CurrentSmokeTime = new TimeSpan(0, 0, 0);

                });
            }

            CancellationTokenSource cts = stopSmokingTimerCancellation; // safe copy
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {               
                if (cts.IsCancellationRequested)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        this.SmokeDateTimeDisplayAtSmokeDateTime.Text = ZeroTimerStr;
                        CurrentSmokeTime = new TimeSpan(0, 0, 0);

                    });
                   
                   
                    return false;
                }

                CurrentSmokeTime = CurrentSmokeTime + TimeSpan.FromSeconds(1);

                Device.BeginInvokeOnMainThread(() =>
                {                   
                    this.SmokeDateTimeDisplayAtSmokeDateTime.Text = string.Format("{0:hh\\:mm\\:ss}", CurrentSmokeTime);
                });

                if (this.CurrentSmokeTime.TotalMinutes > Globals.OneSmokeTreshHoldTimeMinutes)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        this.SmokeDateTimeDisplayAtSmokeDateTime.Text = ZeroTimerStr;

                        CurrentSmokeTime = new TimeSpan(0, 0, 0);

                        IsSmoking = false;
                    });                   

                    MessagingCenter.Send<UnderTestView>(this, "DelaySmoke");
      

                    StartLastTimeSmokedTimer();

                    return false;
                }


                return true;

            });          
        }

        private void StartLastTimeSmokedTimer()
        {

            CancellationTokenSource cts = stopLastTimeSmokedTimerCancellation; // safe copy
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                if (cts.IsCancellationRequested)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        this.LastSmokeDateTimeDisplayDateTime.Text = ZeroTimerStr;

                        TimeSenceLastSmoke = new TimeSpan(0, 0, 0);
                    });
                    

                    return false;
                }

                TimeSenceLastSmoke = TimeSenceLastSmoke + TimeSpan.FromSeconds(1);


                Device.BeginInvokeOnMainThread(() =>
                {
                    this.LastSmokeDateTimeDisplayDateTime.Text = string.Format("{0:hh\\:mm\\:ss}", TimeSenceLastSmoke);
                });


                return true;

            });
        }


        private void StartTestingTimer()
        {

            CancellationTokenSource cts = stopTestingTimerCancellation; // safe copy
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                if (cts.IsCancellationRequested)
                {
                    return false;
                }


                this.TestLeftTime = this.TestLeftTime - TimeSpan.FromSeconds(1);

                Device.BeginInvokeOnMainThread(() =>
                {
                    this.Timer.Text = string.Format("{0:hh\\:mm\\:ss}", this.TestLeftTime);
                });

                if (this.TestLeftTime <= new TimeSpan(0, 0, 2))
                {
                    StopAllTimers();

                    IsSmoking = false;

                    MessagingCenter.Send<UnderTestView>(this, "TestCompleted");


                    return false;
                }

                return true;

            });
        }

        #endregion

        #region StopTimers

        /// <summary>
        /// Stops Smoking Timer
        /// </summary>
        private void StopSmokingTimer()
        {
            Interlocked.Exchange(ref stopSmokingTimerCancellation, new CancellationTokenSource()).Cancel();

            Device.BeginInvokeOnMainThread(() =>
            {
                this.SmokeDateTimeDisplayAtSmokeDateTime.Text = ZeroTimerStr;

                CurrentSmokeTime = new TimeSpan(0, 0, 0);
            });
            

        }

        /// <summary>
        /// Stops TimeSenceLastSmoke
        /// </summary>
        private void StopLastTimeSmokedTimer()
        {
            Interlocked.Exchange(ref stopLastTimeSmokedTimerCancellation, new CancellationTokenSource()).Cancel();

            Device.BeginInvokeOnMainThread(() =>
            {
                this.LastSmokeDateTimeDisplayDateTime.Text = ZeroTimerStr;

                TimeSenceLastSmoke = new TimeSpan(0, 0, 0);
            });
            
        }

        /// <summary>
        /// Stops Testing Timer
        /// </summary>
        private void StopTestingTimer()
        {
            Interlocked.Exchange(ref stopTestingTimerCancellation, new CancellationTokenSource()).Cancel();
        }


        private void StopAllTimers()
        {
            StopSmokingTimer();
            StopTestingTimer();
            StopLastTimeSmokedTimer();
        }


        #endregion

        #region ButtonEvents

        private async void Start_Smoking_Clicked(object sender, EventArgs e)
        {

            var userNotification = await _dialogService
                  .ConfirmAsync(AppResources.UnderTestViewModelStartSmokeConfirmMessage,
                  AppResources.UnderTestViewModelStartSmokeConfirmTitle,
                  AppResources.YesButtonText,
                  AppResources.NoButtonText);

            if (userNotification)
            {
                MessagingCenter.Send<UnderTestView>(this, "ExecuteStartSmoking");

                IsSmoking = true;

                this.stopSmokingTimerCancellation = new CancellationTokenSource();

                StartSmokingTimer(true);

                StopLastTimeSmokedTimer();

                UpdateUI();
            }
        }

        private async void Mark_One__Smoke_Clicked(object sender, EventArgs e)
        {
            var userNotification = await _dialogService
                .ConfirmAsync(AppResources.UnderTestViewModelStartSmokeConfirmMessage,
                    AppResources.UnderTestViewModelStartSmokeConfirmTitle,
                    AppResources.YesButtonText,
                    AppResources.NoButtonText);

            if (userNotification)
            {
                MessagingCenter.Send<UnderTestView>(this, "MarkOneSmoked");


                IsSmoking = false;

                Device.BeginInvokeOnMainThread(() =>
                {
                    SmokeCount.Text = (int.Parse(SmokeCount.Text) + 1).ToString();
                });

                StopSmokingTimer();

                this.stopLastTimeSmokedTimerCancellation = new CancellationTokenSource();

                StartLastTimeSmokedTimer();

                UpdateUI();

            }
        }

        private async void Stop_Test_Clicked(object sender, EventArgs e)
        {
            // Check if user is shure
            var userNotification = await _dialogService
                 .ConfirmAsync(AppResources.UnderTestViewModelStopTestMessage,
                 AppResources.UnderTestViewModelRestartTestingLabel,
                 AppResources.ButtonOkText,
                 AppResources.ButtonCancelText);

            if (userNotification)
            {

                StopAllTimers();
                MessagingCenter.Send<UnderTestView>(this, "ExecuteStopTestingCommand");
            
            }         
        }

        #endregion

        #region METHODS

        private void UpdateUI()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                StartSmoking.IsVisible = !IsSmoking;
                MarkOneSmoked.IsVisible = IsSmoking;

                LastSmokeDateTimeDisplayLabel.IsVisible = !IsSmoking;
                LastSmokeDateTimeDisplayDateTime.IsVisible = !IsSmoking;

                SmokeDateTimeDisplayAtSmokeLabel.IsVisible = IsSmoking;
                SmokeDateTimeDisplayAtSmokeDateTime.IsVisible = IsSmoking;
            });
            
        }

        #endregion
    }
}