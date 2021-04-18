using Moq;
using NUnit.Framework;
using Realms;
using SmokeFree;
using SmokeFree.Abstraction.Managers;
using SmokeFree.Abstraction.Services.Data.Test;
using SmokeFree.Abstraction.Services.General;
using SmokeFree.Abstraction.Utility.DeviceUtilities;
using SmokeFree.Abstraction.Utility.Logging;
using SmokeFree.Abstraction.Utility.Wrappers;
using SmokeFree.Data.Models;
using SmokeFree.ViewModels.ErrorAndEmpty;
using SmokeFree.ViewModels.Test;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace UnderTestViewModelTests.UnitTests
{
    /// <summary>
    /// UnderTestViewModel - SendTestCompletitionNotificationAsync Tests
    /// </summary>
    public class SendNotificationAsync_Should
    {
        /// <summary>
        /// Send Notification  when user is set Notifications
        /// </summary>
        [Test]
        public async Task Send_Notification_When_User_Is_Notifiable()
        {
            //Arrange
            var config = new InMemoryConfiguration(Guid.NewGuid().ToString());
            var realm = Realm.GetInstance(config);

            var navigationServiceMock = new Mock<INavigationService>();
            var dateTimeWrapperMock = new Mock<IDateTimeWrapper>();
            var appLoggerServiceMock = new Mock<IAppLogger>();
            var dialogServiceMock = new Mock<IDialogService>();

            var notificationManagerMock = new Mock<INotificationManager>();
            notificationManagerMock.Setup(e => e.SendNotification(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>()));

            var testCalculationServiceMock = new Mock<ITestCalculationService>();
            var deviceTimerMock = new Mock<IDeviceTimer>();

            var underTestViewModel = new UnderTestViewModel(
                realm,
                navigationServiceMock.Object,
                dateTimeWrapperMock.Object,
                appLoggerServiceMock.Object,
                dialogServiceMock.Object,
                notificationManagerMock.Object,
                testCalculationServiceMock.Object,
                deviceTimerMock.Object
                );

            var userId = Globals.UserId;

            var user = new User()
            {
                Id = userId,
                NotificationState = true
            };


            realm.Write(() =>
            {
                realm.Add(user);
            });

            // Act
            await underTestViewModel.SendNotificationAsync("Title", "Message");

            //Assert
            notificationManagerMock.Verify(e => e.SendNotification(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>()), Times.Once);
        }

        /// <summary>
        /// Not sending Notification if user is set Notifications to false
        /// </summary>
        [Test]
        public async Task Not_Sending_Notification_When_User_Is_Desabled_Notifications()
        {
            //Arrange
            var config = new InMemoryConfiguration(Guid.NewGuid().ToString());
            var realm = Realm.GetInstance(config);

            var navigationServiceMock = new Mock<INavigationService>();
            var dateTimeWrapperMock = new Mock<IDateTimeWrapper>();
            var appLoggerServiceMock = new Mock<IAppLogger>();
            var dialogServiceMock = new Mock<IDialogService>();

            var notificationManagerMock = new Mock<INotificationManager>();
            notificationManagerMock.Setup(e => e.SendNotification(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>()));

            var testCalculationServiceMock = new Mock<ITestCalculationService>();
            var deviceTimerMock = new Mock<IDeviceTimer>();

            var underTestViewModel = new UnderTestViewModel(
                realm,
                navigationServiceMock.Object,
                dateTimeWrapperMock.Object,
                appLoggerServiceMock.Object,
                dialogServiceMock.Object,
                notificationManagerMock.Object,
                testCalculationServiceMock.Object,
                deviceTimerMock.Object
                );

            var userId = Globals.UserId;

            var user = new User()
            {
                Id = userId,
                NotificationState = false
            };


            realm.Write(() =>
            {
                realm.Add(user);
            });

            // Act
            await underTestViewModel.SendNotificationAsync("Title", "Message");

            //Assert
            notificationManagerMock.Verify(e => e.SendNotification(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>()), Times.Never);
        }

        /// <summary>
        /// Navigates to SomethingWentWrongViewModel when user is not found
        /// </summary>
        [Test]
        public async Task Navigates_To_SomethingWentWrongViewModel_When_User_Is_Not_Found()
        {
            //Arrange
            var config = new InMemoryConfiguration(Guid.NewGuid().ToString());
            var realm = Realm.GetInstance(config);

            var navigationServiceMock = new Mock<INavigationService>();
            navigationServiceMock.Setup(e => e.NavigateToAsync<SomethingWentWrongViewModel>());

            var dateTimeWrapperMock = new Mock<IDateTimeWrapper>();
            var appLoggerServiceMock = new Mock<IAppLogger>();
            var dialogServiceMock = new Mock<IDialogService>();
            var notificationManagerMock = new Mock<INotificationManager>();
            var testCalculationServiceMock = new Mock<ITestCalculationService>();
            var deviceTimerMock = new Mock<IDeviceTimer>();

            var underTestViewModel = new UnderTestViewModel(
                realm,
                navigationServiceMock.Object,
                dateTimeWrapperMock.Object,
                appLoggerServiceMock.Object,
                dialogServiceMock.Object,
                notificationManagerMock.Object,
                testCalculationServiceMock.Object,
                deviceTimerMock.Object
                );

            // Act
            await underTestViewModel.SendNotificationAsync("Title", "Message");

            //Assert
            navigationServiceMock.Verify(e => e.NavigateToAsync<SomethingWentWrongViewModel>(), Times.Once);
        }
    }
}
