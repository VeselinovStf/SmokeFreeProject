using Realms;
using SmokeFree.Abstraction.Services.General;
using SmokeFree.Abstraction.Utility.Logging;
using SmokeFree.Abstraction.Utility.Wrappers;
using SmokeFree.Data.Models;
using SmokeFree.Models.Views.Challenge;
using SmokeFree.Resx;
using SmokeFree.Utilities.UserStateHelpers;
using SmokeFree.ViewModels.AppSettings;
using SmokeFree.ViewModels.Base;
using SmokeFree.ViewModels.Test;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.ObjectModel;

namespace SmokeFree.ViewModels.Challenge
{
    /// <summary>
    /// CreateChallengeVie Model
    /// </summary>
    public class CreateChallengeViewModel : ViewModelBase
    {
        #region FIELDS

        /// <summary>
        /// Database
        /// </summary>
        private readonly Realm _realm;

        /// <summary>
        /// Smoke Free Goal Completition DateTime
        /// </summary>
        private DateTime _goalCompletitionTime;

        /// <summary>
        /// Current challenge id, set by Initialize
        /// </summary>
        private string _currentChallengeId;

        /// <summary>
        /// User Smoke Status
        /// </summary>
        private UserSmokeStatusItem _userSmokeStatus;

        #endregion

        #region CTOR

        public CreateChallengeViewModel(
            Realm realm,
            INavigationService navigationService,
            IDateTimeWrapper dateTimeWrapper,
            IAppLogger appLogger,
            IDialogService dialogService) : base(navigationService, dateTimeWrapper, appLogger, dialogService)
        {
            // Database
            _realm = realm;

            // View Title
            ViewTitle = AppResources.CreateChallengeViewTitle;
        }

        #endregion

        #region INIT

        /// <summary>
        /// Initialize View Initial State
        /// </summary>
        public override async Task InitializeAsync(object parameter)
        {
            try
            {
                // Get User From DB
                var userId = Globals.UserId;
                var user = _realm.Find<User>(userId);

                // Validate User
                if (user != null)
                {
                    var challenge = user.Challenges
                        .FirstOrDefault(e => !e.IsDeleted);

                    // Validate User Challenge
                    if (challenge != null)
                    {
                        // Set GoalCompletitionTime
                        var currentGoaSelectedCompletitionTestTime = challenge.GoalCompletitionTime.LocalDateTime;
                        if (currentGoaSelectedCompletitionTestTime < this._dateTime.Now())
                        {
                            // Notify User if GoalCompletitionTime < DateTime.Now
                            await base._dialogService.ShowDialog(
                                AppResources.CreateChallengeViewModelNewGoalDialogMessage,
                                AppResources.CreateChallengeViewModelNewGoalDialogTitle,
                                AppResources.ButtonOkText);   
                        }
                        else
                        {
                            this.GoalCompletitionTime = currentGoaSelectedCompletitionTestTime;
                        }

                        // Set Initial Challenge Id
                        this.CurrentChallengeId = challenge.Id;

                        var userSmokerStatus = StringToEnum.ToUserState<UserSmokeStatuses>(user.UserSmokeStatuses);
                        var userSmokerStatusIcon = Globals.UserSmokeStatusesSet[userSmokerStatus].First().Item1;
                        var userSmokerStatusMessage = Globals.UserSmokeStatusesSet[userSmokerStatus].First().Item2;

                        this.UserSmokeStatus = new UserSmokeStatusItem()
                        {
                            Icon = userSmokerStatusIcon,
                            Title = AppResources.UserSmokeStatusTitle,
                            Message = userSmokerStatusMessage
                        };
                    }
                    else
                    {
                        // User Challenge Not Found!
                        base._appLogger.LogCritical($"Can't find User Challenge:User Id {userId}");

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
        /// Navigate back to Test Result View
        /// </summary>
        public IAsyncCommand OnBackCommand => new AsyncCommand(
            ExecuteNavigateToBack,
            onException: base.GenericCommandExeptionHandler,
            allowsMultipleExecutions: false);

        private async Task ExecuteNavigateToBack()
        {
            await base._navigationService.NavigateToAsync<TestResultViewModel>();
        }

        #endregion

        #region PROPS

        /// <summary>
        /// Smoke Free Goal Completition DateTime
        /// </summary>
        public DateTime GoalCompletitionTime
        {
            get { return _goalCompletitionTime; }
            set
            {
                _goalCompletitionTime = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Challenge ID
        /// </summary>
        public string CurrentChallengeId
        {
            get { return _currentChallengeId; }
            set 
            { 
                _currentChallengeId = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// User Smoke Status
        /// </summary>
        public UserSmokeStatusItem UserSmokeStatus
        {
            get { return _userSmokeStatus; }
            set 
            {
                _userSmokeStatus = value;
                OnPropertyChanged();
            }
        }



        #endregion


    }
}
