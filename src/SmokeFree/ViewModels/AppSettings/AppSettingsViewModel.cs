using Realms;
using SmokeFree.Abstraction.Services.General;
using SmokeFree.Abstraction.Utility.Logging;
using SmokeFree.Abstraction.Utility.Wrappers;
using SmokeFree.Data.Models;
using SmokeFree.Resx;
using SmokeFree.ViewModels.Base;
using SmokeFree.ViewModels.OnBoarding;
using System;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.ObjectModel;

namespace SmokeFree.ViewModels.AppSettings
{
    /// <summary>
    /// AppSetting View Model
    /// </summary>
    public class AppSettingsViewModel : ViewModelBase
    {
        #region FIELDS

        /// <summary>
        /// Switch notifications on/ off
        /// </summary>
        private bool _notificationSwitch;

        /// <summary>
        /// Database
        /// </summary>
        private readonly Realm _realm;

        /// <summary>
        /// Application User
        /// </summary>
        private User _appUser;

        #endregion

        #region CTOR

        public AppSettingsViewModel(
            Realm realm,
            INavigationService navigationService,
            IDateTimeWrapper dateTimeWrapper,
            IAppLogger appLogger,
            IDialogService dialogService) : base(navigationService, dateTimeWrapper, appLogger, dialogService)
        {
            // Set View Title
            ViewTitle = AppResources.AppSettingsViewTitle;

            // Database
            _realm = realm;
        }

        #endregion

        #region INIT

        public override async Task InitializeAsync(object parameter)
        {
            try
            {
                // SET Global User Id
                var globalUserId = Globals.UserId;

                // Get User By Global Id
                var user = _realm.Find<User>(globalUserId);

                // For First Run Of App 
                if (user != null)
                {
                    this.AppUser = user;
                    this.NotificationSwitch = user.NotificationState;
                }
                else
                {
                    // User Not Found!
                    base._appLogger.LogCritical($"Can't find User: User Id {globalUserId}");

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
        /// Navigate back to previous command
        /// </summary>
        public IAsyncCommand NavigateBackCommand => new AsyncCommand(
            NavigateBack,
            onException: base.GenericCommandExeptionHandler,
            allowsMultipleExecutions: false);

        private async Task NavigateBack()
        {
            await base._navigationService.RemoveLastFromBackStackAsync();
        }

        /// <summary>
        /// Navigate back to previous command
        /// </summary>
        public IAsyncCommand StartTutorialCommand => new AsyncCommand(
            StartTutorial,
            onException: base.GenericCommandExeptionHandler,
            allowsMultipleExecutions: false);

        private async Task StartTutorial()
        {          
            await base._navigationService.NavigateToAsync<OnBoardingViewModel>();
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Change Notifications state in App Preferencess
        /// </summary>
        /// <param name="value"></param>
        private void ChangeNotificationsState(bool value)
        {
            try
            {
                this._realm.Write(() =>
                {
                    this.AppUser.NotificationState = value;
                });
            }
            catch (Exception ex)
            {
                base._appLogger.LogError(ex.Message);

                base.InternalErrorMessageToUser().Wait();
            }

        }

        #endregion

        #region PROPS

        /// <summary>
        /// Switch notifications on/ off
        /// </summary>
        public bool NotificationSwitch
        {
            get { return _notificationSwitch; }
            set
            {
                _notificationSwitch = value;
                OnPropertyChanged();
                ChangeNotificationsState(value);
            }
        }

        /// <summary>
        /// Application User
        /// </summary>
        public User AppUser
        {
            get { return _appUser; }
            set
            {
                _appUser = value;
            }
        }

        #endregion
        
    }
}
