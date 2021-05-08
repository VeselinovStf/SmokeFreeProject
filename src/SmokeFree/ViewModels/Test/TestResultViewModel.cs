using Realms;
using SmokeFree.Abstraction.Services.Data.Test;
using SmokeFree.Abstraction.Services.General;
using SmokeFree.Abstraction.Utility.Logging;
using SmokeFree.Abstraction.Utility.Wrappers;
using SmokeFree.Data.Models;
using SmokeFree.Models.Views.Test;
using SmokeFree.Resx;
using SmokeFree.Utilities.Parsers;
using SmokeFree.Utilities.UserStateHelpers;
using SmokeFree.ViewModels.AppSettings;
using SmokeFree.ViewModels.Base;
using SmokeFree.ViewModels.Challenge;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.ObjectModel;

namespace SmokeFree.ViewModels.Test
{
    /// <summary>
    /// TestResultView Model
    /// </summary>
    public class TestResultViewModel : ViewModelBase
    {
        #region FIELDS

        /// <summary>
        /// Test Results
        /// </summary>
        private TestResult _testResult;

        private bool _inCreateChallenge;

        /// <summary>
        /// Database
        /// </summary>
        private readonly Realm _realm;

        private readonly ITestCalculationService _testCalculationService;

        /// <summary>
        /// Test Results Elements Collection
        /// </summary>
        private ObservableCollection<TestResultItem> _testResults;


        #endregion

        #region CTOR

        public TestResultViewModel(
            Realm realm,
            INavigationService navigationService,
            IDateTimeWrapper dateTimeWrapper,
            IAppLogger appLogger,
            IDialogService dialogService,
            ITestCalculationService testCalculationService) : base(navigationService, dateTimeWrapper, appLogger, dialogService)
        {
            // Set View Title
            ViewTitle = AppResources.TestResultViewModelTiitle;

            // Database
            _realm = realm;

            _testCalculationService = testCalculationService;

            _testResults = new ObservableCollection<TestResultItem>();
        }

        #endregion

        #region INIT

        /// <summary>
        /// Initialize View Model Initial State
        /// </summary>
        /// <param name="parameter">PARAM</param>
        public override async Task InitializeAsync(object parameter)
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
                        .FirstOrDefault(t => t.Id == testId && !t.IsDeleted);

                    // Validate User Test
                    if (userTest != null)
                    {
                        var testResult = userTest.CompletedTestResult;


                        // Validate Test Result
                        if (testResult != null)
                        {
                            this.TestResult = testResult;

                            this.TestResults.Clear();

                            TestResults = new ObservableCollection<TestResultItem>()
                            {
                                new TestResultItem()
                                {
                                    Icon = Globals.UserSmokeStatusesSet[StringToEnum.ToUserState<UserSmokeStatuses>(user.UserSmokeStatuses)].First().Item1,
                                    DisplayTitle = AppResources.UserSmokeStatusTitle,
                                    Value = Globals.UserSmokeStatusesSet[StringToEnum.ToUserState<UserSmokeStatuses>(user.UserSmokeStatuses)].First().Item2,
                                    Description = AppResources.UserSmokeStatusDescription
                                },
                                new TestResultItem()
                                {
                                    Icon = "\ue06a",
                                    DisplayTitle = AppResources.TestResultTestTimeLabel,
                                    Value = DoubleToString.DateTime(TestResult.TotalTestTimeSeconds),
                                    Description = AppResources.TestResultTestTimeDescription
                                },
                                new TestResultItem()
                                {
                                    Icon = "\ue728",
                                    DisplayTitle = AppResources.TestResultStartedDateLabel,
                                    Value = DateTimeOfsetToString.DateTime(TestResult.TestStartDate),
                                    Description = AppResources.TestResultStartedDateDescription
                                },
                                new TestResultItem()
                                {
                                    Icon = "\ue728",
                                    DisplayTitle = AppResources.TestResultEndDateLabel,
                                    Value = DateTimeOfsetToString.DateTime(TestResult.EndStartDate),
                                    Description = AppResources.TestResultEndDateDescription
                                },
                                new TestResultItem()
                                {
                                    Icon = "\ueb78",
                                    DisplayTitle = AppResources.TestResultSmokedCigarsLabel,
                                    Value = TestResult.TotalSmokedCigars.ToString(),
                                    Description = AppResources.TestResultSmokedCigarsDescription
                                },
                                new TestResultItem()
                                {
                                    Icon = "\ue0c8",
                                    DisplayTitle = AppResources.TestResultAvarageForDayLabel,
                                    Value = DoubleToString.Procent(TestResult.AvarageSmokedCigarsPerDay),
                                    Description = AppResources.TestResultAvarageForDayDescription
                                },
                                new TestResultItem()
                                {
                                    Icon = "\ue0f1",
                                    DisplayTitle = AppResources.TestResultAvarageCleanOxygenTimeLabel,
                                    Value = DoubleToString.DateTime(TestResult.AvarageCleanOxygenTimeSeconds),
                                    Description = AppResources.TestResultAvarageCleanOxygenTimeDescription
                                },
                                new TestResultItem()
                                {
                                    Icon = "\ue0ab",
                                    DisplayTitle = AppResources.TestResultTotalSmokedGazTime,
                                    Value = DoubleToString.DateTime(TestResult.TotalSmokeGasTimeTimeSeconds),
                                    Description = AppResources.TestResultTotalSmokedGazTimeDescription
                                },
                                new TestResultItem()
                                {
                                    Icon = "\ue044",
                                    DisplayTitle = AppResources.TestResultAvarageSmokeDistance,
                                    Value = DoubleToString.DateTime(TestResult.AvarageSmokeDistanceSeconds),
                                    Description = AppResources.TestResultAvarageSmokeDistanceDescription
                                },
                                new TestResultItem()
                                {
                                    Icon = "\ue044",
                                    DisplayTitle = AppResources.TestResultAvarageSmokeActiveTimeSeconds,
                                    Value = DoubleToString.DateTime(TestResult.AvarageSmokeActiveTimeSeconds),
                                    Description = AppResources.TestResultAvarageSmokeActiveTimeSecondsDescription
                                },
                                new TestResultItem()
                                {
                                    Icon = "\ue044",
                                    DisplayTitle = AppResources.TestResultAvarageSmokingTimeSeconds,
                                    Value = DoubleToString.DateTime(TestResult.AvarageSmokingTimeSeconds),
                                    Description = AppResources.TestResultAvarageSmokingTimeSecondsDescription
                                },
                                new TestResultItem()
                                {
                                    Icon = "\ue044",
                                    DisplayTitle = AppResources.TestResultTotalCleanOxygenSeconds,
                                    Value = DoubleToString.DateTime(TestResult.TotalCleanOxygenSeconds),
                                    Description = AppResources.TestResultTotalCleanOxygenSecondsDescription
                                },

                            };

                            var userState = StringToEnum.ToUserState<UserStates>(user.UserState);
                            if (userState == UserStates.InCreateChallenge)
                            {
                                this.InCreateChallenge = true;
                            }

                        }
                        else
                        {
                            // User Test Result Not Found!
                            base._appLogger.LogCritical($"Can't find User Test Results: User id: {userId}, Test Id {testId}");

                            await base.InternalErrorMessageToUser();
                        }
                    }
                    else
                    {
                        // User Test Not Found!
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

        #endregion

        #region COMMANDS

        /// <summary>
        /// Navigate to settings View
        /// </summary>
        public IAsyncCommand OnSettingsCommand => new AsyncCommand(
            ExecuteNavigateToSetting,
            onException: base.GenericCommandExeptionHandler,
            allowsMultipleExecutions: false);

        private async Task ExecuteNavigateToSetting()
        {
            await base._navigationService.NavigateToAsync<AppSettingsViewModel>();
        }

        /// <summary>
        /// Navigate back
        /// </summary>
        public IAsyncCommand BackToCreateChallengeCommand => new AsyncCommand(
            BackToCreateChallenge,
            onException: base.GenericCommandExeptionHandler,
            allowsMultipleExecutions: false);

        private async Task BackToCreateChallenge()
        {
            await base._navigationService.BackToPreviousAsync();
        }

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
                            var userTest = user.Tests.FirstOrDefault(t => t.UserId == userId && !t.IsDeleted);

                            if (userTest != null)
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

                                // Remove Test Result
                                var userTestResults = userTest.CompletedTestResult;
                                if (userTestResults != null)
                                {
                                    userTestResults.IsDeleted = true;
                                    userTestResults.DeletedOn = this._dateTime.Now();
                                    userTestResults.ModifiedOn = this._dateTime.Now();
                                }
                                else
                                {
                                    base._appLogger.LogCritical($"Can't find User Test Result: User Id {userId}, User Test Id {user.TestId}");

                                }
                            }

                            var testChallenge = user.Challenges.FirstOrDefault(c => c.UserId == userId && !c.IsDeleted);

                            if (testChallenge != null)
                            {
                                // Remove Challenge
                                testChallenge.IsDeleted = true;
                                testChallenge.DeletedOn = this._dateTime.Now();
                                testChallenge.ModifiedOn = this._dateTime.Now();

                            }

                            // Update User Status
                            user.UserState = UserStates.CompletedOnBoarding.ToString();
                            user.TestId = string.Empty;
                        });

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
                base._appLogger.LogCritical(ex);

                await base.InternalErrorMessageToUser();
            }
        }

        /// <summary>
        /// Navigate to Create Challenge View
        /// </summary>
        public IAsyncCommand OnCreateChallengeCommand => new AsyncCommand(
            ExecuteNavigateToCreateChallenge,
            onException: base.GenericCommandExeptionHandler,
            allowsMultipleExecutions: false);

        private async Task ExecuteNavigateToCreateChallenge()
        {
            var userId = Globals.UserId;
            var user = _realm.Find<User>(userId);

            if (user != null)
            {
                _realm.Write(() =>
                {
                    user.UserState = UserStates.InCreateChallenge.ToString();
                    user.ModifiedOn = this._dateTime.Now();
                });

                await base._navigationService.NavigateToAsync<CreateChallengeViewModel>();
            }
            else
            {
                // User Not Found!
                base._appLogger.LogCritical($"Can't find User: User Id {userId}");

                await base.InternalErrorMessageToUser();
            }
        }

        #endregion

        #region PROPS

        /// <summary>
        /// Current Display Test Results
        /// </summary>
        public TestResult TestResult
        {
            get { return _testResult; }
            set
            {
                _testResult = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Hide Create Challenge Button if is in Create
        /// </summary>
        public bool InCreateChallenge
        {
            get { return _inCreateChallenge; }
            set
            {
                _inCreateChallenge = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Test Results Elements Collection
        /// </summary>
        public ObservableCollection<TestResultItem> TestResults
        {
            get { return _testResults; }
            set
            {
                _testResults = value;
                OnPropertyChanged();
            }
        }


        #endregion
    }
}
