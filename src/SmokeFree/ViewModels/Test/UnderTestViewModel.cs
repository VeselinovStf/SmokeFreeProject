using Realms;
using SmokeFree.Abstraction.Managers;
using SmokeFree.Abstraction.Services.Data.Test;
using SmokeFree.Abstraction.Services.General;
using SmokeFree.Abstraction.Utility.DeviceUtilities;
using SmokeFree.Abstraction.Utility.Logging;
using SmokeFree.Abstraction.Utility.Wrappers;
using SmokeFree.Data.Models;
using SmokeFree.Models.Managers.NotificationManager;
using SmokeFree.Resx;
using SmokeFree.ViewModels.AppSettings;
using SmokeFree.ViewModels.Base;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.ObjectModel;

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
        /// Device Specific Notification Manager
        /// </summary>
        private readonly INotificationManager _notificationManager;

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
            INotificationManager notificationManager,
            ITestCalculationService testCalculationService,
            IDeviceTimer deviceTimer) : base(navigationService, dateTimeWrapper, appLogger, dialogService)
        {
            // Set View Title
            ViewTitle = AppResources.UnderTestViewTiitle;

            // Database
            _realm = realm;

            // Device Specific Notification Manager
            _notificationManager = notificationManager;

            // Test Calculation Service
            _testCalculationService = testCalculationService;

            // Device Timer
            _deviceTimer = deviceTimer;

            // Stop Testing Timer Cancelation Token
            this.stopTestingTimerCancellation = new CancellationTokenSource();

            // Smoking Timer Cancelation Token
            this.stopSmokingTimerCancellation = new CancellationTokenSource();
        }

        #endregion

        #region INIT

        /// <summary>
        /// Initialize View State
        /// </summary>
        /// <param name="parameter">Optional</param>
        /// <returns>Base Initialize Async</returns>
        public override async Task InitializeAsync(object parameter)
        {
            try
            {
                // Notification for test completition
                await InitiateTestCompletitionNotificationEvent();

                // Get User From DB
                var userId = Globals.UserId;
                var user = this._realm.Find<User>(userId);

                // Validate User
                if (user != null)
                {
                    // Get Current User Test
                    var currentUserTestId = user.TestId;
                    var currentTest = user.Tests
                        .FirstOrDefault(e => e.Id == currentUserTestId);

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
                        if (testCalculation.CurrentSmokeTime > TimeSpan.FromSeconds(1))
                        {
                            this.IsSmoking = true;
                        }

                        // Start Device Cound Down for Test Left Time
                        StartTestintTimer();
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
                base._appLogger.LogError(ex.Message);

                await base.InternalErrorMessageToUser();
            }
        }

        /// <summary>
        /// Initate Notification For Test COmpletition
        /// </summary>
        private async Task InitiateTestCompletitionNotificationEvent()
        {
            try
            {
                // Get Current User
                var userId = Globals.UserId;
                var user = _realm.Find<User>(userId);

                // Check if is valid
                if (user != null)
                {
                    var userNotificationPremission = user.NotificationState;

                    // Send Notification if user is allowed notifications
                    if (userNotificationPremission)
                    {
                        this._notificationManager.NotificationReceived += (sender, eventArgs) =>
                        {
                            var evtData = (NotificationEventArgs)eventArgs;
                            ShowNotification(evtData.Title, evtData.Message);
                        };
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
                base._appLogger.LogError($"Can't Initialize Under Test Completition Notification: {ex.Message}");

                await base.InternalErrorMessageToUser();
            }

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
                    // Get Current User
                    var userId = Globals.UserId;
                    var user = _realm.Find<User>(userId);

                    // Validate User
                    if (user != null)
                    {
                        var currentTestId = user.TestId;

                        // Delete Current Test Information From DB                        
                        _realm.Write(() =>
                        {
                            var userTest = user.Tests.FirstOrDefault(t => t.UserId == userId);

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

                            var testChallenge = user.Challenges.FirstOrDefault(c => c.UserId == userId);

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

                        // TODO: A: Stop any other timer
                        // TODO: A: Stop Notification

                        // Navigate to Create Test
                        await this._navigationService.NavigateToAsync<CreateTestViewModel>();
                        //TODO: B: Clear navigation stack

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
                base._appLogger.LogError(ex.Message);

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
                // Stop Testing Timer
                StopTestingTimer();

                await base._navigationService.NavigateToAsync<AppSettingsViewModel>();
            }
            catch (Exception ex)
            {
                base._appLogger.LogError(ex.Message);

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
                            .FirstOrDefault(u => u.UserId == user.Id);

                        // Validate Test
                        if (userTest != null)
                        {
                            // Check for un-finished smokes
                            var notCompleteSmokes = userTest.SmokedCigaresUnderTest
                                .Where(e => !e.StartSmokeTime.Equals(new DateTimeOffset()));

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
                base._appLogger.LogError(ex.Message);

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
                .ConfirmAsync(AppResources.UnderTestViewModelStartSmokeConfirmMessage,
                    AppResources.UnderTestViewModelStartSmokeConfirmTitle,
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
        /// Empty Method, Used By Platform Specific aps
        /// </summary>
        /// <param name="title">Notification Title</param>
        /// <param name="message">Notification Message</param>
        private void ShowNotification(string title, string message) { }

        /// <summary>
        /// Starts Testing Time
        /// </summary>
        private void StartTestintTimer()
        {
            _deviceTimer
                .Start(async () =>
                {
                    this.TestLeftTime = this.TestLeftTime - TimeSpan.FromSeconds(1);

                    if (this.TestLeftTime <= new TimeSpan(0, 0, 2))
                    {
                        // TODO: C: Add loader to view
                        base.IsBusy = true;

                        // Execute Function for stop testing
                        await MarkTestCompletedAsync();
                        await CreateTestResultAsync();
                        await SendNotificationAsync(
                            AppResources.UnderTestViewModelCompleteTestMessage,
                            AppResources.UnderTestViewModelCompleteTestNotificationMessage
                            );

                        StopTestingTimer();

                        base.IsBusy = false;

                        await NavigateToTestResults();

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
                .Start(async () =>
                {
                    this.CurrentSmokeTime += TimeSpan.FromSeconds(1);

                    if (this.CurrentSmokeTime.TotalSeconds > Globals.OneSmokeTreshHoldTimeMinutes)
                    {
                        // TODO: C: Add loader to view
                        base.IsBusy = true;

                        await SendNotificationAsync(
                            AppResources.UnderTestViewModelOneSmokeTreshHoldNotificationTitle,
                            AppResources.UnderTestViewModelOneSmokeTreshHoldNotificationMessage);
                        await MarkSmokedAfterDelayLimitAsync();

                        base.IsBusy = false;

                        return false;
                    }

                    return true;

                }, this.stopSmokingTimerCancellation);
        }

        /// <summary>
        /// Create Current Test Results
        /// </summary>
        public async Task CreateTestResultAsync()
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

                            // Write To db
                            this._realm.Write(() =>
                            {
                                userTest.CompletedTestResult = testResult;
                            });
                        }
                        else
                        {
                            // Cand Do Test Result Calculations
                            base._appLogger.LogCritical($"Can't Calculate Test Results: User id: {userId}, Test Id {testId}");

                            await base.InternalErrorMessageToUser();
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

                base._appLogger.LogError(ex.Message);

                await base.InternalErrorMessageToUser();
            }
        }

        /// <summary>
        /// Navigate to next view
        /// </summary>
        private async Task NavigateToTestResults()
        {
            await base._navigationService.NavigateToAsync<TestResultViewModel>();
        }

        /// <summary>
        /// Send Device Specific Notification For test Completition
        /// </summary>
        public async Task SendNotificationAsync(string notificationTitle, string notificationMessage)
        {
            try
            {
                // Get user
                var userId = Globals.UserId;
                var user = _realm.Find<User>(userId);

                // Validate User
                if (user != null)
                {
                    // Get User Notifications Options
                    var userNotification = user.NotificationState;
                    if (userNotification)
                    {
                        // Send Notificatio
                        this._notificationManager.SendNotification(
                                notificationTitle, notificationMessage);
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
                base._appLogger.LogError(ex.Message);

                await base.InternalErrorMessageToUser();
            }
        }

        /// <summary>
        /// Marks Current test completed
        /// </summary>
        public async Task MarkTestCompletedAsync()
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
                base._appLogger.LogError(ex.Message);

                await base.InternalErrorMessageToUser();
            }
        }

        /// <summary>
        /// Mark one smoke after delay of time
        /// </summary>
        private async Task MarkSmokedAfterDelayLimitAsync()
        {
            await base._dialogService.ShowDialog(
                AppResources.UnderTestViewModelMarkAfterDelayDialogMessage,
                AppResources.TestDataAccuired,
                AppResources.ButtonOkText);

            await SmokeOneCigareAsync();
        }

        /// <summary>
        /// Mark One Cigare Smoke
        /// </summary>
        /// <returns></returns>
        public async Task SmokeOneCigareAsync()
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
                            .FirstOrDefault(e => e.Id.Equals(this.CurrentSmokeId));

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
                            this.TimeSenceLastSmoke = this._testCalculationService
                                .TimeSinceLastSmoke(currentTest, this._dateTime.Now());

                            // Update Db
                            this._realm.Write(() =>
                            {
                                currentSmoke.EndSmokeTime = this._dateTime.Now();
                                currentSmoke.ModifiedOn = this._dateTime.Now();
                            });

                            // Set it not smoking
                            this.IsSmoking = false;

                            // +1 new ( currentrly smoked)
                            this.CurrentlySmokedCount = currentCountSmokes + 1;

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
                base._appLogger.LogError(ex.Message);

                await base.InternalErrorMessageToUser();
            }            
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
