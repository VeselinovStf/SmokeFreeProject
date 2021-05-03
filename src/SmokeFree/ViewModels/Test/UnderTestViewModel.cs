
using Plugin.LocalNotification;
using Realms;
using SmokeFree.Abstraction.Services.Data.Test;
using SmokeFree.Abstraction.Services.General;
using SmokeFree.Abstraction.Utility.DeviceUtilities;
using SmokeFree.Abstraction.Utility.Logging;
using SmokeFree.Abstraction.Utility.Wrappers;
using SmokeFree.Constants.Messages;
using SmokeFree.Data.Models;
using SmokeFree.Resx;
using SmokeFree.ViewModels.AppSettings;
using SmokeFree.ViewModels.Base;
using SmokeFree.Views.Test;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace SmokeFree.ViewModels.Test
{
    /// <summary>
    /// UnderTestView Model
    /// </summary>
    public class UnderTestViewModel : ViewModelBase
    {
        #region FIELDS


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
        public CancellationTokenSource stopTimeSenceLastSmokeTimerCancellation;

        /// <summary>
        /// Curent Smoked Count
        /// </summary>
        private int _currentlySmokedCount;

        /// <summary>
        /// Timer From last Smoked
        /// </summary>
        private TimeSpan _timeSenceLastSmoke;

        /// <summary>
        /// Test Left Time Indicator
        /// </summary>
        private TimeSpan _testLeftTime;

        /// <summary>
        /// Smoking State
        /// </summary>
        private bool _isSmoking;

        /// <summary>
        /// Current Smoke Id
        /// </summary>
        private string _currentSmokeId;

        /// <summary>
        /// Current Smoke Time Display
        /// </summary>
        private TimeSpan _currentSmokeTime;

        /// <summary>
        /// Database
        /// </summary>
        private readonly Realm _realm;

        /// <summary>
        /// Test Calculations Service Abstraction
        /// </summary>
        private readonly ITestCalculationService _testCalculationService;

        /// <summary>
        /// Device Timer Abstraction
        /// </summary>
        private readonly IDeviceTimer _deviceTimer;

        #endregion

        #region CTOR

        public UnderTestViewModel(
            Realm realm,
            INavigationService navigationService,
            IDateTimeWrapper dateTimeWrapper,
            IAppLogger appLogger,
            IDialogService dialogService,
            ITestCalculationService testCalculationService,
            IDeviceTimer deviceTimer) : base(navigationService, dateTimeWrapper, appLogger, dialogService)
        {
            // Set View Title
            ViewTitle = AppResources.UnderTestViewTiitle;

            // Database
            _realm = realm;

            // Test Calculation Service
            _testCalculationService = testCalculationService;

            // Device Timer
            _deviceTimer = deviceTimer;

            // Initial Subscribe-UnSubscribe to OnAppearing of View Model
            MessagingCenter.Subscribe<UnderTestView>(this, MessagingCenterConstant.UnderTestViewAppearing, async (e) =>
            {
                await AppearingInitializeAsync();

                MessagingCenter.Unsubscribe<UnderTestView>(this, MessagingCenterConstant.UnderTestViewAppearing);
            });
        }

        #endregion

        #region INIT

        /// <summary>
        /// Initialize View State
        /// Called Initialy and if app is re-oppend
        /// </summary>
        /// <returns>Base Initialize Async</returns>
        public async Task AppearingInitializeAsync()
        {
            try
            {
                // Stop Testing Timer Cancelation Token
                this.stopTestingTimerCancellation = new CancellationTokenSource();

                // Smoking Timer Cancelation Token
                this.stopSmokingTimerCancellation = new CancellationTokenSource();

                // TimeSenceLastSmoke Timer Cancelation Token
                this.stopTimeSenceLastSmokeTimerCancellation = new CancellationTokenSource();

                // Get User From DB
                var userId = Globals.UserId;
                var user = this._realm.Find<User>(userId);

                // Validate User
                if (user != null)
                {
                    // Get Current User Test
                    var currentUserTestId = user.TestId;
                    var currentTest = user.Tests
                        .FirstOrDefault(e => e.Id == currentUserTestId && !e.IsDeleted);

                    // Validate Test
                    if (currentTest != null)
                    {
                        // Calculate Test 
                        var testCalculation = this._testCalculationService
                            .GetCurrentTestDataCalculation(_dateTime.Now(), currentTest);

                        // Assignm properties
                        this.CurrentlySmokedCount = testCalculation.CurrentSmokedCount;
                        this.TimeSenceLastSmoke = testCalculation.TimeSinceLastSmoke;
                        this.TestLeftTime = testCalculation.TestTimeLeft;

                        // Set value of currently smoked or string.Empty 
                        // if user is smoking for first time
                        this.CurrentSmokeId = testCalculation.CurrentSmokeId;
                        this.CurrentSmokeTime = testCalculation.CurrentSmokeTime;

                        // Check if is smoking
                        if (testCalculation.IsSmoking)//testCalculation.CurrentSmokeTime > TimeSpan.FromSeconds(1))
                        {
                            this.IsSmoking = true;

                            StartSmokingTimer();
                        }
                        else
                        {
                            // View is called with un-finished smoke -> add to TimeSenceLastSmoke
                            //if (currentTest.SmokedCigaresUnderTest.Any(e => !e.StartSmokeTime.Equals(new DateTimeOffset()) && e.EndSmokeTime.Equals(new DateTimeOffset())))
                            //{
                            if (currentTest.SmokedCigaresUnderTest.Count > 0)
                            {
                                this.IsSmoking = false;

                                this.TimeSenceLastSmoke = this._testCalculationService
                                    .TimeSinceLastSmoke(currentTest, this._dateTime.Now());

                                StartTimeSenceLastSmokeTimer();
                            }

                            //}

                        }

                        // Start Device Cound Down for Test Left Time

                        // TODO: A: ATTENTION In fot the end of testing
                        StartTestintTimer();

                        // Initial Subscribe-UnSubscribe to OnDesapearing of View Model
                        MessagingCenter.Subscribe<UnderTestView>(this, MessagingCenterConstant.UnderTestViewDisappearing, (e) =>
                        {
                            DesapearingInitializeAsync();

                            MessagingCenter.Unsubscribe<UnderTestView>(this, MessagingCenterConstant.UnderTestViewDisappearing);
                        });
                    }
                    else
                    {
                        // User Test Not Found!
                        base._appLogger.LogCritical($"Can't find User Test: User Id: {userId} , Test Id: {currentUserTestId}");

                        await base.InternalErrorMessageToUser();
                    }
                }
                else
                {
                    // User Not Found!
                    base._appLogger.LogCritical($"Can't find User: {userId}");

                    await base.InternalErrorMessageToUser();
                }
            }
            catch (Exception ex)
            {
                base._appLogger.LogCritical(ex);

                await base.InternalErrorMessageToUser();
            }
        }

        public void DesapearingInitializeAsync()
        {
            StopSmokingTimer();
            StopTestingTimer();
            StopTimeSenceLastSmokeTimer();

            // Post Subscribe-UnSubscribe to OnAppearing of View Model
            MessagingCenter.Subscribe<UnderTestView>(this, MessagingCenterConstant.UnderTestViewAppearing, async (e) =>
            {
                await AppearingInitializeAsync();

                MessagingCenter.Unsubscribe<UnderTestView>(this, MessagingCenterConstant.UnderTestViewAppearing);
            });

        }

        #endregion

        #region COMMANDS

        /// <summary>
        /// Async Start Testing Command and Logic
        /// </summary>
        public IAsyncCommand OnStopTestingCommand => new AsyncCommand(
            ExecuteStopTestingCommand,
            onException: base.GenericCommandExeptionHandler,
            allowsMultipleExecutions: false);

        private async Task ExecuteStopTestingCommand()
        {
            try
            {
                // Check if user is shure
                var userNotification = await base._dialogService
                     .ConfirmAsync(AppResources.UnderTestViewModelStopTestMessage,
                     AppResources.UnderTestViewModelRestartTestingLabel,
                     AppResources.ButtonOkText,
                     AppResources.ButtonCancelText);

                if (userNotification)
                {
                    // Call Stop Testing
                    await StopTesting();

                    // Navigate to Create Test
                    await this._navigationService.NavigateToAsync<CreateTestViewModel>();
                    //TODO: B: Clear navigation stack

                }
            }
            catch (Exception ex)
            {
                base._appLogger.LogCritical(ex);

                await base.InternalErrorMessageToUser();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private async Task StopTesting()
        {
            // Get Current User
            var userId = Globals.UserId;
            var user = _realm.Find<User>(userId);

            // Validate User
            if (user != null)
            {
                var currentTestId = user.TestId;

                // Delete Current Test Information From DB
                var userTest = user.Tests.FirstOrDefault(t => t.UserId == userId && !t.IsDeleted);
                var testChallenge = user.Challenges.FirstOrDefault(c => c.UserId == userId && !c.IsDeleted);
                // 

                _realm.Write(() =>
                {
                    // Remove Test
                    userTest.IsDeleted = true;
                    userTest.DeletedOn = this._dateTime.Now();
                    userTest.ModifiedOn = this._dateTime.Now();

                    // Remove smoked cigares if persist
                    if (userTest.SmokedCigaresUnderTest.Count() > 0)
                    {
                        foreach (var smoke in userTest.SmokedCigaresUnderTest)
                        {
                            smoke.IsDeleted = true;
                            smoke.DeletedOn = this._dateTime.Now();
                            smoke.ModifiedOn = this._dateTime.Now();
                        }
                    }

                    // Remove Challenge
                    testChallenge.IsDeleted = true;
                    testChallenge.DeletedOn = this._dateTime.Now();
                    testChallenge.ModifiedOn = this._dateTime.Now();

                    // Update User Status
                    user.UserState = UserStates.CompletedOnBoarding.ToString();
                    user.TestId = string.Empty;
                });

                // Stop Testing Timer
                StopTestingTimer();
                StopTimeSenceLastSmokeTimer();
                StopSmokingTimer();

                // Stop Testing Time Notification
                NotificationCenter.Current.Cancel(Globals.TestingTimeNotificationId);

                // Stop Smoke Delayed Time Notification
                NotificationCenter.Current.Cancel(Globals.DelayedSmokeNotificationId);

            }
            else
            {
                // User Not Found!
                base._appLogger.LogCritical($"Can't find User: User Id {userId}");

                await base.InternalErrorMessageToUser();
            }
        }

        /// <summary>
        /// Navigate to settings View
        /// </summary>
        public IAsyncCommand OnSettingsCommand => new AsyncCommand(
            ExecuteNavigateToSetting,
            onException: base.GenericCommandExeptionHandler,
            allowsMultipleExecutions: false);

        private async Task ExecuteNavigateToSetting()
        {
            try
            {
                await base._navigationService.NavigateToAsync<AppSettingsViewModel>();
            }
            catch (Exception ex)
            {
                base._appLogger.LogCritical(ex);

                await base.InternalErrorMessageToUser();
            }

        }

        /// <summary>
        /// Start Smoking
        /// </summary>
        public IAsyncCommand<bool> StartSmokingCommand => new AsyncCommand<bool>(isSmoking =>
           ExecuteStartSmoking(isSmoking),
            onException: base.GenericCommandExeptionHandler,
            allowsMultipleExecutions: false);

        private async Task ExecuteStartSmoking(bool isSmoking)
        {
            try
            {
                // Check User Notification
                var userNotification = await base._dialogService
                    .ConfirmAsync(AppResources.UnderTestViewModelStartSmokeConfirmMessage,
                    AppResources.UnderTestViewModelStartSmokeConfirmTitle,
                    AppResources.YesButtonText,
                    AppResources.NoButtonText);

                if (userNotification)
                {
                    // Get Current User
                    var userId = Globals.UserId;
                    var user = _realm.Find<User>(userId);

                    // Validate User
                    if (user != null)
                    {
                        // Get User Test
                        var currentTestId = user.TestId;
                        var userTest = user.Tests
                            .FirstOrDefault(u => u.UserId == user.Id && !u.IsDeleted);

                        // Validate Test
                        if (userTest != null)
                        {
                            // Check for un-finished smokes
                            var notCompleteSmokes = userTest.SmokedCigaresUnderTest
                                .Where(e => e.EndSmokeTime.Equals(new DateTimeOffset()) && !e.IsDeleted);

                            if (notCompleteSmokes.Count() > 0)
                            {
                                // Invalid App State
                                base._appLogger.LogCritical($"In Test : {currentTestId}, User Id: {userId}, was detected un finished smoke! Invalid App State!");

                                foreach (var notCompleteSmoke in notCompleteSmokes)
                                {
                                    // Slow operation, but app state must be correct!
                                    this._realm.Write(() =>
                                    {
                                        notCompleteSmoke.EndSmokeTime = this._dateTime.Now();
                                        notCompleteSmoke.ModifiedOn = this._dateTime.Now();
                                    });
                                }
                            }

                            var newSmoke = new Smoke()
                            {
                                Id = Guid.NewGuid().ToString(),
                                CreatedOn = this._dateTime.Now(),
                                StartSmokeTime = this._dateTime.Now(),
                                TestId = currentTestId
                            };

                            // Add One Smoked to Test
                            this._realm.Write(() =>
                            {
                                userTest.SmokedCigaresUnderTest.Add(newSmoke);
                            });

                            // Setup VM Props
                            this.CurrentSmokeId = newSmoke.Id;
                            this.IsSmoking = true;
                            this.CurrentSmokeTime = new TimeSpan(0, 0, 0);

                            // Start Smoking Timer
                            StartSmokingTimer();

                            StopTimeSenceLastSmokeTimer();

                            if (user.NotificationState)
                            {
                                // Register Notification
                                var delaySmokeNotification = new NotificationRequest
                                {

                                    NotificationId = Globals.DelayedSmokeNotificationId,
                                    Title = AppResources.UnderTestViewModelOneSmokeTreshHoldNotificationTitle,
                                    Description = AppResources.UnderTestViewModelOneSmokeTreshHoldNotificationMessage,
                                    ReturningData = "Dummy data", // Returning data when tapped on notification.
                                    NotifyTime = DateTime.Now.AddMinutes(Globals.OneSmokeTreshHoldTimeMinutes),
                                    Android = new AndroidOptions()
                                    {
                                        IconName = "icon"
                                    } // Used for Scheduling local notification, if not specified notification will show immediately.
                                };

                                NotificationCenter.Current.Show(delaySmokeNotification);
                            }

                        }
                        else
                        {
                            // User Test Not Found!
                            base._appLogger.LogCritical($"Can't find User Test: User Id: {userId} , Test Id: {currentTestId}");

                            await base.InternalErrorMessageToUser();
                        }

                    }
                    else
                    {
                        // User Not Found!
                        base._appLogger.LogCritical($"Can't find User: User Id {userId}");

                        await base.InternalErrorMessageToUser();
                    }
                }
            }
            catch (Exception ex)
            {
                base._appLogger.LogCritical(ex);

                await base.InternalErrorMessageToUser();
            }
        }


        /// <summary>
        /// Mark One Smoked
        /// </summary>
        public IAsyncCommand<bool> MarkOneSmokedCommand => new AsyncCommand<bool>(isSmoking =>
           MarkOneSmoked(isSmoking),
            onException: base.GenericCommandExeptionHandler,
            allowsMultipleExecutions: false);

        private async Task MarkOneSmoked(bool isSmoking)
        {
            // Check User Notification
            var userNotification = await base._dialogService
                .ConfirmAsync(AppResources.UnderTestViewModelStopSmokeConfirmMessage,
                    AppResources.UnderTestViewModelStopSmokeConfirmTitle,
                    AppResources.YesButtonText,
                    AppResources.NoButtonText);

            if (userNotification)
            {
                await SmokeOneCigareAsync();
            }
        }

        #endregion

        // TODO: B: Add notification for app inner work! Use IsBusy for methods
        #region METHODS

        /// <summary>
        /// Increment TimeSenceLastSmoke
        /// </summary>
        private void StartTimeSenceLastSmokeTimer()
        {
            _deviceTimer
                .Start(() =>
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        this.TimeSenceLastSmoke += TimeSpan.FromSeconds(1);

                    });


                    return true;

                }, this.stopTimeSenceLastSmokeTimerCancellation);
        }

        /// <summary>
        /// Starts Testing Time
        /// </summary>
        private void StartTestintTimer()
        {
            _deviceTimer
                .Start(() =>
               {
                   // Invcoke TestLeftTime UI changes on main thread
                   Device.BeginInvokeOnMainThread(() =>
                  {
                      this.TestLeftTime = this.TestLeftTime - TimeSpan.FromSeconds(1);
                  });

                   if (this.TestLeftTime <= new TimeSpan(0, 0, 2))
                   {
                       // TODO: C: Add loader to view
                       base.IsBusy = true;

                       // Execute Function for stop testing
                       MarkTestCompletedAsync();
                       CreateTestResultAsync();

                       StopTestingTimer();

                       base.IsBusy = false;

                       return false;
                   }

                   return true;

               }, this.stopTestingTimerCancellation);
        }

        /// <summary>
        /// Starts Smoking Timer for each new smoke
        /// </summary>
        private void StartSmokingTimer()
        {
            _deviceTimer
                .Start(() =>
               {
                   Device.BeginInvokeOnMainThread(() =>
                   {
                       this.CurrentSmokeTime += TimeSpan.FromSeconds(1);

                   });


                   if (this.CurrentSmokeTime.TotalMinutes > Globals.OneSmokeTreshHoldTimeMinutes)
                   {
                       // TODO: C: Add loader to view
                       base.IsBusy = true;

                       MarkSmokedAfterDelayLimitAsync();

                       base.IsBusy = false;

                       return false;
                   }

                   return true;

               }, this.stopSmokingTimerCancellation);
        }

        /// <summary>
        /// Create Current Test Results
        /// </summary>
        public async void CreateTestResultAsync()
        {
            try
            {
                // Get user
                var userId = Globals.UserId;
                var user = _realm.Find<User>(userId);

                // Validate User
                if (user != null)
                {
                    var testId = user.TestId;
                    var userTest = user.Tests
                        .FirstOrDefault(t => t.Id == testId);

                    // Validate User Test
                    if (userTest != null)
                    {
                        if (userTest.SmokedCigaresUnderTest.Count < 1)
                        {
                            await this._dialogService
                            .ShowDialog(
                                AppResources.UnderTestViewModelCreateTestResultErrorMessage,
                                AppResources.UnderTestViewModelCreateTestResultErrorTitle,
                                AppResources.ButtonOkText);

                            await StopTesting();

                            // Navigate to Create Test
                            await this._navigationService.NavigateToAsync<CreateTestViewModel>();
                            //TODO: B: Clear navigation stack
                        }
                        else
                        {
                            // Calculate Test Results
                            var testResultCalculation = this._testCalculationService
                                .CalculateTestResult(userTest);

                            // Validate Test Results
                            if (testResultCalculation.Success)
                            {
                                // Setup Test Result
                                var testResult = testResultCalculation.TestResultCalculation;
                                testResult.TestId = testId;
                                testResult.CreatedOn = this._dateTime.Now();

                                var userStatusCalculation = this._testCalculationService
                                            .CalculateUserSmokeStatus(testResult);

                                if (userStatusCalculation.Success)
                                {
                                    // Write To db
                                    this._realm.Write(() =>
                                    {
                                        user.UserSmokeStatuses = userStatusCalculation.Status.ToString();
                                        userTest.CompletedTestResult = testResult;
                                        user.UserState = UserStates.IsTestComplete.ToString();
                                    });

                                    NavigateToTestResults();
                                }
                                else
                                {
                                    // User Test Result Not Found!
                                    base._appLogger.LogCritical($"Can't Calculate User Smoke Status: User id: {userId}, Test Id {testId}");

                                    await base.InternalErrorMessageToUser();
                                }
                               
                            }
                            else
                            {
                                // Cand Do Test Result Calculations
                                base._appLogger.LogCritical($"Can't Calculate Test Results: User id: {userId}, Test Id {testId}, Reason: {testResultCalculation.Message}");

                                await base.InternalErrorMessageToUser();
                            }
                        }

                    }
                    else
                    {
                        // User Not Found!
                        base._appLogger.LogCritical($"Can't find User Test: User id: {userId}, Test Id {testId}");

                        await base.InternalErrorMessageToUser();
                    }
                }
                else
                {
                    // User Not Found!
                    base._appLogger.LogCritical($"Can't find User: User Id {userId}");

                    await base.InternalErrorMessageToUser();
                }
            }
            catch (Exception ex)
            {
                base._appLogger.LogCritical(ex);

                await base.InternalErrorMessageToUser();
            }
        }

        /// <summary>
        /// Navigate to next view
        /// </summary>
        private async void NavigateToTestResults()
        {
            await base._navigationService.NavigateToAsync<TestResultViewModel>();
        }


        /// <summary>
        /// Marks Current test completed
        /// </summary>
        public async void MarkTestCompletedAsync()
        {
            try
            {
                // Get user
                var userId = Globals.UserId;
                var user = _realm.Find<User>(userId);

                // Validate User
                if (user != null)
                {
                    // Get Current State
                    var currentTestId = user.TestId;
                    var currentTest = user
                        .Tests
                        .FirstOrDefault(t => t.Id == currentTestId);

                    // Validate Test
                    if (currentTest != null)
                    {
                        // Validate App State
                        if (currentTest.IsCompleted)
                        {
                            base._appLogger.LogCritical($"User Test is with flag Completed! But was not completed at a time! App State is Compromised!");
                        }

                        this._realm.Write(() =>
                        {
                            currentTest.IsCompleted = true;
                            currentTest.ModifiedOn = this._dateTime.Now();
                            user.UserState = UserStates.IsTestComplete.ToString();
                        });

                        await this._dialogService
                            .ShowDialog(
                                AppResources.UnderTestViewModelCompleteTestMessage,
                                AppResources.UnderTestViewModelCompleteTestTitle,
                                AppResources.UnderTestViewModelCompleteTestButton);

                        // Stop Testing Time Notification
                        NotificationCenter.Current.Cancel(Globals.TestingTimeNotificationId);

                        // Stop Smoke Delayed Time Notification
                        NotificationCenter.Current.Cancel(Globals.DelayedSmokeNotificationId);

                    }
                    else
                    {
                        // User Not Found!
                        base._appLogger.LogCritical($"Can't find User Test: User Id: {userId}, Test Id {currentTestId}");

                        await base.InternalErrorMessageToUser();
                    }
                }
                else
                {
                    // User Not Found!
                    base._appLogger.LogCritical($"Can't find User: User Id {userId}");

                    await base.InternalErrorMessageToUser();
                }
            }
            catch (Exception ex)
            {
                base._appLogger.LogCritical(ex);

                await base.InternalErrorMessageToUser();
            }
        }

        /// <summary>
        /// Mark one smoke after delay of time
        /// </summary>
        private async void MarkSmokedAfterDelayLimitAsync()
        {
            base._dialogService.ShowToast(
                AppResources.UnderTestViewModelMarkAfterDelayDialogMessage);

            await SmokeOneCigareAsync(true);
        }

        /// <summary>
        /// Mark One Cigare Smoke
        /// </summary>
        /// <returns></returns>
        public async Task SmokeOneCigareAsync(bool delayed = false)
        {
            try
            {
                // Get user
                var userId = Globals.UserId;
                var user = _realm.Find<User>(userId);

                // Validate User
                if (user != null)
                {
                    // Get Current State
                    var currentTestId = user.TestId;
                    var currentTest = user
                        .Tests
                        .FirstOrDefault(t => t.Id == currentTestId);

                    // Validate Test
                    if (currentTest != null)
                    {
                        // Get Current Test Smoke
                        var currentSmoke = currentTest
                            .SmokedCigaresUnderTest
                            .FirstOrDefault(e => e.Id.Equals(this.CurrentSmokeId) && !e.IsDeleted);

                        // Validate
                        if (currentSmoke != null)
                        {
                            // Validate app state
                            if (!currentSmoke.EndSmokeTime.Equals(new DateTimeOffset()))
                            {
                                base._appLogger.LogCritical($"Current Test Smoke is completed, App state is compromissed : User Id: {userId}, Test Id {currentTestId}");
                            }

                            // Current count
                            var currentCountSmokes = currentTest.SmokedCigaresUnderTest.Count;

                            // Calculate time sence previous smoke
                            //this.TimeSenceLastSmoke = this._testCalculationService
                            //    .TimeSinceLastSmoke(currentTest, this._dateTime.Now());

                            this.TimeSenceLastSmoke = new TimeSpan(0, 0, 0);

                            // Update Db
                            this._realm.Write(() =>
                            {
                                // Is smoke bean forgoten
                                if (delayed)
                                {
                                    currentSmoke.EndSmokeTime = currentSmoke
                                        .StartSmokeTime
                                        .LocalDateTime
                                        .AddMinutes(Globals.OneSmokeTreshHoldTimeMinutes);
                                }
                                else
                                {
                                    currentSmoke.EndSmokeTime = this._dateTime.Now();
                                }

                                currentSmoke.ModifiedOn = this._dateTime.Now();
                            });

                            // Set it not smoking
                            this.IsSmoking = false;

                            this.CurrentlySmokedCount = currentCountSmokes;

                            // STOP SMOKING NOW Notification if is smoking Now
                            NotificationCenter.Current.Cancel(Globals.DelayedSmokeNotificationId);

                            StartTimeSenceLastSmokeTimer();
                            // Stop Timer
                            StopSmokingTimer();
                        }
                        else
                        {
                            // User Test Smoke Not Found!
                            base._appLogger.LogCritical($"Can't find User Test Smoke: User Id: {userId}, Test Id {currentTestId}");

                            await base.InternalErrorMessageToUser();
                        }
                    }
                    else
                    {
                        // User Test Not Found!
                        base._appLogger.LogCritical($"Can't find User Test: User Id: {userId}, Test Id {currentTestId}");

                        await base.InternalErrorMessageToUser();
                    }
                }
                else
                {
                    // User Not Found!
                    base._appLogger.LogCritical($"Can't find User: User Id {userId}");

                    await base.InternalErrorMessageToUser();
                }
            }
            catch (Exception ex)
            {
                base._appLogger.LogCritical(ex);

                await base.InternalErrorMessageToUser();
            }
        }

        /// <summary>
        /// Stops TimeSenceLastSmoke
        /// </summary>
        private void StopTimeSenceLastSmokeTimer()
        {
            _deviceTimer.Stop(this.stopTimeSenceLastSmokeTimerCancellation);

            this.stopTimeSenceLastSmokeTimerCancellation = new CancellationTokenSource();
        }

        /// <summary>
        /// Stops Testing Timer
        /// </summary>
        private void StopTestingTimer()
        {
            _deviceTimer.Stop(this.stopTestingTimerCancellation);
        }

        /// <summary>
        /// Stops Smoking Timer
        /// </summary>
        public void StopSmokingTimer()
        {
            _deviceTimer.Stop(this.stopSmokingTimerCancellation);

            this.stopSmokingTimerCancellation = new CancellationTokenSource();
        }

        #endregion

        #region PROPS

        /// <summary>
        /// Curent Smoked Count
        /// </summary>
        public int CurrentlySmokedCount
        {
            get { return _currentlySmokedCount; }
            set
            {
                _currentlySmokedCount = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Current Smoke Id
        /// </summary>
        public string CurrentSmokeId
        {
            get { return _currentSmokeId; }
            set
            {
                _currentSmokeId = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Current Smoke Time Display
        /// </summary>
        public TimeSpan CurrentSmokeTime
        {
            get { return _currentSmokeTime; }
            set
            {
                _currentSmokeTime = value;
                OnPropertyChanged();
            }
        }


        /// <summary>
        /// Timer From last Smoked
        /// </summary>
        public TimeSpan TimeSenceLastSmoke
        {
            get { return _timeSenceLastSmoke; }
            set
            {
                _timeSenceLastSmoke = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Test Left Time Indicator
        /// </summary>
        public TimeSpan TestLeftTime
        {
            get { return _testLeftTime; }
            set
            {
                _testLeftTime = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Smoking State
        /// </summary>
        public bool IsSmoking
        {
            get { return _isSmoking; }
            set
            {

                _isSmoking = value;
                OnPropertyChanged();
            }
        }

        #endregion
    }
}

