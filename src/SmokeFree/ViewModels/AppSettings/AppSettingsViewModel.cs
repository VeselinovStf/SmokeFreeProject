using Realms;
using SmokeFree.Abstraction.Services.General;
using SmokeFree.Abstraction.Utility.Logging;
using SmokeFree.Abstraction.Utility.Wrappers;
using SmokeFree.Data.Models;
using SmokeFree.Models.Views.AppSetting;
using SmokeFree.Resx;
using SmokeFree.ViewModels.Base;
using SmokeFree.ViewModels.OnBoarding;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.Helpers;
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
        /// Test Time Duration Collection
        /// </summary>
        private ObservableCollection<LanguageItem> _languages;

        /// <summary>
        /// Switch notifications on/ off
        /// </summary>
        private bool _notificationSwitch;
        private LanguageItem _selectedTLanguageItem;

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

            _languages = new ObservableCollection<LanguageItem>();

            InitiateTestTimeDurations();
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
                base._appLogger.LogCritical(ex);

                await base.InternalErrorMessageToUser();
            }
        }

        /// <summary>
        /// Initialize Test Time Durations Collection
        /// </summary>
        private void InitiateTestTimeDurations()
        {
            this.Languages.Clear();

            var languages = new List<LanguageItem>()
            {
                new LanguageItem()
                {
                     Value = 2,
                     DisplayText =  "Bulgarian"
                },
                new LanguageItem()
                {
                     Value = 1,
                     DisplayText =  "English"
                }
            };

            foreach (var language in languages)
            {
                this.Languages.Add(language);
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
            await base._navigationService.BackToPreviousAsync();
        }

        /// <summary>
        /// Restart Application State
        /// </summary>
        public IAsyncCommand RestartAppCommand => new AsyncCommand(
            ExecuteRestartApp,
            onException: base.GenericCommandExeptionHandler,
            allowsMultipleExecutions: false);

        private async Task ExecuteRestartApp()
        {
            // Check if user is shore
            var userNotification = await base._dialogService
                 .ConfirmAsync(
                    "This acction is going to delete all save data!",
                    "Attention",
                    "Ok",
                    "Cancel"
                 );

            if (userNotification)
            {
                _realm.Write(() =>
                {
                    _realm.RemoveAll<User>();
                    _realm.RemoveAll<Data.Models.Test>();
                    _realm.RemoveAll<Data.Models.Challenge>();
                    _realm.RemoveAll<TestResult>();
                    _realm.RemoveAll<Smoke>();
                    _realm.RemoveAll<DayChallengeSmoke>();
                });

                await base._navigationService.InitializeAsync();
            }
           
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
                base._appLogger.LogCritical(ex);

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

        /// <summary>
        /// Test Time Duration for Picker
        /// </summary>
        public ObservableCollection<LanguageItem> Languages
        {
            get { return _languages; }
            set
            {
                if (value != null)
                {
                    _languages = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Selected Test Time Duration Item
        /// </summary>
        public LanguageItem SelectedTLanguageItem
        {
            get { return _selectedTLanguageItem; }
            set
            {
                if (value != null)
                {
                    _selectedTLanguageItem = value;

                    //TODO: A1 Change language
                    LocalizationResourceManager.Current.CurrentCulture = value == null ? CultureInfo.CurrentCulture : new CultureInfo(value.Value);

                    OnPropertyChanged();
                }
            }
        }

        #endregion

    }
}
