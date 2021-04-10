using SmokeFree.Abstraction.Services.General;
using SmokeFree.Abstraction.Utility.Logging;
using SmokeFree.Abstraction.Utility.Wrappers;
using System.Threading.Tasks;

namespace SmokeFree.ViewModels.Base
{
    /// <summary>
    /// Base Model For ViewModels
    /// </summary>
    public class ViewModelBase
    {
        #region FIELDS

        /// <summary>
        /// View Title
        /// </summary>
        private string _viewTitle;

        /// <summary>
        /// Application Navigation Service
        /// </summary>
        protected readonly INavigationService _navigationService;

        /// <summary>
        /// Application DateTime Provider
        /// </summary>
        protected readonly IDateTimeWrapper _dateTime;

        /// <summary>
        /// Application Logger
        /// </summary>
        protected readonly IAppLogger _appLogger;

        /// <summary>
        /// Application Dialog Service
        /// </summary>
        protected readonly IDialogService _dialogService;

        #endregion

        #region CTOR

        public ViewModelBase(
            INavigationService navigationService,
            IDateTimeWrapper dateTimeWrapper,
            IAppLogger appLogger,
            IDialogService dialogService)
        {
            _navigationService = navigationService;
            _dateTime = dateTimeWrapper;
            _appLogger = appLogger;
            _dialogService = dialogService;
        }

        #endregion

        #region INIT

        /// <summary>
        /// Initialize ViewModel
        /// </summary>
        /// <param name="parameter">View Model Parameter</param>
        /// <returns>Completed Task</returns>
        public virtual Task InitializeAsync(object parameter)
        {
            return Task.CompletedTask;
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Display Internal Error Message To User - Device Toast With Error Message
        /// </summary>
        /// <param name="message">Message to display</param>
        protected virtual void InternalErrorMessageToUser(string message = Globals.InternalErrorUserMessage)
        {
            this._dialogService.ShowToast(message);
        }

        #endregion

        #region PROPS

        /// <summary>
        /// Title Of Each view
        /// </summary>
        public string ViewTitle
        {
            get { return _viewTitle; }
            set
            {
                if (value != null)
                {
                    _viewTitle = value;                    
                }
            }
        }

        #endregion
    }
}
