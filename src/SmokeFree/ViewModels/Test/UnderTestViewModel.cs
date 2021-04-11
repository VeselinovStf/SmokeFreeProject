using Realms;
using SmokeFree.Abstraction.Managers;
using SmokeFree.Abstraction.Services.General;
using SmokeFree.Abstraction.Utility.Logging;
using SmokeFree.Abstraction.Utility.Wrappers;
using SmokeFree.Data.Models;
using SmokeFree.Models.Managers.NotificationManager;
using SmokeFree.Resx;
using SmokeFree.ViewModels.Base;
using System;

namespace SmokeFree.ViewModels.Test
{
    /// <summary>
    /// UnderTestView Model
    /// </summary>
    public class UnderTestViewModel : ViewModelBase
    {
        #region FIELDS

        /// <summary>
        /// Database
        /// </summary>
        private readonly Realm _realm;

        /// <summary>
        /// Device Specific Notification Manager
        /// </summary>
        private readonly INotificationManager _notificationManager;

        #endregion

        #region CTOR

        public UnderTestViewModel(
            Realm realm,
            INavigationService navigationService,
            IDateTimeWrapper dateTimeWrapper,
            IAppLogger appLogger,
            IDialogService dialogService,
            INotificationManager notificationManager) : base(navigationService, dateTimeWrapper, appLogger, dialogService)
        {
            // Set View Title
            ViewTitle = AppResources.UnderTestViewTiitle;

            // Database
            _realm = realm;

            // Device Specific Notification Manager
            _notificationManager = notificationManager;

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



        #endregion
    }
}
