using Moq;
using NUnit.Framework;
using Realms;
using SmokeFree;
using SmokeFree.Abstraction.Services.Data.Test;
using SmokeFree.Abstraction.Services.General;
using SmokeFree.Abstraction.Utility.DeviceUtilities;
using SmokeFree.Abstraction.Utility.Logging;
using SmokeFree.Abstraction.Utility.Wrappers;
using SmokeFree.Data.Models;
using SmokeFree.ViewModels.ErrorAndEmpty;
using SmokeFree.ViewModels.Test;
using System;
using System.Threading.Tasks;

namespace UnderTestViewModelTests.UnitTests
{
    /// <summary>
    /// UnderTestViewModel - MarkTestCompleted Tests
    /// </summary>
    public class MarkTestCompleted_Should
    {
        /// <summary>
        /// Mark Test Completed When State is appropriate
        /// </summary>
        [Test]
        public async Task Mark_Test_Completed()
        {
            //Arrange
            var config = new InMemoryConfiguration(Guid.NewGuid().ToString());
            var realm = Realm.GetInstance(config);

            var dateTime = DateTime.Now;
            var navigationServiceMock = new Mock<INavigationService>();

            var dateTimeWrapperMock = new Mock<IDateTimeWrapper>();
            dateTimeWrapperMock.Setup(e => e.Now()).Returns(dateTime);

            var appLoggerServiceMock = new Mock<IAppLogger>();
            var dialogServiceMock = new Mock<IDialogService>();

            var testCalculationServiceMock = new Mock<ITestCalculationService>();
            var deviceTimerMock = new Mock<IDeviceTimer>();

            var underTestViewModel = new UnderTestViewModel(
                realm,
                navigationServiceMock.Object,
                dateTimeWrapperMock.Object,
                appLoggerServiceMock.Object,
                dialogServiceMock.Object,
                testCalculationServiceMock.Object,
                deviceTimerMock.Object
                );

            var userId = Globals.UserId;
            var testId = "TEST_ID";

            var user = new User()
            {
                Id = userId,
                TestId = testId
            };

            var userTest = new Test()
            {
                Id = testId,
                UserId = userId
            };

            realm.Write(() =>
            {
                realm.Add(user);

                user.Tests.Add(userTest);
            });

            // Act
            underTestViewModel.MarkTestCompletedAsync();

            //Assert
            Assert.IsTrue(userTest.IsCompleted);
            Assert.IsTrue(user.UserState.Equals(UserStates.IsTestComplete.ToString()));
            Assert.IsTrue(userTest.ModifiedOn.LocalDateTime.Equals(dateTime));
        }

        /// <summary>
        /// Show User Completition Dialog
        /// </summary>
        [Test]
        public async Task Show_User_Dialog_When_Test_Is_Completed()
        {
            //Arrange
            var config = new InMemoryConfiguration(Guid.NewGuid().ToString());
            var realm = Realm.GetInstance(config);

            var dateTime = DateTime.Now;
            var navigationServiceMock = new Mock<INavigationService>();

            var dateTimeWrapperMock = new Mock<IDateTimeWrapper>();
            dateTimeWrapperMock.Setup(e => e.Now()).Returns(dateTime);

            var appLoggerServiceMock = new Mock<IAppLogger>();

            var dialogServiceMock = new Mock<IDialogService>();
            dialogServiceMock.Setup(e => e.ShowDialog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));


            var testCalculationServiceMock = new Mock<ITestCalculationService>();
            var deviceTimerMock = new Mock<IDeviceTimer>();

            var underTestViewModel = new UnderTestViewModel(
                realm,
                navigationServiceMock.Object,
                dateTimeWrapperMock.Object,
                appLoggerServiceMock.Object,
                dialogServiceMock.Object,

                testCalculationServiceMock.Object,
                deviceTimerMock.Object
                );

            var userId = Globals.UserId;
            var testId = "TEST_ID";

            var user = new User()
            {
                Id = userId,
                TestId = testId
            };

            var userTest = new Test()
            {
                Id = testId,
                UserId = userId
            };

            realm.Write(() =>
            {
                realm.Add(user);

                user.Tests.Add(userTest);
            });

            // Act
            underTestViewModel.MarkTestCompletedAsync();

            //Assert
            dialogServiceMock.Verify(e => e.ShowDialog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(1));
        }

        /// <summary>
        /// Navigate to SomethingWentWrongViewModel Whrong view model when user is not found
        /// </summary>
        [Test]
        public async Task Navigate_To_SomethingWentWrongViewModel_When_Db_Throws()
        {
            //Arrange           
            var config = new InMemoryConfiguration(Guid.NewGuid().ToString());
            var realm = Realm.GetInstance(config);

            var navigationServiceMock = new Mock<INavigationService>();
            navigationServiceMock.Setup(e => e.NavigateToAsync<SomethingWentWrongViewModel>());

            var dateTimeWrapperMock = new Mock<IDateTimeWrapper>();
            var appLoggerServiceMock = new Mock<IAppLogger>();
            var dialogServiceMock = new Mock<IDialogService>();

            var testCalculationServiceMock = new Mock<ITestCalculationService>();
            var deviceTimerMock = new Mock<IDeviceTimer>();

            var underTestViewModel = new UnderTestViewModel(
                realm,
                navigationServiceMock.Object,
                dateTimeWrapperMock.Object,
                appLoggerServiceMock.Object,
                dialogServiceMock.Object,

                testCalculationServiceMock.Object,
                deviceTimerMock.Object
                );

            // Act
            underTestViewModel.MarkTestCompletedAsync();

            //Assert
            navigationServiceMock.Verify(e => e.NavigateToAsync<SomethingWentWrongViewModel>(), Times.Exactly(1));
        }

        /// <summary>
        /// Navigate to SomethingWentWrongViewModel if user test is not found
        /// </summary>
        [Test]
        public async Task Navigate_To_SomethingWentWrongViewModel_When_User_Test_Is_Not_Found()
        {
            //Arrange
            var config = new InMemoryConfiguration(Guid.NewGuid().ToString());
            var realm = Realm.GetInstance(config);

            var navigationServiceMock = new Mock<INavigationService>();
            navigationServiceMock.Setup(e => e.NavigateToAsync<SomethingWentWrongViewModel>());

            var dateTimeWrapperMock = new Mock<IDateTimeWrapper>();
            var appLoggerServiceMock = new Mock<IAppLogger>();
            var dialogServiceMock = new Mock<IDialogService>();

            var testCalculationServiceMock = new Mock<ITestCalculationService>();
            var deviceTimerMock = new Mock<IDeviceTimer>();

            var underTestViewModel = new UnderTestViewModel(
                realm,
                navigationServiceMock.Object,
                dateTimeWrapperMock.Object,
                appLoggerServiceMock.Object,
                dialogServiceMock.Object,

                testCalculationServiceMock.Object,
                deviceTimerMock.Object
                );

            var userId = Globals.UserId;

            var user = new User()
            {
                Id = userId,
            };

            realm.Write(() =>
            {
                realm.Add(user);
            });

            // Act
            underTestViewModel.MarkTestCompletedAsync();

            //Assert
            navigationServiceMock.Verify(e => e.NavigateToAsync<SomethingWentWrongViewModel>(), Times.Exactly(1));

        }

        /// <summary>
        /// Log Critical if User Test Is already completed
        /// </summary>
        [Test]
        public async Task Log_Critical_If_User_Test_Is_Completed()
        {
            //Arrange
            var config = new InMemoryConfiguration(Guid.NewGuid().ToString());
            var realm = Realm.GetInstance(config);

            var dateTime = DateTime.Now;
            var navigationServiceMock = new Mock<INavigationService>();
            var dateTimeWrapperMock = new Mock<IDateTimeWrapper>();

            var appLoggerServiceMock = new Mock<IAppLogger>();
            appLoggerServiceMock.
                Setup(e => e.LogCritical(It.IsAny<string>(), It.IsAny<string>()));

            var dialogServiceMock = new Mock<IDialogService>();

            var testCalculationServiceMock = new Mock<ITestCalculationService>();
            var deviceTimerMock = new Mock<IDeviceTimer>();

            var underTestViewModel = new UnderTestViewModel(
                realm,
                navigationServiceMock.Object,
                dateTimeWrapperMock.Object,
                appLoggerServiceMock.Object,
                dialogServiceMock.Object,

                testCalculationServiceMock.Object,
                deviceTimerMock.Object
                );

            var userId = Globals.UserId;
            var testId = "TEST_ID";

            var user = new User()
            {
                Id = userId,
                TestId = testId
            };

            var userTest = new Test()
            {
                Id = testId,
                UserId = userId,
                IsCompleted = true
            };

            realm.Write(() =>
            {
                realm.Add(user);

                user.Tests.Add(userTest);
            });

            // Act
            underTestViewModel.MarkTestCompletedAsync();

            //Assert
            appLoggerServiceMock.Verify(e => e.LogCritical(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(1));
        }
    }
}
