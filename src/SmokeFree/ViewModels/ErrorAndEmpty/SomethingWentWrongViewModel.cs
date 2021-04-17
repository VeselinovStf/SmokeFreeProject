using SmokeFree.Abstraction.Services.General;
using SmokeFree.Abstraction.Utility.Logging;
using SmokeFree.Abstraction.Utility.Wrappers;
using SmokeFree.Resx;
using SmokeFree.ViewModels.Base;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.ObjectModel;

namespace SmokeFree.ViewModels.ErrorAndEmpty
{
    /// <summary>
    /// ViewModel for something went wrong page.
    /// </summary>
    public class SomethingWentWrongViewModel : ViewModelBase
    {
        #region FIELDS

        /// <summary>
        /// Display image path
        /// </summary>
        private string _imagePath;

        /// <summary>
        /// View Header
        /// </summary>
        private string _header;

        /// <summary>
        /// View Content
        /// </summary>
        private string _content;

        /// <summary>
        /// Try Again Command Text
        /// </summary>
        private string _tryAgainCommandText;

        /// <summary>
        /// Report Issue Command Text
        /// </summary>
        private string _reportIssueCommandText;

        /// <summary>
        /// Application Logging Utility
        /// </summary>
        private readonly ILocalLogUtility _localLogUtility;

        #endregion

        #region CTOR

        public SomethingWentWrongViewModel(
            INavigationService navigationService, 
            IDateTimeWrapper dateTimeWrapper, 
            IAppLogger appLogger, 
            IDialogService dialogService,
            ILocalLogUtility localLogUtility) : base(navigationService, dateTimeWrapper, appLogger, dialogService)
        {
            // Set View Title
            base.ViewTitle = AppResources.SomethingWentWrongViewModelTitle;

            // Application Logging Utility
            this._localLogUtility = localLogUtility;

            // Set View Image 
            this._imagePath = AppResources.SomethingWentWrongViewModelImage;

            // Set View Header
            this.Header = AppResources.SomethingWentWrongViewModelHeader;

            // Set View Content
            this.Content = AppResources.InternalErrorUserMessage;

            // Set Try Again Button Text
            this.TryAgainCommandText = AppResources.SomethingWentWrongViewModelTryAgainButtonText;

            // Report Issue Command Text
            this.ReportIssueCommandText = AppResources.SomethingWentWrongViewModelReportIssueButtonText;           
        }

        #endregion

        #region COMMANDS

        /// <summary>
        /// Navigate back to previous view
        /// </summary>
        public IAsyncCommand TryAgainCommand => new AsyncCommand(
            ExecuteTryAgain,
            onException: base.GenericCommandExeptionHandler,
            allowsMultipleExecutions: false);

        private async Task ExecuteTryAgain()
        {           
            await base._navigationService.RemoveLastFromBackStackAsync();
        }

        /// <summary>
        /// Report Application Crash Command
        /// </summary>
        public IAsyncCommand ReportIssueCommand => new AsyncCommand(
            ExecuteReportIssue,
            onException: base.GenericCommandExeptionHandler,
            allowsMultipleExecutions: false);

        private async Task ExecuteReportIssue()
        {          
            try
            {
                // Get Archived Logs
                var archivedLogsUtilityResponse = this._localLogUtility
                    .CreateLogZipFile();

                // Check if Logs are Archived
                if (archivedLogsUtilityResponse.Created)
                {
                    // Send Them To Dev Team Email
                    // Notify User
                }
                else
                {

                }
            
            }
            catch (System.Exception ex)
            {
                base._appLogger.LogError(ex.Message);

                await base.InternalErrorMessageToUser();
            }

            await Task.CompletedTask;
        }

        #endregion

        #region PROPS


        /// <summary>
        /// Gets or sets the ImagePath.
        /// </summary>
        public string ImagePath
        {
            get
            {
                return this._imagePath;
            }

            set
            {
                this._imagePath = value;
                base.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the Header.
        /// </summary>
        public string Header
        {
            get
            {
                return this._header;
            }

            set
            {
                this._header = value;
                base.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the Content.
        /// </summary>
        public string Content
        {
            get
            {
                return this._content;
            }

            set
            {
                this._content = value;
                base.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Try Again Button Command Text
        /// </summary>
        public string TryAgainCommandText
        {
            get
            {
                return this._tryAgainCommandText;
            }

            set
            {
                this._tryAgainCommandText = value;
                base.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Report Issue Command Text
        /// </summary>
        public string ReportIssueCommandText
        {
            get
            {
                return this._reportIssueCommandText;
            }

            set
            {
                this._reportIssueCommandText = value;
                base.OnPropertyChanged();
            }
        }

        #endregion
    }
}
