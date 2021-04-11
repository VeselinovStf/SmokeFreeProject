using Realms;
using SmokeFree.Abstraction.Managers;
using SmokeFree.Abstraction.Services.Data.Test;
using SmokeFree.Abstraction.Services.General;
using SmokeFree.Abstraction.Utility.Logging;
using SmokeFree.Abstraction.Utility.Wrappers;
using SmokeFree.Data.Models;
using SmokeFree.Models.Managers.NotificationManager;
using SmokeFree.Resx;
using SmokeFree.ViewModels.Base;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SmokeFree.ViewModels.Test
{
    /// <summary>
    /// UnderTestView Model
    /// </summary>
    public class UnderTestViewModel : ViewModelBase
    {
        #region FIELDS

        /// <summary>
        /// Curent Smoked Count
        /// </summary>
        private int _currentlySmokedCount;

        /// <summary>
        /// Timer From last Smoked
        /// </summary>
        private TimeSpan _timeSenceLastSmoke;

        /// <summary>
        /// Test Left Time Indicator
        /// </summary>
        private TimeSpan _testLeftTime;

        /// <summary>
        /// Smoking State
        /// </summary>
        private bool _isSmoking;

        /// <summary>
        /// Current Smoke Id
        /// </summary>
        private string _currentSmokeId;

        /// <summary>
        /// Current Smoke Time Display
        /// </summary>
        private TimeSpan _currentSmokeTime;

        /// <summary>
        /// Database
        /// </summary>
        private readonly Realm _realm;

        /// <summary>
        /// Device Specific Notification Manager
        /// </summary>
        private readonly INotificationManager _notificationManager;

        /// <summary>
        /// Test Calculations Service Abstraction
        /// </summary>
        private readonly ITestCalculationService _testCalculationService;

        #endregion

        #region CTOR

        public UnderTestViewModel(
            Realm realm,
            INavigationService navigationService,
            IDateTimeWrapper dateTimeWrapper,
            IAppLogger appLogger,
            IDialogService dialogService,
            INotificationManager notificationManager,
            ITestCalculationService testCalculationService) : base(navigationService, dateTimeWrapper, appLogger, dialogService)
        {
            // Set View Title
            ViewTitle = AppResources.UnderTestViewTiitle;

            // Database
            _realm = realm;

            // Device Specific Notification Manager
            _notificationManager = notificationManager;

            // Test Calculation Service
            _testCalculationService = testCalculationService;

            // Notification for test completition
            InitiateTestCompletitionNotificationEvent();
        }

        #endregion

        #region INIT

        /// <summary>
        /// Initate Notification For Test COmpletition
        /// </summary>
        private void InitiateTestCompletitionNotificationEvent()
        {         
            try
            {
                // Get Current User
                var userId = Globals.UserId;
                var user = _realm.Find<User>(userId);

                // Check if is valid
                if (user != null)
                {
                    var userNotificationPremission = user.NotificationState;

                    // Send Notification if user is allowed notifications
                    if (userNotificationPremission)
                    {
                        this._notificationManager.NotificationReceived += (sender, eventArgs) =>
                        {
                            var evtData = (NotificationEventArgs)eventArgs;
                            ShowNotification(evtData.Title, evtData.Message);
                        };
                    }
                }
                else
                {
                    // User Not Found!
                    base._appLogger.LogCritical($"Can't find User: {userId}");

                    // TODO: A: Navigate to Error View Model
                    // Set Option for 'go back'
                    base.InternalErrorMessageToUser();
                }
            }
            catch (Exception ex)
            {
                base._appLogger.LogError($"Can't Initialize Under Test Completition Notification: {ex.Message}");

                // TODO: A: Navigate to Error View Model
                // Set Option for 'go back'
                base.InternalErrorMessageToUser();
            }
            
        }

        /// <summary>
        /// Initialize View State
        /// </summary>
        /// <param name="parameter">Optional</param>
        /// <returns>Base Initialize Async</returns>
        public override Task InitializeAsync(object parameter)
        {
            try
            {
                // Get User From DB
                var userId = Globals.UserId;
                var user = this._realm.Find<User>(userId);

                // Validate User
                if (user != null)
                {
                    // Get Current User Test
                    var currentUserTestId = user.TestId;
                    var currentTest = user.Tests
                        .FirstOrDefault(e => e.Id == currentUserTestId);

                    // Validate Test
                    if (currentTest != null)
                    {
                        // Calculate Test 
                        var testCalculation = this._testCalculationService
                            .GetCurrentTestDataCalculation(_dateTime.Now(), currentTest);

                        // Assignm properties
                        this.CurrentlySmokedCount = testCalculation.CurrentSmokedCount;
                        this.TimeSenceLastSmoke = testCalculation.TimeSinceLastSmoke;
                        this.TestLeftTime = testCalculation.TestTimeLeft;

                        // Set value of currently smoked or string.Empty 
                        // if user is smoking for first time
                        this.CurrentSmokeId = testCalculation.CurrentSmokeId;
                        this.CurrentSmokeTime = testCalculation.CurrentSmokeTime;

                        // Check if is smoking
                        if (testCalculation.CurrentSmokeTime > TimeSpan.FromSeconds(1))
                        {
                            this.IsSmoking = true;
                        }

                        // Start Device Cound Down for Test Left Time
                        //StartTestintTimer();
                    }
                    else
                    {
                        // User Not Found!
                        base._appLogger.LogCritical($"Can't find User Test: {currentUserTestId}");

                        // TODO: A: Navigate to Error View Model
                        // Set Option for 'go back'
                        base.InternalErrorMessageToUser();
                    }
                }
                else
                {
                    // User Not Found!
                    base._appLogger.LogCritical($"Can't find User: {userId}");

                    // TODO: A: Navigate to Error View Model
                    // Set Option for 'go back'
                    base.InternalErrorMessageToUser();
                }
            }
            catch (Exception ex)
            {
                base._appLogger.LogError(ex.Message);

                // TODO: A: Navigate to Error View Model
                // Set Option for 'go back'
                base.InternalErrorMessageToUser();
            }

            return base.InitializeAsync(parameter);
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Empty Method, Used By Platform Specific aps
        /// </summary>
        /// <param name="title">Notification Title</param>
        /// <param name="message">Notification Message</param>
        private void ShowNotification(string title, string message) { }

        #endregion

        #region PROPS

        /// <summary>
        /// Curent Smoked Count
        /// </summary>
        public int CurrentlySmokedCount
        {
            get { return _currentlySmokedCount; }
            set
            {
                _currentlySmokedCount = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Current Smoke Id
        /// </summary>
        public string CurrentSmokeId
        {
            get { return _currentSmokeId; }
            set
            {
                _currentSmokeId = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Current Smoke Time Display
        /// </summary>
        public TimeSpan CurrentSmokeTime
        {
            get { return _currentSmokeTime; }
            set
            {
                _currentSmokeTime = value;
                OnPropertyChanged();
            }
        }


        /// <summary>
        /// Timer From last Smoked
        /// </summary>
        public TimeSpan TimeSenceLastSmoke
        {
            get { return _timeSenceLastSmoke; }
            set
            {
                _timeSenceLastSmoke = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Test Left Time Indicator
        /// </summary>
        public TimeSpan TestLeftTime
        {
            get { return _testLeftTime; }
            set
            {
                _testLeftTime = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Smoking State
        /// </summary>
        public bool IsSmoking
        {
            get { return _isSmoking; }
            set
            {

                _isSmoking = value;
                OnPropertyChanged();
            }
        }

        #endregion
    }
}
