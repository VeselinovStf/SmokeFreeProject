using Plugin.LocalNotification;
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
using System.Linq;
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

            InitiateTestTimeLanguages();
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

                    var culture = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;

                    var currentlySelectedLanguage = this.Languages.FirstOrDefault(e => e.Value == culture);

                    this.SelectedTLanguageItem = currentlySelectedLanguage;
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
        private void InitiateTestTimeLanguages()
        {
            this.Languages.Clear();

            var languages = new List<LanguageItem>()
            {
                new LanguageItem()
                {
                     Value = "bg",
                     DisplayText = "Sa"
                },
                new LanguageItem()
                {
                     Value = "en",
                     DisplayText =  "English (United States)"
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
                // Cleare DB
                _realm.Write(() =>
                {
                    _realm.RemoveAll<User>();
                    _realm.RemoveAll<Data.Models.Test>();
                    _realm.RemoveAll<Data.Models.Challenge>();
                    _realm.RemoveAll<TestResult>();
                    _realm.RemoveAll<Smoke>();
                    _realm.RemoveAll<DayChallengeSmoke>();
                });

                // Stop Testing Time Notification
                NotificationCenter.Current.Cancel(Globals.TestingTimeNotificationId);

                // Stop Smoke Delayed Time Notification
                NotificationCenter.Current.Cancel(Globals.DelayedSmokeNotificationId);

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


        /// <summary>
        /// Visit Dev Web Site 
        /// </summary>
        public IAsyncCommand VisitWebsiteCommand => new AsyncCommand(
            VisitWebsite,
            onException: base.GenericCommandExeptionHandler,
            allowsMultipleExecutions: false);

        private async Task VisitWebsite()
        {
            //TODO: Visit Website
        }

        /// <summary>
        /// Rank App In Store
        /// </summary>
        public IAsyncCommand RankAppCommand => new AsyncCommand(
            RankApp,
            onException: base.GenericCommandExeptionHandler,
            allowsMultipleExecutions: false);

        private async Task RankApp()
        {
            //TODO: Rank App
        }

        /// <summary>
        /// Send FeedBack Data to Dev team
        /// </summary>
        public IAsyncCommand SendFeedBackDataCommand => new AsyncCommand(
            SendFeedBackData,
            onException: base.GenericCommandExeptionHandler,
            allowsMultipleExecutions: false);

        private async Task SendFeedBackData()
        {
            //TODO: Send FeedBack Data
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
                if (!value)
                {
                    // Stop Testing Time Notification
                    NotificationCenter.Current.Cancel(Globals.TestingTimeNotificationId);

                    // Stop Smoke Delayed Time Notification
                    NotificationCenter.Current.Cancel(Globals.DelayedSmokeNotificationId);
                }

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
