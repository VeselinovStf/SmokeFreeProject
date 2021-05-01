using Realms;
using SmokeFree.Abstraction.Services.General;
using SmokeFree.Abstraction.Utility.Logging;
using SmokeFree.Abstraction.Utility.Wrappers;
using SmokeFree.Data.Models;
using SmokeFree.Resx;
using SmokeFree.ViewModels.AppSettings;
using SmokeFree.ViewModels.Base;
using SmokeFree.ViewModels.Challenge;
using System;
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

        /// <summary>
        /// Database
        /// </summary>
        private readonly Realm _realm;

        #endregion

        #region CTOR

        public TestResultViewModel(
            Realm realm,
            INavigationService navigationService,
            IDateTimeWrapper dateTimeWrapper,
            IAppLogger appLogger,
            IDialogService dialogService) : base(navigationService, dateTimeWrapper, appLogger, dialogService)
        {
            // Set View Title
            ViewTitle = AppResources.TestResultViewModelTiitle;

            // Database
            _realm = realm;
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

                            var testChallenge = user.Challenges.FirstOrDefault(c => c.UserId == userId && !c.IsDeleted);

                            // Remove Challenge
                            testChallenge.IsDeleted = true;
                            testChallenge.DeletedOn = this._dateTime.Now();
                            testChallenge.ModifiedOn = this._dateTime.Now();

                            // Remove Test Result
                            var userTestResults = userTest.CompletedTestResult;
                            if (userTestResults != null)
                            {
                                userTestResults.IsDeleted = true;
                                userTestResults.DeletedOn = this._dateTime.Now();
                                userTestResults.ModifiedOn = this._dateTime.Now();
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
            await base._navigationService.NavigateToAsync<CreateChallengeViewModel>();
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

        #endregion
    }
}
