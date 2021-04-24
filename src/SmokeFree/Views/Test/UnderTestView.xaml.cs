
using Realms;
using SmokeFree.Abstraction.Services.Data.Test;
using SmokeFree.Abstraction.Services.General;
using SmokeFree.Abstraction.Utility.Logging;
using SmokeFree.Bootstrap;
using SmokeFree.Data.Models;
using SmokeFree.ViewModels.ErrorAndEmpty;
using SmokeFree.ViewModels.Test;
using System;
using System.Linq;
using System.Threading;
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

        /// <summary>
        /// Smoking State
        /// </summary>
        private bool IsSmoking;

        public UnderTestView()
        {
            InitializeComponent();
        }

       

        protected override void OnAppearing()
        {
            var appLogger = AppContainer.Resolve<IAppLogger>();

            try
            {              
                var realm = AppContainer.Resolve<Realm>();
                var user = realm.Find<User>(Globals.UserId);

                var currentTest = user.Tests.FirstOrDefault(e => e.UserId == user.Id);
                var testCalculationService = AppContainer.Resolve<ITestCalculationService>();
                var testCalculation = testCalculationService
                              .GetCurrentTestDataCalculation(DateTime.Now, currentTest);

                TestLeftTime = testCalculation.TestTimeLeft;
                CurrentSmokeTime = testCalculation.CurrentSmokeTime;
                SmokeCount.Text = testCalculation.CurrentSmokedCount.ToString();
                

                // Stop Testing Timer Cancelation Token
                this.stopTestingTimerCancellation = new CancellationTokenSource();
                this.stopLastTimeSmokedTimerCancellation = new CancellationTokenSource();
                this.stopSmokingTimerCancellation = new CancellationTokenSource();

                StartTestingTimer();

                if (testCalculation.IsSmoking)
                {
                    IsSmoking = true;

                    StartSmokingTimer();
                }
                else
                {
                    // View is called with un-finished smoke -> add to TimeSenceLastSmoke
                    //if (currentTest.SmokedCigaresUnderTest.Any(e => !e.StartSmokeTime.Equals(new DateTimeOffset()) && e.EndSmokeTime.Equals(new DateTimeOffset())))
                    //{
                    if (testCalculation.CurrentSmokedCount > 0)
                    {
                        TimeSenceLastSmoke = testCalculationService
                            .TimeSinceLastSmoke(currentTest, DateTime.Now);

                        StartLastTimeSmokedTimer();
                    }
                        
                    //}
                }

                UpdateUI();

            }
            catch (Exception ex)
            {
                var navigationService = AppContainer.Resolve<INavigationService>();

                appLogger.LogCritical(ex);

                navigationService.NavigateToAsync<SomethingWentWrongViewModel>();
            }
            
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            var appLogger = AppContainer.Resolve<IAppLogger>();

            try
            {
                StopAllTimers();
                //StopTestingTimer();

                //if (IsSmoking)
                //{
                //    StopSmokingTimer();
                //}
                //else
                //{
                //    if (TimeSenceLastSmoke >= TimeSpan.FromSeconds(1))
                //    {
                //        StopLastTimeSmokedTimer();
                //    }
                //}
            }
            catch (Exception ex)
            {
                var navigationService = AppContainer.Resolve<INavigationService>();

                appLogger.LogCritical(ex);

                navigationService.NavigateToAsync<SomethingWentWrongViewModel>();
            }
            

            base.OnDisappearing();
        }

        #region StartTimers

        private void StartSmokingTimer()
        {

            CancellationTokenSource cts = stopSmokingTimerCancellation; // safe copy
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {               
                if (cts.IsCancellationRequested)
                {
                    return false;
                }

                CurrentSmokeTime = CurrentSmokeTime + TimeSpan.FromSeconds(1);

                Device.BeginInvokeOnMainThread(() =>
                {                   
                    this.SmokeDateTimeDisplayAtSmokeDateTime.Text = string.Format("{0:hh\\:mm\\:ss}", CurrentSmokeTime);
                });

                if (this.CurrentSmokeTime.TotalMinutes > Globals.OneSmokeTreshHoldTimeMinutes)
                {
                    MessagingCenter.Send<UnderTestView>(this, "DelaySmoke");

                    IsSmoking = false;

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

            CurrentSmokeTime = new TimeSpan(0, 0, 0);
        }

        /// <summary>
        /// Stops TimeSenceLastSmoke
        /// </summary>
        private void StopLastTimeSmokedTimer()
        {
            Interlocked.Exchange(ref stopLastTimeSmokedTimerCancellation, new CancellationTokenSource()).Cancel();

            TimeSenceLastSmoke = new TimeSpan(0, 0, 0);
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

        private void Start_Smoking_Clicked(object sender, EventArgs e)
        {
            IsSmoking = true;

            this.stopSmokingTimerCancellation = new CancellationTokenSource();

            StartSmokingTimer();

            StopLastTimeSmokedTimer();

            UpdateUI();
        }

        private void Mark_One__Smoke_Clicked(object sender, EventArgs e)
        {
            IsSmoking = false;

            SmokeCount.Text = (int.Parse(SmokeCount.Text) + 1).ToString();

            StopSmokingTimer();

            this.stopLastTimeSmokedTimerCancellation = new CancellationTokenSource();

            StartLastTimeSmokedTimer();

            UpdateUI();

        }

        private void Stop_Test_Clicked(object sender, EventArgs e)
        {
            StopAllTimers();
        }

        #endregion

        #region METHODS

        private void UpdateUI()
        {
            StartSmoking.IsVisible = !IsSmoking;
            MarkOneSmoked.IsVisible = IsSmoking;

            LastSmokeDateTimeDisplayLabel.IsVisible = !IsSmoking;
            LastSmokeDateTimeDisplayDateTime.IsVisible = !IsSmoking;

            SmokeDateTimeDisplayAtSmokeLabel.IsVisible = IsSmoking;
            SmokeDateTimeDisplayAtSmokeDateTime.IsVisible = IsSmoking;
        }

        #endregion
    }
}