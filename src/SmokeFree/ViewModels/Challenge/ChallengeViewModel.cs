using Realms;
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
            IDeviceTimer deviceTimer) : base(navigationService, dateTimeWrapper, appLogger, dialogService)
        {
            _realm = realm;

            _deviceTimer = deviceTimer;

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
                    .FirstOrDefault(e => !e.IsDeleted && e.UserId == userId);

                // Validate Challenge
                if (challenge != null)
                {
                    // Set User Smoke Status
                    var userSmokerStatus = StringToEnum.ToUserState<UserSmokeStatuses>(user.UserSmokeStatuses);
                    var userSmokerStatusIcon = Globals.UserSmokeStatusesSet[userSmokerStatus].First().Item1;
                    var userSmokerStatusMessage = Globals.UserSmokeStatusesSet[userSmokerStatus].First().Item2;

                    this.UserSmokeStatus = new UserSmokeStatusItem()
                    {
                        Icon = userSmokerStatusIcon,
                        Title = AppResources.UserSmokeStatusTitle,
                        Message = userSmokerStatusMessage
                    };

                    // Not Started Challenge
                    if (challenge.CurrentDayOfChallenge == 0)
                    {
                        // App State Validation
                        if (challenge.ChallengeSmokes.Count == 0)
                        {
                            this.IsSmoking = false;
                        }
                        else
                        {
                            // User Challenge Not Found!
                            base._appLogger.LogCritical($"On Challenge Day 0, Found Challenge Smokes! Invalid App State: User Id :{userId}, Challenge Id {challenge.Id}");

                            await base.InternalErrorMessageToUser();
                        }

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

                        // First Challenge Visit without smoked cigares
                        if (currentDayChallenge == null)
                        {
                            this.IsSmoking = false;

                            var firstDayChallengeSmokes = challenge
                                .ChallengeSmokes.FirstOrDefault(e => !e.IsDeleted && !e.IsCompleted && e.DayOfChallenge == 1);

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
                                    this.TimeToNextSmoke = new TimeSpan(0,0, secondsTimeToNext);

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
                .ConfirmAsync(AppResources.UnderTestViewModelStopSmokeConfirmMessage,
                    AppResources.UnderTestViewModelStopSmokeConfirmTitle,
                    AppResources.YesButtonText,
                    AppResources.NoButtonText);

            if (userNotification)
            {
                await MarkDayCompleted();
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Redo One Day Of Challenge
        /// </summary>
        /// <returns></returns>
        private async Task BackOneChallengeDay()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Mark One Cigar Smoked
        /// </summary>
        /// <param name="delayed"></param>
        /// <returns></returns>
        private async Task SmokeOneCigareAsync(bool delayed = false)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Set Complete Test
        /// </summary>
        private async void CompleteChallenge()
        {
            throw new NotImplementedException();
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
