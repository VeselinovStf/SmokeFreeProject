using Realms;
using SmokeFree.Abstraction.Services.General;
using SmokeFree.Abstraction.Utility.Logging;
using SmokeFree.Abstraction.Utility.Wrappers;
using SmokeFree.Data.Models;
using SmokeFree.Resx;
using SmokeFree.ViewModels.AppSettings;
using SmokeFree.ViewModels.Base;
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
