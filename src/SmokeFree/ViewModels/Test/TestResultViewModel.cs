using Realms;
using SmokeFree.Abstraction.Services.General;
using SmokeFree.Abstraction.Utility.Logging;
using SmokeFree.Abstraction.Utility.Wrappers;
using SmokeFree.Data.Models;
using SmokeFree.Resx;
using SmokeFree.ViewModels.AppSettings;
using SmokeFree.ViewModels.Base;
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
                        .FirstOrDefault(t => t.Id == testId);

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
