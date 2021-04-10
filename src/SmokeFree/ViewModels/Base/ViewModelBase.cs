using SmokeFree.Abstraction.Services.General;
using SmokeFree.Abstraction.Utility.Logging;
using SmokeFree.Abstraction.Utility.Wrappers;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SmokeFree.ViewModels.Base
{
    /// <summary>
    /// Base Class For View Models. Implements INotifyPropertyChanged
    /// </summary>
    public class ViewModelBase : INotifyPropertyChanged
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

        #region EVENTS

        /// <summary>
        /// Property Changed event
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region METHODS

        /// <summary>
        /// Invoke on Property Change
        /// </summary>
        /// <param name="propertyName">Optional: Changable Property Name</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Display Internal Error Message To User - Device Toast With Error Message
        /// </summary>
        /// <param name="message">Message to display</param>
        protected virtual void InternalErrorMessageToUser(string message = Globals.InternalErrorUserMessage)
        {
            this._dialogService.ShowToast(message);
        }

        /// <summary>
        /// Handle Exeptions Thrown by Commands
        /// </summary>
        /// <param name="ex">Thrown Exception</param>
        protected virtual void GenericCommandExeptionHandler(Exception ex)
        {
            // Log Error
            this._appLogger.LogError(ex.Message);

            // Display default internal error message to user
            this.InternalErrorMessageToUser();
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
