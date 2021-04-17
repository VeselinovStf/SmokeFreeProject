﻿using Realms;
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
                        // User Not Found!
                        base._appLogger.LogCritical($"Can't find User Test: {currentUserTestId}");

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
            // Stop Testing Timer
            StopTestingTimer();

            await base._navigationService.NavigateToAsync<AppSettingsViewModel>();
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
                        // Execute Function for stop testing
                        await MarkTestCompleted();
                        CreateTestResult();
                        SendTestCompletitionNotification();
                        StopTestingTimer();
                        NavigateToTestResults();

                        return false;
                    }

                    return true;

                }, this.stopTestingTimerCancellation);
        }

        /// <summary>
        /// Create Current Test Results
        /// </summary>
        private void CreateTestResult()
        {
            //TODO: A: Implement
        }

        /// <summary>
        /// Navigate to next view
        /// </summary>
        private void NavigateToTestResults()
        {
            //TODO: A: Implement
        }

        /// <summary>
        /// Send Device Specific Notification For test Completition
        /// </summary>
        private void SendTestCompletitionNotification()
        {
            //TODO: A: Implement
        }

        /// <summary>
        /// Marks Current test completed
        /// </summary>
        public async Task MarkTestCompleted()
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
                            .ShowDialog("Test Completed", "Congratuations!", "Continue to challenge!");

                    }
                    else
                    {
                        // User Not Found!
                        base._appLogger.LogCritical($"Can't find User Test: Test Id {currentTestId}");

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
        /// Stop Testing Timer
        /// </summary>
        private void StopTestingTimer()
        {
            _deviceTimer.Stop(this.stopTestingTimerCancellation);
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
