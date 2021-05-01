using Plugin.LocalNotification;
using Realms;
using SmokeFree.Abstraction.Services.General;
using SmokeFree.Abstraction.Utility.Logging;
using SmokeFree.Abstraction.Utility.Wrappers;
using SmokeFree.Data.Models;
using SmokeFree.Models.Views.Test;
using SmokeFree.Resx;
using SmokeFree.ViewModels.AppSettings;
using SmokeFree.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.ObjectModel;

namespace SmokeFree.ViewModels.Test
{
    /// <summary>
    /// Create Test View Model
    /// </summary>
    public class CreateTestViewModel : ViewModelBase
    {
        #region FIELDS

        /// <summary>
        /// Test Time Duration Collection
        /// </summary>
        private ObservableCollection<TestDurationItem> _testTimeDurations;

        /// <summary>
        /// Selected Test Duration Item
        /// </summary>
        private TestDurationItem _selectedTestTimeDurationItem;

        /// <summary>
        /// Initial Goal To Stop Smoking DateTime Value
        /// </summary>
        private DateTime _goalDateTime;

        /// <summary>
        /// Database
        /// </summary>
        private readonly Realm _realm;

        #endregion

        #region CTOR

        public CreateTestViewModel(
           Realm realm,
           INavigationService navigationService,
           IDateTimeWrapper dateTimeWrapper,
           IAppLogger appLogger,
           IDialogService dialogService)
           : base(navigationService, dateTimeWrapper, appLogger, dialogService)
        {
            // Set View Title
            ViewTitle = AppResources.CreateTestViewTiitle;

            // Set Database
            _realm = realm;

            // Collection Of Test Times that user can chose frome
            this._testTimeDurations = new ObservableCollection<TestDurationItem>();

            // Goal Date Completition
            this.GoalDateTime = _dateTime.Now();

            // Init Collection Of Test Durations
            InitiateTestTimeDurations();
        }

        #endregion

        #region INIT

        /// <summary>
        /// Initialize Test Time Durations Collection
        /// </summary>
        private void InitiateTestTimeDurations()
        {
            this.TestTimeDurations.Clear();

            var durations = new List<TestDurationItem>()
            {
                new TestDurationItem()
                {
                     DayValue = 1,
                     DisplayText =  string.Format("1 {0}", AppResources.SingleDay)
                },
                new TestDurationItem()
                {
                     DayValue = 2,
                     DisplayText =  string.Format("2 {0}", AppResources.PluralDay)
                },
                new TestDurationItem()
                {
                     DayValue = 3,
                     DisplayText =  string.Format("3 {0}", AppResources.PluralDay)
                },
                new TestDurationItem()
                {
                     DayValue = 4,
                     DisplayText =  string.Format("4 {0}", AppResources.PluralDay)
                },
                new TestDurationItem()
                {
                     DayValue = 5,
                     DisplayText =  string.Format("5 {0}", AppResources.PluralDay)
                }
            };

            foreach (var duration in durations)
            {
                this.TestTimeDurations.Add(duration);
            }
        }

        #endregion

        #region COMMANDS

        /// <summary>
        /// Async Start Testing Command and Logic
        /// </summary>
        public IAsyncCommand OnStartTestingCommand => new AsyncCommand(
            ExecuteStartTestingCommand,
            onException: base.GenericCommandExeptionHandler,
            allowsMultipleExecutions: false);

        private async Task ExecuteStartTestingCommand()
        {
            try
            {
                // Check if user is shore
                var userNotification = await base._dialogService
                     .ConfirmAsync(
                        string.Format(AppResources.ConfirmStartTestingDialogMessage, this.SelectedTestTimeDurationItem.DisplayText),
                        AppResources.ConfirmStartTestingDialogTitle,
                        AppResources.ButtonOkText,
                        AppResources.ButtonCancelText
                     );

                // Check User Notification Result
                if (userNotification)
                {
                    // Get Values For Creation
                    var testDuration = this.SelectedTestTimeDurationItem.DayValue;
                    var userId = Globals.UserId;
                    var goalTime = this.GoalDateTime;

                    // Goal Time Validation
                    if (goalTime < base._dateTime.Now().AddDays(Globals.MinChallengeDays))
                    {
                        await this._dialogService.ShowDialog(
                            string.Format(AppResources.CreateTestInvalidGoalTimeMessage, Globals.MinChallengeDays),
                            AppResources.CreateTestInvalidGoalTimeTitle,
                            AppResources.ButtonOkText);
                    }
                    else
                    {
                        // Get Current User
                        var user = this._realm.Find<User>(userId);

                        // Only One Active Test per user check
                        // State Managment Error
                        if (user.Tests
                            .FirstOrDefault(e => e.UserId == user.Id && !e.IsDeleted) != null)
                        {
                            base._appLogger
                                .LogError($"Only One Test Per User Can Exist! User : {userId} : already have one!");

                            await base.InternalErrorMessageToUser();
                        }

                        if (user != null)
                        {
                            // Total Days of Test
                            var goalTimeInDays = (goalTime - _dateTime.Now()).Days;

                            // Initial Chalens - Store StartUp data
                            var challenge = new Data.Models.Challenge()
                            {
                                GoalCompletitionTime = goalTime,
                                CreatedOn = _dateTime.Now(),
                                TotalChallengeDays = goalTimeInDays,
                                UserId = user.Id
                            };


                            var testEndDate = _dateTime.Now().AddDays(testDuration);

                            if (Globals.MockRun)
                            {
                                testEndDate = _dateTime.Now().AddSeconds(30);
                            }

                            // User Test
                            var newTest = new Data.Models.Test()
                            {
                                UserId = user.Id,
                                CreatedOn = _dateTime.Now(),
                                TestStartDate = _dateTime.Now(),
                                TestEndDate = testEndDate,
                            };

                            // Write to DB
                            this._realm.Write(() =>
                            {
                                user.Challenges.Add(challenge);
                                user.Tests.Add(newTest);
                                user.UserState = UserStates.UserUnderTesting.ToString();
                                user.TestId = newTest.Id;
                            });

                            if (user.NotificationState)
                            {
                                // Register Notification
                                var testTimerNotification = new NotificationRequest
                                {
                                    NotificationId = Globals.TestingTimeNotificationId,
                                    Title = AppResources.UnderTestViewModelCompleteTestMessage,
                                    Description = AppResources.UnderTestViewModelCompleteTestNotificationMessage,
                                    ReturningData = "Dummy data", // Returning data when tapped on notification.
                                    NotifyTime = testEndDate.AddSeconds(3) // Used for Scheduling local notification, if not specified notification will show immediately.
                                };

                                NotificationCenter.Current.Show(testTimerNotification);
                            }

                            // Navigate to Under Test
                            await base._navigationService.NavigateToAsync<UnderTestViewModel>();
                        }
                        else
                        {
                            base._appLogger.LogError($"Can't Find User in Database: USER ID {userId}");

                            await base.InternalErrorMessageToUser();
                        }
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
            await base._navigationService.NavigateToAsync<AppSettingsViewModel>();
        }

        #endregion

        #region PROPS

        /// <summary>
        /// Test Time Duration for Picker
        /// </summary>
        public ObservableCollection<TestDurationItem> TestTimeDurations
        {
            get { return _testTimeDurations; }
            set
            {
                if (value != null)
                {
                    _testTimeDurations = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Selected Test Time Duration Item
        /// </summary>
        public TestDurationItem SelectedTestTimeDurationItem
        {
            get { return _selectedTestTimeDurationItem; }
            set
            {
                if (value != null)
                {
                    _selectedTestTimeDurationItem = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Initial Goal To Stop Smoking DateTime Value
        /// </summary>
        public DateTime GoalDateTime
        {
            get { return _goalDateTime; }
            set
            {
                _goalDateTime = value;
                OnPropertyChanged();
            }
        }

        #endregion

    }
}
