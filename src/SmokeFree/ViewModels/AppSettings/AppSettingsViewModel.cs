using Plugin.LocalNotification;
using Realms;
using SmokeFree.Abstraction.Services.General;
using SmokeFree.Abstraction.Utility.DeviceUtilities;
using SmokeFree.Abstraction.Utility.Logging;
using SmokeFree.Abstraction.Utility.Wrappers;
using SmokeFree.Bootstrap;
using SmokeFree.Data.Models;
using SmokeFree.Models.Views.AppSetting;
using SmokeFree.Resx;
using SmokeFree.Utilities.Logging;
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
using Xamarin.Essentials;

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
        /// Application Preferences store
        /// </summary>
        private readonly IAppPreferencesService _appPreferencesService;

        /// <summary>
        /// Network Connectivity Service 
        /// </summary>
        private readonly INetworkConnectionService _connectionService;

        /// <summary>
        /// Application Logging Utility
        /// </summary>
        private readonly ILocalLogUtility _localLogUtility;

        /// <summary>
        /// Device Email Sender
        /// </summary>
        private readonly IDeviceEmailSender _emailSender;

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
            IDialogService dialogService,
            IAppPreferencesService appPreferencesService,
            INetworkConnectionService networkConnectionService,
            ILocalLogUtility localLogUtility,
            IDeviceEmailSender deviceEmailSender) : base(navigationService, dateTimeWrapper, appLogger, dialogService)
        {
            // Set View Title
            ViewTitle = AppResources.AppSettingsViewTitle;

            // Database
            _realm = realm;

            // Application Preferences store
            _appPreferencesService = appPreferencesService;

            // Network Connectivity Service 
            _connectionService = networkConnectionService;

            // Application Logging Utility
            _localLogUtility = localLogUtility;

            // Device Email Sender
            _emailSender = deviceEmailSender;

            // App Availible Languges
            _languages = new ObservableCollection<LanguageItem>();

            // App Availible Colours Shemes
            //_colours = new ObservableCollection<ColourItem>();

            InitiateAppLanguages();
            //InitializeAppColourShemes();
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

                    var appPreferencesLocalization = this._appPreferencesService.LanguageValue;
                    var culture = string.Empty;
                    if (appPreferencesLocalization.Equals(string.Empty))
                    {
                        culture = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
                    }
                    else
                    {
                        culture = appPreferencesLocalization;
                    }

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
        private void InitiateAppLanguages()
        {
            this.Languages.Clear();

            var languages = new List<LanguageItem>()
            {
                new LanguageItem()
                {
                     Value = "bg",
                     DisplayText = AppResources.BulgarianLanguageDisplayLabel
                },
                new LanguageItem()
                {
                     Value = "en",
                     DisplayText =   AppResources.EnglishLanguageDisplayLabel
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
            try
            {
                if (this._connectionService.IsConnected)
                {
                    await Browser.OpenAsync(Globals.AppWebSiteUrl, BrowserLaunchMode.SystemPreferred);
                }
                else
                {
                    // Notify User
                    await this._dialogService
                        .ShowDialog(
                            AppResources.IssueNoWebConnectionMessageTitle,
                            AppResources.CantSendIssueEmailMessage,
                            AppResources.ButtonOkText);
                }
            }
            catch (Exception ex)
            {
                // User Not Found!
                base._appLogger.LogCritical($"Can't Open Application Web Site: {ex.Message}");

                this._dialogService
                    .ShowToast(AppResources.CantOpenAppWebSiteToastMessage);
            }
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
            try
            {
                if (this._connectionService.IsConnected)
                {
                    await Browser.OpenAsync(Globals.AppRankWebSiteUrl, BrowserLaunchMode.SystemPreferred);
                }
                else
                {
                    // Notify User
                    await this._dialogService
                        .ShowDialog(
                            AppResources.IssueNoWebConnectionMessageTitle,
                            AppResources.CantSendIssueEmailMessage,
                            AppResources.ButtonOkText);
                }
            }
            catch (Exception ex)
            {
                // User Not Found!
                base._appLogger.LogCritical($"Can't Open Application Web Site: {ex.Message}");

                this._dialogService
                    .ShowToast(AppResources.CantOpenAppWebSiteToastMessage);
            }
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
            try
            {
                var responseMessageTitle = string.Empty;
                var responseMessage = string.Empty;

                // Check If User is able to send Email at all
                if (this._connectionService.IsConnected)
                {
                    // Get Archived Logs
                    var archivedLogsUtilityResponse = this._localLogUtility
                        .CreateLogZipFile();

                    // Check if Logs are Archived
                    if (archivedLogsUtilityResponse.Created)
                    {
                        // Send Them To Dev Team Email
                        var emailSendResult = await this._emailSender
                            .SendEmailAsync(
                                Globals.IssueReportTitle,
                                Globals.IssueReportBody,
                                Globals.ReportIssueEmails,
                                archivedLogsUtilityResponse.Message);

                        if (emailSendResult.Success)
                        {
                            // Success
                            responseMessageTitle = AppResources.EmailSuccesTitle;
                            responseMessage = AppResources.IssueEmailSuccessMessage;
                        }
                        else
                        {
                            base._appLogger.LogError(emailSendResult.Message);


                            // Can't send email
                            responseMessageTitle = AppResources.CantSendEmailTitle;
                            responseMessage = AppResources.CantSendIssueEmailMessage;
                        }
                    }
                    else
                    {
                        // Can't create log zip
                        base._appLogger.LogError($"Reason: {archivedLogsUtilityResponse.Message} : No data, or whrong folder structure!");

                        // Success
                        responseMessageTitle = AppResources.IssueNoDataToSendTitle;
                        responseMessage = AppResources.IssueNoDataToSendMessage;
                    }
                }
                else
                {
                    base._appLogger.LogError(AppResources.IssueNoWebConnectionMessageTitle);

                    // User Is not connected to web - can't send issue
                    responseMessageTitle = AppResources.IssueNoWebConnectionMessageTitle;
                    responseMessage = AppResources.IssueNoWebConnectionMessage;
                }

                // Notify User
                await this._dialogService
                    .ShowDialog(
                        responseMessage,
                        responseMessageTitle,
                        AppResources.ButtonOkText);
            }
            catch (System.Exception ex)
            {
                base._appLogger.LogCritical(ex);

                await base.InternalErrorMessageToUser();
            }
        }

        /// <summary>
        /// Send FeedBack Data to Dev team
        /// </summary>
        public IAsyncCommand SendDBCommand => new AsyncCommand(
            SendDB,
            onException: base.GenericCommandExeptionHandler,
            allowsMultipleExecutions: false);

        private async Task SendDB()
        {
            try
            {
                var responseMessageTitle = string.Empty;
                var responseMessage = string.Empty;

                // Check If User is able to send Email at all
                if (this._connectionService.IsConnected)
                {
                    var userId = Globals.UserId;
                    var user = _realm.Find<User>(userId);

                    // Get Archived DB
                    var archivedLogsUtilityResponse = this._localLogUtility
                        .CreateDbZipFile(
                            AppContainer.GetRealmConfiguration.DatabasePath.Replace("default.realm" , ""),
                            DbDump.DumpInMemory(this._realm));

                    // Check if Logs are Archived
                    if (archivedLogsUtilityResponse.Created)
                    {
                        // Send Them To Dev Team Email
                        var emailSendResult = await this._emailSender
                            .SendEmailAsync(
                                Globals.IssueReportTitle,
                                Globals.IssueReportBody,
                                Globals.ReportIssueEmails,
                                archivedLogsUtilityResponse.Message);

                        if (emailSendResult.Success)
                        {
                            // Success
                            responseMessageTitle = AppResources.EmailSuccesTitle;
                            responseMessage = AppResources.DbEmailSuccessMessage;
                        }
                        else
                        {
                            base._appLogger.LogError(emailSendResult.Message);


                            // Can't send email
                            responseMessageTitle = AppResources.CantSendEmailTitle;
                            responseMessage = AppResources.CantSendIssueEmailMessage;
                        }
                    }
                    else
                    {
                        // Can't create log zip
                        base._appLogger.LogError($"Reason: {archivedLogsUtilityResponse.Message} : No data, or whrong folder structure!");

                        // Success
                        responseMessageTitle = AppResources.DbNoDataToSendTitle;
                        responseMessage = AppResources.DbNoDataToSendMessage;
                    }
                }
                else
                {
                    base._appLogger.LogError(AppResources.DbNoWebConnectionMessageTitle);

                    // User Is not connected to web - can't send issue
                    responseMessageTitle = AppResources.DbNoWebConnectionMessageTitle;
                    responseMessage = AppResources.DbNoWebConnectionMessage;
                }

                // Notify User
                await this._dialogService
                    .ShowDialog(
                        responseMessage,
                        responseMessageTitle,
                        AppResources.ButtonOkText);
            }
            catch (System.Exception ex)
            {
                base._appLogger.LogCritical(ex);

                await base.InternalErrorMessageToUser();
            }
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

                    LocalizationResourceManager.Current.CurrentCulture = value == null ? CultureInfo.CurrentCulture : new CultureInfo(value.Value);

                    this._appPreferencesService.LanguageValue = value.Value;

                    OnPropertyChanged();
                }
            }
        }

        #endregion
    }
}
