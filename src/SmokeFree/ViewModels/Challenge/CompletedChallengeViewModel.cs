using Realms;
using SmokeFree.Abstraction.Services.General;
using SmokeFree.Abstraction.Utility.DeviceUtilities;
using SmokeFree.Abstraction.Utility.Logging;
using SmokeFree.Abstraction.Utility.Wrappers;
using SmokeFree.Bootstrap;
using SmokeFree.Data.Models;
using SmokeFree.Resx;
using SmokeFree.Utilities.Logging;
using SmokeFree.ViewModels.AppSettings;
using SmokeFree.ViewModels.Base;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.ObjectModel;

namespace SmokeFree.ViewModels.Challenge
{
    public class CompletedChallengeViewModel : ViewModelBase
    {
        #region FIELDS

        private ChallengeResult _challengeResult;

        private readonly Realm _realm;

        private readonly INetworkConnectionService _connectionService;

        private readonly ILocalLogUtility _localLogUtility;

        private readonly IDeviceEmailSender _emailSender;

        #endregion

        #region CTOR

        public CompletedChallengeViewModel(
            Realm realm,
            INavigationService navigationService,
            IDateTimeWrapper dateTimeWrapper,
            IAppLogger appLogger, 
            IDialogService dialogService,
            INetworkConnectionService connectionService,
            ILocalLogUtility localLogUtility,
            IDeviceEmailSender deviceEmailSender) : base(navigationService, dateTimeWrapper, appLogger, dialogService)
        {
            _realm = realm;

            _connectionService = connectionService;

            _localLogUtility = localLogUtility;

            _emailSender = deviceEmailSender;
        }

        #endregion

        #region INIT

        public override async Task InitializeAsync(object parameter)
        {
            try
            {
                var userId = Globals.UserId;
                var user = _realm.Find<User>(userId);

                if (user != null)
                {
                    var challengeResult = _realm.All<ChallengeResult>()
                        .Where(e => !e.IsDeleted && e.Id.Equals(user.ChallengeResultId))
                        .FirstOrDefault();

                    if (challengeResult != null)
                    {
                        this.ChallengeResult = challengeResult;
                    }
                    else
                    {
                        // User Not Found!
                        base._appLogger.LogCritical($"Can't find User Challenge Result:User Id {userId}, Challenge Result Id: {user.ChallengeResultId}");

                        await base.InternalErrorMessageToUser();
                    }
                }
                else
                {
                    // User Not Found!
                    base._appLogger.LogCritical($"Can't find User:User Id {userId}");

                    await base.InternalErrorMessageToUser();
                }
            }
            catch (Exception ex)
            {
                base._appLogger.LogCritical(ex);

                await base.InternalErrorMessageToUser();
            }
        }

        #endregion

        #region COMMANDS

        /// <summary>
        /// Async Start Challenge Command and Logic
        /// </summary>
        public IAsyncCommand OnRestartToTestCommand => new AsyncCommand(
            RestartApp,
            onException: base.GenericCommandExeptionHandler,
            allowsMultipleExecutions: false);

        private async Task RestartApp()
        {
            try
            {
                // Check if user is shure
                var userNotification = await base._dialogService
                     .ConfirmAsync(
                     AppResources.CreateChallengeViewModelBackToTestingMessage,
                     AppResources.CreateChallengeViewModelBackToTestingTitle,
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
                        var userTest = user.Tests.FirstOrDefault(t => t.UserId == userId && !t.IsDeleted);
                        var testChallenge = user.Challenges.FirstOrDefault(c => c.UserId == userId && !c.IsDeleted);
                        // 

                        _realm.Write(() =>
                        {
                            // Remove Test
                            userTest.IsDeleted = true;
                            userTest.DeletedOn = this._dateTime.Now();
                            userTest.ModifiedOn = this._dateTime.Now();

                            userTest.CompletedTestResult.IsDeleted = true;
                            userTest.CompletedTestResult.DeletedOn = this._dateTime.Now();
                            userTest.CompletedTestResult.ModifiedOn = this._dateTime.Now();

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

                            // Remove Challenge
                            testChallenge.IsDeleted = true;
                            testChallenge.DeletedOn = this._dateTime.Now();
                            testChallenge.ModifiedOn = this._dateTime.Now();

                            foreach (var cs in testChallenge.ChallengeSmokes)
                            {
                                cs.IsDeleted = true;
                                cs.DeletedOn = this._dateTime.Now();
                                cs.ModifiedOn = this._dateTime.Now();
                            }

                            // TODO: Found Bug
                            testChallenge.CompletedChallengeResult.IsDeleted = true;
                            testChallenge.CompletedChallengeResult.ModifiedOn = _dateTime.Now();
                            testChallenge.CompletedChallengeResult.DeletedOn = _dateTime.Now();
                            user.ChallengeResultId = string.Empty;

                            // Update User Status
                            user.UserState = UserStates.CompletedOnBoarding.ToString();
                            user.TestId = string.Empty;
                        });

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
        /// Navigate to settings View
        /// </summary>
        public IAsyncCommand OnSettingsCommand => new AsyncCommand(
            ExecuteNavigateToSetting,
            onException: base.GenericCommandExeptionHandler,
            allowsMultipleExecutions: false);

        private async Task ExecuteNavigateToSetting()
        {
            try
            {
                await base._navigationService.NavigateToAsync<AppSettingsViewModel>();
            }
            catch (Exception ex)
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
                            AppContainer.GetRealmConfiguration.DatabasePath.Replace("default.realm", ""),
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

        #region PROPS


        public ChallengeResult ChallengeResult
        {
            get { return _challengeResult; }
            set 
            { 
                _challengeResult = value;
                OnPropertyChanged();
            }
        }


        #endregion
    }
}
