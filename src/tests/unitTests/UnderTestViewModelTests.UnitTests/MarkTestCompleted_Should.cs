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
            await underTestViewModel.MarkTestCompleted();

            //Assert
            Assert.IsTrue(userTest.IsCompleted);
            Assert.IsTrue(user.UserState.Equals(UserStates.IsTestComplete.ToString()));
            Assert.IsTrue(userTest.ModifiedOn.LocalDateTime.Equals(dateTime));
        }

    }
}
