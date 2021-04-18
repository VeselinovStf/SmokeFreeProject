using Moq;
using NUnit.Framework;
using Realms;
using SmokeFree;
using SmokeFree.Abstraction.Services.General;
using SmokeFree.Abstraction.Utility.Logging;
using SmokeFree.Abstraction.Utility.Wrappers;
using SmokeFree.Data.Models;
using SmokeFree.ViewModels.AppSettings;
using System;
namespace AppSettingsViewModelTests.UnitTests
{
    /// <summary>
    /// AppSettingsViewModel - NotificationSwitch Tests
    /// </summary>
    public class NotificationSwitch_Should
    {
        /// <summary>
        /// Chenge in View - Changes Db entity NotificationState
        /// </summary>
        [Test]
        public void Change_NotificationState_Of_User_In_Db()
        {
            //Arrange
            var config = new InMemoryConfiguration(Guid.NewGuid().ToString());
            var realm = Realm.GetInstance(config);

            var navigationServiceMock = new Mock<INavigationService>();
            var dateTimeWrapperMock = new Mock<IDateTimeWrapper>();
            var appLoggerServiceMock = new Mock<IAppLogger>();
            var dialogServiceMock = new Mock<IDialogService>();

            var appSettingsViewModel = new AppSettingsViewModel(
                realm,
                navigationServiceMock.Object,
                dateTimeWrapperMock.Object,
                appLoggerServiceMock.Object,
                dialogServiceMock.Object
                );

            var userNotificationState = true;
            var userId = Globals.UserId;

            var user = new User()
            {
                Id = userId,
                NotificationState = userNotificationState
            };

            realm.Write(() =>
            {
                realm.Add(user);
            });

            appSettingsViewModel.InitializeAsync(new object());
            appSettingsViewModel.NotificationSwitch = false;

            var updatedUser = realm.Find<User>(userId);

            //Assert
            Assert.False(updatedUser.NotificationState);
        }
    }
}
