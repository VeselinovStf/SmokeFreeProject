using Plugin.LocalNotification;
using Realms;
using SmokeFree.Abstraction.Services.Data.Test;
using SmokeFree.Abstraction.Services.General;
using SmokeFree.Abstraction.Utility.DeviceUtilities;
using SmokeFree.Abstraction.Utility.Logging;
using SmokeFree.Abstraction.Utility.Wrappers;
using SmokeFree.Constants.Messages;
using SmokeFree.Data.Models;
using SmokeFree.Models.Views.Challenge;
using SmokeFree.Resx;
using SmokeFree.Utilities.UserStateHelpers;
using SmokeFree.ViewModels.AppSettings;
using SmokeFree.ViewModels.Base;
using SmokeFree.Views.Challenge;
using SmokeFree.Views.Test;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace SmokeFree.ViewModels.Challenge
{
    /// <summary>
    /// ChallengeVie Model
    /// </summary>
    public class ChallengeViewModel : ViewModelBase
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
        /// TimeToNextSmoke Timer Cancelation Token
        /// </summary>
        public CancellationTokenSource stopTimeToNextSmokeTimerCancellation;

        /// <summary>
        /// Database
        /// </summary>
        private readonly Realm _realm;

        private readonly IDeviceTimer _deviceTimer;

        private readonly IChallengeCalculationService _challengeCalculationService;

        private readonly ITestCalculationService _testCalculationService;

        /// <summary>
        /// Curent Smoked Count
        /// </summary>
        private int _currentlySmokedCount;

        /// <summary>
        /// Curent Day Max Smoked Count
        /// </summary>
        private int _currentDayMaxSmokeCountCount;

        /// <summary>
        /// Timer To Next Smoke
        /// </summary>
        private TimeSpan _timeToNextSmoke;

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
        /// User Smoke Status
        /// </summary>
        private UserSmokeStatusItem _userSmokeStatus;

        /// <summary>
        /// Current Day Challenge Id
        /// </summary>
        private string _currentDayChallengeId;


        #endregion

        #region CTOR

        public ChallengeViewModel(
            Realm realm,
            INavigationService navigationService,
            IDateTimeWrapper dateTimeWrapper,
            IAppLogger appLogger,
            IDialogService dialogService,
            IDeviceTimer deviceTimer,
            IChallengeCalculationService challengeCalculationService,
            ITestCalculationService testCalculationService) : base(navigationService, dateTimeWrapper, appLogger, dialogService)
        {
            _realm = realm;

            _deviceTimer = deviceTimer;

            _challengeCalculationService = challengeCalculationService;

            _testCalculationService = testCalculationService;

            // Initial Subscribe-UnSubscribe to OnAppearing of View Model
            MessagingCenter.Subscribe<ChallengeView>(this, MessagingCenterConstant.ChallengeViewAppearing, async (e) =>
            {
                await AppearingInitializeAsync();

                MessagingCenter.Unsubscribe<ChallengeView>(this, MessagingCenterConstant.ChallengeViewAppearing);
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

                // Time To Next Smoke Timer Cancelation Token
                this.stopTimeToNextSmokeTimerCancellation = new CancellationTokenSource();

                // Get Challenge
                var userId = Globals.UserId;
                var user = _realm.Find<User>(userId);

                if (user == null)
                {
                    // User Not Found!
                    base._appLogger.LogCritical($"Can't find User: {userId}");

                    await base.InternalErrorMessageToUser();
                }

                var challenge = _realm.All<Data.Models.Challenge>()
                    .FirstOrDefault(e => !e.IsDeleted && e.UserId == userId && !e.IsCompleted);

                // Validate Challenge
                if (challenge != null)
                {
                    // Set User Smoke Status
                    var userSmokerStatus = StringToEnum.ToUserState<UserSmokeStatuses>(user.UserSmokeStatuses);
                    SetUserSmokerStatus(userSmokerStatus);

                    // Not Started Challenge
                    if (challenge.CurrentDayOfChallenge == 0)
                    {

                        this.IsSmoking = false;

                        // First Challenge Day
                        _realm.Write(() =>
                        {
                            challenge.CurrentDayOfChallenge = 1;
                        });

                        var firstDayChallengeSmokes = challenge
                            .ChallengeSmokes.FirstOrDefault(e => !e.IsDeleted && !e.IsCompleted && e.DayOfChallenge == 1);

                        if (firstDayChallengeSmokes != null)
                        {
                            this.CurrentlDayMaxSmokeCount = firstDayChallengeSmokes.DayMaxSmokesLimit;
                            this.CurrentDayChallengeId = firstDayChallengeSmokes.Id;
                            this.CurrentlySmokedCount = 0;
                        }
                        else
                        {
                            // User Challenge Current Day Smokes Not Found!
                            base._appLogger.LogCritical($"On Challenge Day 0 set to 1, Not Found Challenge Smokes! Invalid App State: User Id :{userId}, Challenge Id {challenge.Id}");

                            await base.InternalErrorMessageToUser();
                        }
                    }
                    else
                    {
                        var currentDayChallenge = challenge.ChallengeSmokes
                            .OrderBy(e => e.DayOfChallenge)
                            .FirstOrDefault(e => !e.StartSmokeTime.Equals(new DateTimeOffset()) && e.EndSmokeTime.Equals(new DateTimeOffset()) && !e.IsCompleted && !e.IsDeleted);

                        // First Challenge Visit without smoked cigares - and is not started
                        if (currentDayChallenge == null)
                        {
                            this.IsSmoking = false;

                            var firstDayChallengeSmokes = challenge
                                .ChallengeSmokes.FirstOrDefault(e => !e.IsDeleted && !e.IsCompleted && e.DayOfChallenge == challenge.CurrentDayOfChallenge);

                            if (firstDayChallengeSmokes != null)
                            {
                                this.CurrentlDayMaxSmokeCount = firstDayChallengeSmokes.DayMaxSmokesLimit;
                                this.CurrentDayChallengeId = firstDayChallengeSmokes.Id;
                                this.CurrentlySmokedCount = firstDayChallengeSmokes.DaySmoked;
                            }
                            else
                            {
                                // User Challenge Current Day Smokes Not Found!
                                base._appLogger.LogCritical($"On Challenge Day 0 set to 1, Not Found Challenge Smokes! Invalid App State: User Id :{userId}, Challenge Id {challenge.Id}");

                                await base.InternalErrorMessageToUser();
                            }
                        }
                        else
                        {
                            var challengeSmokes = currentDayChallenge
                                .DaySmokes;

                            this.CurrentlDayMaxSmokeCount = currentDayChallenge.DayMaxSmokesLimit;
                            this.CurrentDayChallengeId = currentDayChallenge.Id;
                            this.CurrentlySmokedCount = currentDayChallenge.DaySmoked;

                            if (challengeSmokes.Count > 0)
                            {
                                var leftUnFinishedSmokes =
                                    challengeSmokes.Where(e => !e.StartSmokeTime.Equals(new DateTimeOffset()) && e.EndSmokeTime.Equals(new DateTimeOffset()));

                                if (leftUnFinishedSmokes.Count() > 0)
                                {
                                    if (leftUnFinishedSmokes.Count() > 1)
                                    {
                                        // User Challenge Current Day Smokes Not Found!
                                        base._appLogger.LogCritical($"On Challenge Day {challenge.CurrentDayOfChallenge},Invalid App State, {leftUnFinishedSmokes.Count()} of Day Challenge Smokes Are Not Completed : User Id :{userId}, Challenge Id {challenge.Id}");

                                        await base.InternalErrorMessageToUser();
                                    }
                                    else
                                    {
                                        this.IsSmoking = true;

                                        var leftSmoke = leftUnFinishedSmokes.FirstOrDefault();
                                        this.CurrentSmokeId = leftSmoke.Id;
                                        this.CurrentSmokeTime = _dateTime.Now().Subtract(leftSmoke.StartSmokeTime.LocalDateTime);

                                        StartSmokingTimer();
                                    }

                                }
                                else
                                {
                                    this.IsSmoking = false;

                                    var lastSmoked = challengeSmokes
                                        .OrderByDescending(e => e.EndSmokeTime)
                                        .FirstOrDefault();

                                    var secondsTimeToNext = (int)_dateTime.Now().Subtract(lastSmoked.EndSmokeTime.LocalDateTime).TotalSeconds + currentDayChallenge.DistanceToNextInSeconds;
                                    this.TimeToNextSmoke = new TimeSpan(0, 0, secondsTimeToNext);

                                    StartTimeToNextSmokeTimer();
                                }
                            }
                            else
                            {
                                this.IsSmoking = false;
                            }
                        }
                    }

                    this.TestLeftTime = challenge.GoalCompletitionTime.LocalDateTime.Subtract(this._dateTime.Now());


                    // Start Testing Timer
                    StartTestintTimer();

                    // Subscribe to OnDesapearing
                    // Initial Subscribe-UnSubscribe to OnDesapearing of View Model
                    MessagingCenter.Subscribe<ChallengeView>(this, MessagingCenterConstant.ChallengeViewDisappearing, (e) =>
                    {
                        DesapearingInitializeAsync();

                        MessagingCenter.Unsubscribe<ChallengeView>(this, MessagingCenterConstant.ChallengeViewDisappearing);
                    });
                }
                else
                {
                    // User Challenge Not Found!
                    base._appLogger.LogCritical($"Can't find User Challenge: {userId}");

                    await base.InternalErrorMessageToUser();
                }
            }
            catch (Exception ex)
            {
                base._appLogger.LogCritical(ex);

                await base.InternalErrorMessageToUser();
            }
        }


        private void DesapearingInitializeAsync()
        {
            StopSmokingTimer();
            StopTestingTimer();
            StopTimeToNextSmokeTimer();

            // Post Subscribe-UnSubscribe to OnAppearing of View Model
            MessagingCenter.Subscribe<UnderTestView>(this, MessagingCenterConstant.ChallengeViewAppearing, async (e) =>
            {
                await AppearingInitializeAsync();

                MessagingCenter.Unsubscribe<UnderTestView>(this, MessagingCenterConstant.ChallengeViewAppearing);
            });

        }

        #endregion

        #region COMMANDS

        /// <summary>
        /// Back One Day Of Challenge
        /// </summary>
        public IAsyncCommand OnBackOneChallengeDayCommand => new AsyncCommand(
            BackOneChallengeDay,
            onException: base.GenericCommandExeptionHandler,
            allowsMultipleExecutions: false);

        /// <summary>
        /// Back One Day Of Challenge
        /// </summary>
        public IAsyncCommand OnMarkDayCompletedCommand => new AsyncCommand(
            ExecuteMarkDayCompleted,
            onException: base.GenericCommandExeptionHandler,
            allowsMultipleExecutions: false);

        private async Task ExecuteMarkDayCompleted()
        {
            // Check User Notification
            var userNotification = await base._dialogService
                .ConfirmAsync(AppResources.ChallengeViewModelMarkDayCompletedConfirmMessage,
                    AppResources.ChallengeViewModelMarkDayCompletedConfirmTitle,
                    AppResources.YesButtonText,
                    AppResources.NoButtonText);

            if (userNotification)
            {
                await MarkDayCompleted();
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
                if (this.TimeToNextSmoke > new TimeSpan(0, 0, 2))
                {
                    // Check User Notification
                    var timeNotification = await base._dialogService
                        .ConfirmAsync(AppResources.ChallengeViewModellStartSmokeTimeConfirmMessage,
                        AppResources.ChallengeViewModelStartSmokeTimeConfirmTitle,
                        AppResources.YesButtonText,
                        AppResources.NoButtonText);

                    if (!timeNotification)
                    {
                        return;
                    }
                }

                if (this.CurrentlySmokedCount + 1 >= this.CurrentlDayMaxSmokeCount)
                {
                    // Check User Notification
                    var countNotification = await base._dialogService
                        .ConfirmAsync(AppResources.ChallengeViewModellStartSmokeCountConfirmMessage,
                        AppResources.ChallengeViewModelStartSmokeCountConfirmTitle,
                        AppResources.YesButtonText,
                        AppResources.NoButtonText);

                    if (!countNotification)
                    {
                        return;
                    }
                }

                // Check User Notification
                var userNotification = await base._dialogService
                    .ConfirmAsync(AppResources.ChallengeViewModellStartSmokeConfirmMessage,
                    AppResources.ChallengeViewModelStartSmokeConfirmTitle,
                    AppResources.YesButtonText,
                    AppResources.NoButtonText);

                if (userNotification)
                {
                    // Validate if previous smoke is complete
                    if (!string.IsNullOrWhiteSpace(this.CurrentSmokeId))
                    {
                        var lastSmokeState = _realm.Find<ChallengeSmoke>(this.CurrentSmokeId);
                        if (lastSmokeState == null)
                        {
                            // Previous smoke is not stored in db
                            base._appLogger.LogError($"Previous Challenge Smoke was not found in db! Internal state issud: Challenge Smoke Id {CurrentSmokeId}");
                        }
                        else
                        {
                            if (!lastSmokeState.IsCompleted)
                            {
                                // Left un completed smoke from previous runs
                                base._appLogger.LogError($"Previous Challenge Smoke was not completed! Internal state issud: Challenge Smoke Id {CurrentSmokeId}");

                                _realm.Write(() =>
                                {
                                    lastSmokeState.IsCompleted = true;
                                    lastSmokeState.ModifiedOn = _dateTime.Now();

                                    if (lastSmokeState.EndSmokeTime.Equals(new DateTimeOffset()))
                                    {
                                        lastSmokeState.EndSmokeTime = _dateTime.Now();
                                    }
                                });
                            }
                        }
                    }
                    else
                    {
                        // If is empty - this is first smoke - set up challenge

                        var currentDayChallenge = _realm.Find<DayChallengeSmoke>(this.CurrentDayChallengeId);

                        if (currentDayChallenge != null)
                        {
                            _realm.Write(() =>
                            {
                                currentDayChallenge.StartSmokeTime = _dateTime.Now();
                            });
                        }
                        else
                        {
                            // Day Challenge Not Found!
                            base._appLogger.LogCritical($"Can't find DayChallengeSmoke: DayChallengeSmoke Id {this.CurrentDayChallengeId}");

                            await base.InternalErrorMessageToUser();
                        }

                    }

                    // Get Current User for Notifications
                    var userId = Globals.UserId;
                    var user = _realm.Find<User>(userId);

                    // Validate User
                    if (user != null)
                    {
                        var currentDayChallenge = _realm.Find<DayChallengeSmoke>(this.CurrentDayChallengeId);

                        // Create new 
                        var newChallengeSmoke = new Data.Models.ChallengeSmoke()
                        {
                            ChallengeId = this.CurrentDayChallengeId,
                            StartSmokeTime = this._dateTime.Now(),
                            CreatedOn = this._dateTime.Now(),
                        };

                        // Save to DB
                        _realm.Write(() =>
                        {
                            currentDayChallenge.DaySmokes.Add(newChallengeSmoke);
                        });

                        // Assign Props
                        this.CurrentSmokeId = newChallengeSmoke.Id;
                        this.IsSmoking = true;

                        // Start Smoking Timer
                        StartSmokingTimer();

                        StopTimeToNextSmokeTimer();

                        if (user.NotificationState)
                        {
                            // Register Notification
                            var delaySmokeNotification = new NotificationRequest
                            {

                                NotificationId = Globals.DelayedChallengeSmokeNotificationId,
                                Title = AppResources.ChallengeViewModelOneSmokeTreshHoldNotificationTitle,
                                Description = AppResources.ChallengeViewModelOneSmokeTreshHoldNotificationMessage,
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

        /// <summary>
        /// Mark One Smoked
        /// </summary>
        public IAsyncCommand<bool> SkipOneSmokedCommand => new AsyncCommand<bool>(isSmoking =>
           ExecuteSkipOneSmoke(),
            onException: base.GenericCommandExeptionHandler,
            allowsMultipleExecutions: false);

        private async Task ExecuteSkipOneSmoke()
        {
            // Check User Notification
            var userNotification = await base._dialogService
                .ConfirmAsync(AppResources.ChallengeViewModelSkipOneSmokeConfirmMessage,
                    AppResources.ChallengeViewModelSkipOneSmokeConfirmTitle,
                    AppResources.YesButtonText,
                    AppResources.NoButtonText);

            if (userNotification)
            {
                SkipSmoke();
            }
        }

        /// <summary>
        /// Async Start Challenge Command and Logic
        /// </summary>
        public IAsyncCommand OnStopChallengeCommand => new AsyncCommand(
            ExecuteStopChallengeCommand,
            onException: base.GenericCommandExeptionHandler,
            allowsMultipleExecutions: false);

        private async Task ExecuteStopChallengeCommand()
        {
            try
            {
                // Check if user is shure
                var userNotification = await base._dialogService
                     .ConfirmAsync(
                     AppResources.CreateChallengeViewModelBackToTestingMessage,
                     AppResources.CreateChallengeViewModelBackToTestingTitle,
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
                        var userTest = user.Tests.FirstOrDefault(t => t.UserId == userId && !t.IsDeleted);
                        var testChallenge = user.Challenges.FirstOrDefault(c => c.UserId == userId && !c.IsDeleted);
                        // 

                        _realm.Write(() =>
                        {
                            // Remove Test
                            userTest.IsDeleted = true;
                            userTest.DeletedOn = this._dateTime.Now();
                            userTest.ModifiedOn = this._dateTime.Now();

                            userTest.CompletedTestResult.IsDeleted = true;
                            userTest.CompletedTestResult.DeletedOn = this._dateTime.Now();
                            userTest.CompletedTestResult.ModifiedOn = this._dateTime.Now();

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

                            foreach (var cs in testChallenge.ChallengeSmokes)
                            {
                                cs.IsDeleted = true;
                                cs.DeletedOn = this._dateTime.Now();
                                cs.ModifiedOn = this._dateTime.Now();
                            }

                            // Update User Status
                            user.UserState = UserStates.CompletedOnBoarding.ToString();
                            user.TestId = string.Empty;
                        });

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

        #endregion

        #region METHODS

        /// <summary>
        /// Increment TimeSenceLastSmoke
        /// </summary>
        private void StartTimeToNextSmokeTimer()
        {
            _deviceTimer
                .Start(() =>
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        this.TimeToNextSmoke -= TimeSpan.FromSeconds(1);

                    });

                    if (this.TimeToNextSmoke < TimeSpan.FromSeconds(1))
                    {
                        // TODO: Add Notification

                        return false;
                    }


                    return true;

                }, this.stopTimeToNextSmokeTimerCancellation);
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
                        CompleteChallenge();

                        StopTestingTimer();

                        base.IsBusy = false;

                        return false;
                    }


                    return true;

                }, this.stopTestingTimerCancellation);
        }

        /// <summary>
        /// Mark one smoke after delay of time
        /// </summary>
        private async void MarkSmokedAfterDelayLimitAsync()
        {
            base._dialogService.ShowToast(
                AppResources.ChallengeViewModelMarkAfterDelayDialogMessage);

            await SmokeOneCigareAsync(true);
        }

        /// <summary>
        /// Complete One Challenge Day
        /// </summary>
        /// <returns></returns>
        private async Task MarkDayCompleted()
        {
            try
            {
                var userId = Globals.UserId;
                var challenge = _realm.All<Data.Models.Challenge>()
                    .FirstOrDefault(e => !e.IsDeleted && e.UserId == userId && !e.IsCompleted);

                if (challenge != null)
                {
                    var currentChallengeDay = challenge
                        .ChallengeSmokes.FirstOrDefault(e => e.Id == this.CurrentDayChallengeId && !e.IsDeleted && !e.IsCompleted);

                    if (currentChallengeDay != null)
                    {

                        var newChallengeDay = _realm.All<DayChallengeSmoke>()
                            .Where(e => !e.IsDeleted && !e.IsCompleted)
                            .ToList();

                        var totalDays = newChallengeDay.Max(e => e.DayOfChallenge);
                        var nextDay = currentChallengeDay.DayOfChallenge + 1;

                        if (totalDays >= nextDay)
                        {
                            _realm.Write(() =>
                            {
                                currentChallengeDay.IsCompleted = true;
                                currentChallengeDay.ModifiedOn = this._dateTime.Now();
                                currentChallengeDay.EndSmokeTime = this._dateTime.Now();

                                foreach (var currentSmoke in currentChallengeDay.DaySmokes)
                                {
                                    currentSmoke.IsCompleted = true;
                                    currentSmoke.ModifiedOn = this._dateTime.Now();
                                    currentSmoke.EndSmokeTime = this._dateTime.Now();
                                    currentSmoke.IsDeleted = true;
                                }

                                challenge.CurrentDayOfChallenge += 1;

                            });

                            NotificationCenter.Current.Cancel(Globals.DelayedChallengeSmokeNotificationId);
                            StopSmokingTimer();
                            StopTimeToNextSmokeTimer();

                            await _navigationService.NavigateToAsync<ChallengeViewModel>();
                            //TODO: Back Stack
                        }
                        else
                        {
                            // Check User Notification
                            var userNotification = await base._dialogService
                                .ConfirmAsync(AppResources.ChallengeViewModelJumpToCompleteConfirmMessage,
                                    AppResources.ChallengeViewModelJumpToCompleteConfirmTitle,
                                    AppResources.YesButtonText,
                                    AppResources.NoButtonText);

                            if (userNotification)
                            {
                                _realm.Write(() =>
                                {
                                    currentChallengeDay.IsCompleted = true;
                                    currentChallengeDay.ModifiedOn = this._dateTime.Now();
                                    currentChallengeDay.EndSmokeTime = this._dateTime.Now();

                                    foreach (var currentSmoke in currentChallengeDay.DaySmokes)
                                    {
                                        currentSmoke.IsCompleted = true;
                                        currentSmoke.ModifiedOn = this._dateTime.Now();
                                        currentSmoke.EndSmokeTime = this._dateTime.Now();
                                        currentSmoke.IsDeleted = true;
                                    }
                                });

                                NotificationCenter.Current.Cancel(Globals.DelayedChallengeSmokeNotificationId);
                                StopSmokingTimer();
                                StopTimeToNextSmokeTimer();

                                CompleteChallenge();
                            }
                            else
                            {
                                return;
                            }
                        }

                    }
                    else
                    {
                        // Day Challenge Not Found!
                        base._appLogger.LogCritical($"Can't find DayChallengeSmoke: DayChallengeSmoke Id {this.CurrentDayChallengeId}");

                        await base.InternalErrorMessageToUser();
                    }
                }
                else
                {
                    // Day Challenge Not Found!
                    base._appLogger.LogCritical($"Can't find User Challenge:User Id {userId}");

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
        /// Mark One Cigar Smoked
        /// </summary>
        /// <param name="delayed"></param>
        /// <returns></returns>
        private async Task SmokeOneCigareAsync(bool delayed = false, bool isSkiped = false)
        {
            try
            {
                var currentChallengeDay = _realm
                    .Find<DayChallengeSmoke>(this.CurrentDayChallengeId);

                if (currentChallengeDay != null)
                {
                    var currentSmoke = currentChallengeDay
                        .DaySmokes.FirstOrDefault(e => e.Id == this.CurrentSmokeId);

                    if (currentSmoke != null)
                    {
                        var userId = Globals.UserId;
                        var user = _realm.Find<User>(userId);

                        var currentSmokeStatus = this._testCalculationService
                            .CalculateUserSmokeStatusBySmokes(currentChallengeDay.DaySmoked + 1);


                        _realm.Write(() =>
                        {
                            currentChallengeDay.DaySmoked += 1;
                            currentSmoke.IsCompleted = true;
                            currentSmoke.ModifiedOn = this._dateTime.Now();

                            if (isSkiped)
                            {
                                currentSmoke.IsSkiped = true;
                            }

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

                            user.UserSmokeStatuses = currentSmokeStatus.ToString();

                        });

                        var userSmokerStatus = StringToEnum.ToUserState<UserSmokeStatuses>(user.UserSmokeStatuses);
                        SetUserSmokerStatus(userSmokerStatus);

                        var secondsTimeToNext = _dateTime.Now().Subtract(_dateTime.Now().AddSeconds(currentChallengeDay.DistanceToNextInSeconds));
                        this.TimeToNextSmoke = secondsTimeToNext;

                        this.CurrentlySmokedCount = currentChallengeDay.DaySmokes.Count;

                        NotificationCenter.Current.Cancel(Globals.DelayedChallengeSmokeNotificationId);

                        this.IsSmoking = false;

                        StartTimeToNextSmokeTimer();

                        StopSmokingTimer();
                    }
                    else
                    {
                        // Day Challenge Not Found!
                        base._appLogger.LogCritical($"Can't find DayChallengeSmoke smoke: Challenge Id: {this.CurrentDayChallengeId}, DayChallengeSmoke Id {this.CurrentDayChallengeId}");

                        await base.InternalErrorMessageToUser();
                    }

                }
                else
                {
                    // Day Challenge Not Found!
                    base._appLogger.LogCritical($"Can't find DayChallengeSmoke: DayChallengeSmoke Id {this.CurrentDayChallengeId}");

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
        /// Set Complete Test
        /// </summary>
        private async void CompleteChallenge()
        {
            try
            {
                // Check if user is complete some of the challenges
                // Complete all
                var userId = Globals.UserId;
                var challenge = _realm.All<Data.Models.Challenge>()
                    .FirstOrDefault(e => !e.IsDeleted && e.UserId == userId && !e.IsCompleted);

                if (challenge != null)
                {
                    var challengeResultCalculation = this._challengeCalculationService
                        .CalculateChallengeResult(challenge);

                    if (challengeResultCalculation.Success)
                    {

                        // Mark All Completed-IsDeleted in db
                        _realm.Write(() =>
                        {
                            var user = _realm.Find<User>(userId);
                            var currentTestId = user.TestId;

                            // Delete Current Test Information From DB
                            var userTest = user.Tests.FirstOrDefault(t => t.UserId == userId && !t.IsDeleted);
                            var testChallenge = user.Challenges.FirstOrDefault(c => c.UserId == userId && !c.IsDeleted);

                            challenge.IsCompleted = true;
                            challenge.ModifiedOn = _dateTime.Now();

                            foreach (var chs in challenge.ChallengeSmokes)
                            {
                                chs.ModifiedOn = _dateTime.Now();
                                chs.DeletedOn = _dateTime.Now();
                                chs.IsDeleted = true;
                                chs.IsCompleted = true;

                                foreach (var s in chs.DaySmokes)
                                {
                                    s.ModifiedOn = _dateTime.Now();
                                    s.DeletedOn = _dateTime.Now();
                                    s.IsDeleted = true;
                                    s.IsCompleted = true;
                                }
                            }

                            // Remove Test
                            userTest.ModifiedOn = this._dateTime.Now();

                            userTest.CompletedTestResult.IsDeleted = true;
                            userTest.CompletedTestResult.DeletedOn = this._dateTime.Now();
                            userTest.CompletedTestResult.ModifiedOn = this._dateTime.Now();

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

                            // Update User Status
                            user.UserState = UserStates.Complete.ToString();
                            user.TestId = string.Empty;
                            user.ChallengeResultId = challengeResultCalculation.ChallengeResult.Id;

                            var challengeResult = challengeResultCalculation.ChallengeResult;
                            challengeResult.ChallengeId = challenge.Id;
                            challengeResult.CreatedOn = _dateTime.Now();

                            _realm.Add(challengeResultCalculation.ChallengeResult);
                        });

                        await this._dialogService
                            .ShowDialog(
                                AppResources.ChallengViewModelCompleteMessage,
                                AppResources.ChallengViewModelCompleteTitle,
                                AppResources.ChallengViewModelCompleteButton);

                        // Stop Testing Time Notification
                        NotificationCenter.Current.Cancel(Globals.ChallengeNotificationId);

                        // Stop Smoke Delayed Time Notification
                        NotificationCenter.Current.Cancel(Globals.DelayedChallengeSmokeNotificationId);

                        StopSmokingTimer();
                        StopTimeToNextSmokeTimer();
                        StopTestingTimer();

                        NavigateToCompletedChallenge();
                    }
                    else
                    {
                        // Day Challenge Not Found!
                        base._appLogger.LogCritical($"Can't Calculate Challenge Result :User Id {userId}, Challenge Id {challenge.Id}, REASON: {challengeResultCalculation.Message}");

                        await base.InternalErrorMessageToUser();
                    }
                }
                else
                {
                    // Day Challenge Not Found!
                    base._appLogger.LogCritical($"Can't find User Challenge:User Id {userId}");

                    await base.InternalErrorMessageToUser();
                }
            }
            catch (Exception ex)
            {
                base._appLogger.LogCritical(ex);

                await base.InternalErrorMessageToUser();
            }
        }

        public async void SkipSmoke()
        {
            await SmokeOneCigareAsync(false, true);
        }

        /// <summary>
        /// Redo One Day Of Challenge
        /// </summary>
        /// <returns></returns>
        private async Task BackOneChallengeDay()
        {
            try
            {
                // Check User Notification
                var userNotification = await base._dialogService
                    .ConfirmAsync(AppResources.ChallengeViewModelBackOneDayConfirmMessage,
                        AppResources.ChallengeViewModelBackOneDayConfirmTitle,
                        AppResources.YesButtonText,
                        AppResources.NoButtonText);

                if (userNotification)
                {
                    var userId = Globals.UserId;
                    var challenge = _realm.All<Data.Models.Challenge>()
                        .FirstOrDefault(e => !e.IsDeleted && e.UserId == userId && !e.IsCompleted);

                    if (challenge != null)
                    {
                        var currentChallengeDay = challenge
                            .ChallengeSmokes.FirstOrDefault(e => e.Id == this.CurrentDayChallengeId && !e.IsCompleted && !e.IsDeleted);

                        if (currentChallengeDay != null)
                        {
                            var previousDayChallenge = challenge
                                .ChallengeSmokes.FirstOrDefault(e => e.DayOfChallenge == currentChallengeDay.DayOfChallenge - 1);

                            if (previousDayChallenge != null)
                            {
                                // Update Entities in DB
                                _realm.Write(() =>
                                {
                                    currentChallengeDay.StartSmokeTime = new DateTimeOffset();
                                    currentChallengeDay.DaySmoked = 0;
                                    currentChallengeDay.ModifiedOn = _dateTime.Now();

                                    foreach (var ds in currentChallengeDay.DaySmokes)
                                    {
                                        ds.IsDeleted = true;
                                        ds.DeletedOn = _dateTime.Now();
                                    }

                                    previousDayChallenge.StartSmokeTime = new DateTimeOffset();
                                    previousDayChallenge.DaySmoked = 0;
                                    previousDayChallenge.ModifiedOn = _dateTime.Now();

                                    foreach (var ds in previousDayChallenge.DaySmokes)
                                    {
                                        ds.IsDeleted = true;
                                        ds.DeletedOn = _dateTime.Now();
                                    }

                                });

                                this.CurrentDayChallengeId = string.Empty;
                                this.CurrentSmokeId = string.Empty;

                                // Stop Smoke Delayed Time Notification
                                NotificationCenter.Current.Cancel(Globals.DelayedChallengeSmokeNotificationId);

                                StopSmokingTimer();
                                StopTimeToNextSmokeTimer();
                                StopTestingTimer();

                                // Re-Initialize
                                await AppearingInitializeAsync();
                            }
                            else
                            {
                                // Day Back Challenge Not Found!
                                base._appLogger.LogCritical($"Can't find User Day Back Challenge - for day {currentChallengeDay.DayOfChallenge - 1} :User Id {userId}, Day Challenge Id: {CurrentDayChallengeId}");

                                //TODO: Localization
                                await base._dialogService
                                    .ShowDialog("Can't Go Back one day, you are on your first",
                                        "Can't Go Back",
                                        AppResources.ButtonOkText);
                            }
                        }
                        else
                        {
                            // Day Challenge Not Found!
                            base._appLogger.LogCritical($"Can't find User Day Challenge:User Id {userId}, Day Challenge Id: {CurrentDayChallengeId}");

                            await base.InternalErrorMessageToUser();
                        }
                    }
                    else
                    {
                        //  Challenge Not Found!
                        base._appLogger.LogCritical($"Can't find User Challenge:User Id {userId}");

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
        /// Navigate to next view
        /// </summary>
        private async void NavigateToCompletedChallenge()
        {
            await base._navigationService.NavigateToAsync<CompletedChallengeViewModel>();
        }

        /// <summary>
        /// Stops Testing Timer
        /// </summary>
        private void StopTestingTimer()
        {
            _deviceTimer.Stop(this.stopTestingTimerCancellation);
        }

        /// <summary>
        /// Stops TimeToNextSmokeTimer
        /// </summary>
        private void StopTimeToNextSmokeTimer()
        {
            _deviceTimer.Stop(this.stopTimeToNextSmokeTimerCancellation);

            this.stopTimeToNextSmokeTimerCancellation = new CancellationTokenSource();
        }

        /// <summary>
        /// Stops Smoking Timer
        /// </summary>
        public void StopSmokingTimer()
        {
            _deviceTimer.Stop(this.stopSmokingTimerCancellation);

            this.stopSmokingTimerCancellation = new CancellationTokenSource();
        }

        private void SetUserSmokerStatus(UserSmokeStatuses userSmokerStatus)
        {
            var userSmokerStatusIcon = Globals.UserSmokeStatusesSet[userSmokerStatus].First().Item1;
            var userSmokerStatusMessage = Globals.UserSmokeStatusesSet[userSmokerStatus].First().Item2;

            this.UserSmokeStatus = new UserSmokeStatusItem()
            {
                Icon = userSmokerStatusIcon,
                Title = AppResources.UserSmokeStatusTitle,
                Message = userSmokerStatusMessage
            };
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
        /// Curent Day MaxSmoked Count
        /// </summary>
        public int CurrentlDayMaxSmokeCount
        {
            get { return _currentDayMaxSmokeCountCount; }
            set
            {
                _currentDayMaxSmokeCountCount = value;
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
        /// Current Day Challenge Id
        /// </summary>
        public string CurrentDayChallengeId
        {
            get { return _currentDayChallengeId; }
            set
            {
                _currentDayChallengeId = value;
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
        /// Timer To Next Smoke
        /// </summary>
        public TimeSpan TimeToNextSmoke
        {
            get { return _timeToNextSmoke; }
            set
            {
                _timeToNextSmoke = value;
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

        /// <summary>
        /// User Smoke Status
        /// </summary>
        public UserSmokeStatusItem UserSmokeStatus
        {
            get { return _userSmokeStatus; }
            set
            {
                _userSmokeStatus = value;
                OnPropertyChanged();
            }
        }


        #endregion
    }
}
