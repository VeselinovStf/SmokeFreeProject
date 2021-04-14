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
using SmokeFree.ViewModels.AppSettings;
using SmokeFree.ViewModels.Test;
using System.Threading;

namespace UnderTestViewModelTests.UnitTests
{
    /// <summary>
    /// UnderTestViewModel - OnSettingsCommand Tests
    /// </summary>
    public class OnSettingsCommand_Should
    {
        /// <summary>
        /// Navigates to App Settings View Model
        /// </summary>
        [Test]
        public void UnderTestViewModel_Navigate_To_AppSettingsViewModel()
        {
            //Arrange
            var config = new InMemoryConfiguration("UnderTestViewModel_Navigate_To_AppSettingsViewModel");
            var realm = Realm.GetInstance(config);

            var navigationServiceMock = new Mock<INavigationService>();
            navigationServiceMock.Setup(e => e.NavigateToAsync<AppSettingsViewModel>());

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

            var user = new User()
            {
                Id = Globals.UserId,
                TestId = "1",
                NotificationState = false
            };

            realm.Write(() =>
            {
                realm.Add(user);
            });

            underTestViewModel.OnSettingsCommand.Execute(new object());

            //Assert
            navigationServiceMock.Verify(e => e.NavigateToAsync<AppSettingsViewModel>(), Times.Exactly(1));
        }


        /// <summary>
        /// Stops Testing Timer
        /// </summary>
        [Test]
        public void Stops_Testing_Time()
        {
            //Arrange
            var config = new InMemoryConfiguration("Stops_Testing_Time");
            var realm = Realm.GetInstance(config);

            var navigationServiceMock = new Mock<INavigationService>();            
            var dateTimeWrapperMock = new Mock<IDateTimeWrapper>();
            var appLoggerServiceMock = new Mock<IAppLogger>();
            var dialogServiceMock = new Mock<IDialogService>();
            var notificationManagerMock = new Mock<INotificationManager>();
            var testCalculationServiceMock = new Mock<ITestCalculationService>();
            var deviceTimerMock = new Mock<IDeviceTimer>();
            deviceTimerMock.Setup(e => e.Stop(It.IsAny<CancellationTokenSource>()));

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

            var user = new User()
            {
                Id = Globals.UserId,
                TestId = "1",
                NotificationState = false
            };

            realm.Write(() =>
            {
                realm.Add(user);
            });

            underTestViewModel.OnSettingsCommand.Execute(new object());

            //Assert
            deviceTimerMock.Verify(e => e.Stop(It.IsAny<CancellationTokenSource>()), Times.Exactly(1));
        }
    }
}
