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
    /// UnderTestViewModel - CreateTestResultAsync Tests
    /// </summary>
    public class CreateTestResultAsync_Should
    {

        /// <summary>
        /// Notify User ( navigate to SomethingWentWrongViewModel ) when test results cant be calculated
        /// </summary>
        [Test]
        public void Notify_User_When_TestResult_Cant_Be_Calculated()
        {
            //Arrange
            var config = new InMemoryConfiguration(Guid.NewGuid().ToString());
            var realm = Realm.GetInstance(config);

            var dateTime = DateTime.Now;
            var navigationServiceMock = new Mock<INavigationService>();
            navigationServiceMock.Setup(e => e.NavigateToAsync<SomethingWentWrongViewModel>());

            var dateTimeWrapperMock = new Mock<IDateTimeWrapper>();
            dateTimeWrapperMock.Setup(e => e.Now()).Returns(dateTime);

            var appLoggerServiceMock = new Mock<IAppLogger>();
            var dialogServiceMock = new Mock<IDialogService>();

            var testCalculationServiceMock = new Mock<ITestCalculationService>();
            testCalculationServiceMock.Setup(e => e.CalculateTestResult(It.IsAny<Test>()))
                .Returns(() => new SmokeFree.Models.Services.Data.Test.CalculateTestResultDTO(false, "FAILSE"));

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
            underTestViewModel.CreateTestResultAsync();

            //Assert
            navigationServiceMock.Verify(e => e.NavigateToAsync<SomethingWentWrongViewModel>(), Times.Exactly(1));
        }

        /// <summary>
        /// Nothing save in db
        /// </summary>
        [Test]
        public void Nothing_Is_Persist_In_DB_When_TestResult_Calculations_Cant_Be_Performed()
        {
            //Arrange
            var config = new InMemoryConfiguration(Guid.NewGuid().ToString());
            var realm = Realm.GetInstance(config);

            var dateTime = DateTime.Now;
            var testResultId = "TEST_RESULT";
            var navigationServiceMock = new Mock<INavigationService>();

            var dateTimeWrapperMock = new Mock<IDateTimeWrapper>();
            dateTimeWrapperMock.Setup(e => e.Now()).Returns(dateTime);

            var appLoggerServiceMock = new Mock<IAppLogger>();
            var dialogServiceMock = new Mock<IDialogService>();

            var testCalculationServiceMock = new Mock<ITestCalculationService>();
            testCalculationServiceMock.Setup(e => e.CalculateTestResult(It.IsAny<Test>()))
                .Returns(() => new SmokeFree.Models.Services.Data.Test.CalculateTestResultDTO(false, "Failse"));

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
            underTestViewModel.CreateTestResultAsync();

            var dbTestResult = realm.Find<TestResult>(testResultId);
            //Assert
            Assert.IsNull(dbTestResult);
        }

        /// <summary>
        /// App State - Notify User ( navigate to SomethingWentWrongViewModel ) when test is not found
        /// </summary>
        [Test]
        public void Notify_User_When_Test_Is_Not_Found()
        {
            //Arrange
            var config = new InMemoryConfiguration(Guid.NewGuid().ToString());
            var realm = Realm.GetInstance(config);

            var dateTime = DateTime.Now;
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
            underTestViewModel.CreateTestResultAsync();

            //Assert
            navigationServiceMock.Verify(e => e.NavigateToAsync<SomethingWentWrongViewModel>(), Times.Exactly(1));
        }

        /// <summary>
        /// App State - Notify User ( navigate to SomethingWentWrongViewModel ) when user is not found
        /// </summary>
        [Test]
        public void Notify_User_When_User_Is_Not_Found()
        {
            //Arrange
            var config = new InMemoryConfiguration(Guid.NewGuid().ToString());
            var realm = Realm.GetInstance(config);

            var dateTime = DateTime.Now;
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
            underTestViewModel.CreateTestResultAsync();

            //Assert
            navigationServiceMock.Verify(e => e.NavigateToAsync<SomethingWentWrongViewModel>(), Times.Exactly(1));
        }
    }
}
