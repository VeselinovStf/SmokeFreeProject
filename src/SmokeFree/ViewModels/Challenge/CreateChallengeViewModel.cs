using Realms;
using SmokeFree.Abstraction.Services.Data.Test;
using SmokeFree.Abstraction.Services.General;
using SmokeFree.Abstraction.Utility.Logging;
using SmokeFree.Abstraction.Utility.Wrappers;
using SmokeFree.Data.Models;
using SmokeFree.Models.Views.Challenge;
using SmokeFree.Resx;
using SmokeFree.Utilities.UserStateHelpers;
using SmokeFree.ViewModels.AppSettings;
using SmokeFree.ViewModels.Base;
using SmokeFree.ViewModels.Test;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.ObjectModel;

namespace SmokeFree.ViewModels.Challenge
{
    /// <summary>
    /// CreateChallengeVie Model
    /// </summary>
    public class CreateChallengeViewModel : ViewModelBase
    {
        #region FIELDS

        /// <summary>
        /// Database
        /// </summary>
        private readonly Realm _realm;

        private readonly IChallengeCalculationService _challengeCalculationService;

        /// <summary>
        /// Smoke Free Goal Completition DateTime
        /// </summary>
        private DateTime _goalCompletitionTime;

        /// <summary>
        /// Current challenge id, set by Initialize
        /// </summary>
        private string _currentChallengeId;

        /// <summary>
        /// User Smoke Status
        /// </summary>
        private UserSmokeStatusItem _userSmokeStatus;

        #endregion

        #region CTOR

        public CreateChallengeViewModel(
            Realm realm,
            INavigationService navigationService,
            IDateTimeWrapper dateTimeWrapper,
            IAppLogger appLogger,
            IDialogService dialogService,
            IChallengeCalculationService challengeCalculationService) : base(navigationService, dateTimeWrapper, appLogger, dialogService)
        {
            // Database
            _realm = realm;

            _challengeCalculationService = challengeCalculationService;

            // View Title
            ViewTitle = AppResources.CreateChallengeViewTitle;
        }

        #endregion

        #region INIT

        /// <summary>
        /// Initialize View Initial State
        /// </summary>
        public override async Task InitializeAsync(object parameter)
        {
            try
            {
                // Get User From DB
                var userId = Globals.UserId;
                var user = _realm.Find<User>(userId);

                // Validate User
                if (user != null)
                {
                    var challenge = user.Challenges
                        .FirstOrDefault(e => !e.IsDeleted);

                    // Validate User Challenge
                    if (challenge != null)
                    {
                        // Set GoalCompletitionTime
                        var currentGoaSelectedCompletitionTestTime = challenge.GoalCompletitionTime.LocalDateTime;
                        if (currentGoaSelectedCompletitionTestTime < this._dateTime.Now())
                        {
                            // Notify User if GoalCompletitionTime < DateTime.Now
                            await base._dialogService.ShowDialog(
                                AppResources.CreateChallengeViewModelNewGoalDialogMessage,
                                AppResources.CreateChallengeViewModelNewGoalDialogTitle,
                                AppResources.ButtonOkText);   
                        }
                        else
                        {
                            this.GoalCompletitionTime = currentGoaSelectedCompletitionTestTime;
                        }

                        // Set Initial Challenge Id
                        this.CurrentChallengeId = challenge.Id;

                        var userSmokerStatus = StringToEnum.ToUserState<UserSmokeStatuses>(user.UserSmokeStatuses);
                        var userSmokerStatusIcon = Globals.UserSmokeStatusesSet[userSmokerStatus].First().Item1;
                        var userSmokerStatusMessage = Globals.UserSmokeStatusesSet[userSmokerStatus].First().Item2;

                        this.UserSmokeStatus = new UserSmokeStatusItem()
                        {
                            Icon = userSmokerStatusIcon,
                            Title = AppResources.UserSmokeStatusTitle,
                            Message = userSmokerStatusMessage
                        };
                    }
                    else
                    {
                        // User Challenge Not Found!
                        base._appLogger.LogCritical($"Can't find User Challenge:User Id {userId}");

                        await base.InternalErrorMessageToUser();
                    }                   
                }
                else
                {
                    // User Not Found!
                    base._appLogger.LogCritical($"Can't find User:User Id {userId}");

                    await base.InternalErrorMessageToUser();
                }

            }
            catch (Exception ex)
            {
                base._appLogger.LogCritical(ex);

                await base.InternalErrorMessageToUser();
            }

        }

        #endregion

        #region COMMANDS

        /// <summary>
        /// Starts Challenge
        /// </summary>
        public IAsyncCommand OnStartChallengeCommand => new AsyncCommand(
            ExecuteStartChallenge,
            onException: base.GenericCommandExeptionHandler,
            allowsMultipleExecutions: false);

        private async Task ExecuteStartChallenge()
        {
            // Get GoalCompletitionTime
            var goalTime = this.GoalCompletitionTime;

            // Check if GoalCompletitionTime is > of constraints!!!
            if (goalTime < this._dateTime.Now().AddDays(Globals.MinChallengeDays))
            {
                await base._dialogService.ShowDialog(
                    AppResources.CreateChellengeViewModelGoalTimeNotificationMessage,
                    AppResources.CreateChellengeViewModelGoalTimeNotificationTitle,
                    AppResources.ButtonOkText);
            }
            else
            {
                // Notify user if is shour
                var checkIfUserIsShour = await base._dialogService
                    .ConfirmAsync(
                    AppResources.CreateChellengeViewModelStartChallengeConfirmMessage,
                    AppResources.CreateChellengeViewModelStartChallengeConfirmTitle,
                    AppResources.YesButtonText, 
                    AppResources.NoButtonText);

                if (checkIfUserIsShour)
                {
                    var userId = Globals.UserId;
                    var user = _realm.Find<User>(userId);

                    if (user == null)
                    {
                        // User Not Found!
                        base._appLogger.LogCritical($"Can't find User: User Id {userId}");

                        await base.InternalErrorMessageToUser();
                    }

                    var userTest = user.Tests
                        .FirstOrDefault(e => !e.IsDeleted && e.IsCompleted);

                    if (userTest == null)
                    {
                        // User Not Found!
                        base._appLogger.LogCritical($"Can't find User Test: User Id {userId}");

                        await base.InternalErrorMessageToUser();
                    }

                    var testResult = userTest.CompletedTestResult;

                    if (testResult == null)
                    {
                        // User Not Found!
                        base._appLogger.LogCritical($"Can't find User Test Results: User Id {userId}");

                        await base.InternalErrorMessageToUser();
                    }

                    var goalTimeInDays = (int)Math.Abs((goalTime - _dateTime.Now()).Days);
                    var avarageSmokeADay = testResult.AvarageSmokedCigarsPerDay;
                    var avarageSmokeActiveTime = testResult.AvarageSmokeActiveTimeSeconds;

                    var challenge = user.Challenges
                        .FirstOrDefault(e => !e.IsDeleted);

                    if (challenge == null)
                    {
                        // User Not Found!
                        base._appLogger.LogCritical($"Can't find User Challenge: User Id {userId}");

                        await base.InternalErrorMessageToUser();
                    }

                    var challengeCalculations = this._challengeCalculationService
                        .CalculatedChallengeSmokes(goalTimeInDays, avarageSmokeADay, avarageSmokeActiveTime, challenge.Id);

                    if (challengeCalculations.Success)
                    {
                        _realm.Write(() =>
                        {
                            // Update Challenge
                            challenge.ChallengeStart = this._dateTime.Now();
                            challenge.GoalCompletitionTime = goalTime;
                            challenge.ModifiedOn = this._dateTime.Now();

                            foreach (var dcs in challengeCalculations.DayChallengeSmokes)
                            {
                                dcs.CreatedOn = this._dateTime.Now();
                                challenge.ChallengeSmokes.Add(dcs);
                            }

                            user.UserState = UserStates.InChallenge.ToString();


                        });

                        await this._navigationService.NavigateToAsync<ChallengeViewModel>();
                    }
                    else
                    {
                        // User Not Found!
                        base._appLogger.LogError($"Can't Calculate New Challenge Data: User Id {userId}, Challenge Id: {challenge.Id}");

                        await base.InternalErrorMessageToUser();
                    }
                }
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

        /// <summary>
        /// Navigate back to Test Result View
        /// </summary>
        public IAsyncCommand OnBackCommand => new AsyncCommand(
            ExecuteNavigateToBack,
            onException: base.GenericCommandExeptionHandler,
            allowsMultipleExecutions: false);

        private async Task ExecuteNavigateToBack()
        {
            await base._navigationService.NavigateToAsync<TestResultViewModel>();
        }

        #endregion

        #region PROPS

        /// <summary>
        /// Smoke Free Goal Completition DateTime
        /// </summary>
        public DateTime GoalCompletitionTime
        {
            get { return _goalCompletitionTime; }
            set
            {
                _goalCompletitionTime = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Challenge ID
        /// </summary>
        public string CurrentChallengeId
        {
            get { return _currentChallengeId; }
            set 
            { 
                _currentChallengeId = value;
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
