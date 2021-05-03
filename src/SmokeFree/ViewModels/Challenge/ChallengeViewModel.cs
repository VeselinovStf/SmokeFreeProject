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

namespace SmokeFree.ViewModels.Challenge
{
    /// <summary>
    /// ChallengeVie Model
    /// </summary>
    public class ChallengeViewModel : ViewModelBase
    {
        #region FIELDS

        private readonly Realm _realm;


        #endregion

        #region CTOR

        public ChallengeViewModel(
            Realm realm,
            INavigationService navigationService,
            IDateTimeWrapper dateTimeWrapper,
            IAppLogger appLogger,
            IDialogService dialogService) : base(navigationService, dateTimeWrapper, appLogger, dialogService)
        {
            _realm = realm;
        }

        #endregion

        #region INIT


        #endregion

        #region COMMANDS

        /// <summary>
        /// Async Start Challenge Command and Logic
        /// </summary>
        public IAsyncCommand OnStopChallengeCommand => new AsyncCommand(
            ExecuteStopChallengeCommand,
            onException: base.GenericCommandExeptionHandler,
            allowsMultipleExecutions: false);

        private async Task ExecuteStopChallengeCommand()
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

        #endregion

        #region PROPS


        #endregion
    }
}
