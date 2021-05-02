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
using SmokeFree.Models.Services.Data.Test;
using SmokeFree.ViewModels.Test;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace UnderTestViewModelTests.UnitTests
{
    /// <summary>
    /// UnderTestViewModel InitializeAsync Tests
    /// </summary>
    public class InitializeAsync_Should
    {
        /// <summary>
        /// Initilize Async Should Set All Properties
        /// </summary>
        [Test]
        public async Task Initialize_Initial_State_Of_UnderTestViewModel_With_Values()
        {
            //Arrange
            var config = new InMemoryConfiguration(Guid.NewGuid().ToString());
            var realm = Realm.GetInstance(config);

            var navigationServiceMock = new Mock<INavigationService>();
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

            object parameter = new object();
            var currentlySmokedCount = 2;
            var timeSenceLastSmoke = new TimeSpan(1, 1, 1);
            var testLeftTime = new TimeSpan(1, 1, 1);
            var currentSmokeId = "ID";
            var currentSmokeTime = new TimeSpan(1, 1, 1);
            var testId = "TEST_ID";

            var user = new User()
            {
                Id = Globals.UserId,
                TestId = testId
            };

            var userTest = new Test()
            {
                Id = testId,
                UserId = Globals.UserId
            };

            realm.Write(() =>
            {
                realm.Add(user);

                user.Tests.Add(userTest);
            });

            var testCalculationResultDTO = new CurrentTestDataCalculationDTO(
                currentlySmokedCount,
                timeSenceLastSmoke,
                testLeftTime,
                currentSmokeId,
                currentSmokeTime, true);

            testCalculationServiceMock.Setup(e => e.GetCurrentTestDataCalculation(It.IsAny<DateTime>(), It.IsAny<Test>()))
                .Returns(testCalculationResultDTO);

            // Act
            await underTestViewModel.AppearingInitializeAsync();

            //Assert
            //Assert.AreEqual(currentlySmokedCount, underTestViewModel.CurrentlySmokedCount, "CurrentlySmokedCount not equal");
            Assert.AreEqual(currentSmokeId, underTestViewModel.CurrentSmokeId, "CurrentSmokeId not equal");
            //Assert.IsTrue(underTestViewModel.TimeSenceLastSmoke.Equals(timeSenceLastSmoke), "TimeSenceLastSmoke not equal");
            //Assert.IsTrue(underTestViewModel.TestLeftTime.Equals(testLeftTime), "TestLeftTime not equal");
            //Assert.IsTrue(underTestViewModel.CurrentSmokeTime.Equals(currentSmokeTime), "CurrentSmokeTime not equal");

        }


        [Test]
        public async Task Logs_Critical_When_User_Is_Not_Found_In_DB()
        {
            //Arrange
            var config = new InMemoryConfiguration(Guid.NewGuid().ToString());
            var realm = Realm.GetInstance(config);

            var navigationServiceMock = new Mock<INavigationService>();
            var dateTimeWrapperMock = new Mock<IDateTimeWrapper>();
            var appLoggerServiceMock = new Mock<IAppLogger>();
            appLoggerServiceMock.Setup(e => e.LogCritical(It.IsAny<string>(), It.IsAny<string>()));

            var dialogServiceMock = new Mock<IDialogService>();

            var testCalculationServiceMock = new Mock<ITestCalculationService>();

            var deviceTimerMock = new Mock<IDeviceTimer>();
            deviceTimerMock.Setup(e => e.Start(It.IsAny<Func<Task<bool>>>(), It.IsAny<CancellationTokenSource>()));

            var underTestViewModel = new UnderTestViewModel(
                realm,
                navigationServiceMock.Object,
                dateTimeWrapperMock.Object,
                appLoggerServiceMock.Object,
                dialogServiceMock.Object,
                testCalculationServiceMock.Object,
                deviceTimerMock.Object
                );

            object parameter = new object();
            var currentlySmokedCount = 2;
            var timeSenceLastSmoke = new TimeSpan(1, 1, 1);
            var testLeftTime = new TimeSpan(1, 1, 1);
            var currentSmokeId = "ID";
            var currentSmokeTime = new TimeSpan(1, 1, 1);

            var testCalculationResultDTO = new CurrentTestDataCalculationDTO(
                currentlySmokedCount,
                timeSenceLastSmoke,
                testLeftTime,
                currentSmokeId,
                currentSmokeTime, true);

            // Act
            await underTestViewModel.AppearingInitializeAsync();

            //Assert
            appLoggerServiceMock.Verify(e => e.LogCritical(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(1));

        }

        [Test]
        public async Task Call_LogCriticall_When_Current_Test_Is_Null()
        {
            //Arrange
            var config = new InMemoryConfiguration(Guid.NewGuid().ToString());
            var realm = Realm.GetInstance(config);

            var navigationServiceMock = new Mock<INavigationService>();
            var dateTimeWrapperMock = new Mock<IDateTimeWrapper>();
            var appLoggerServiceMock = new Mock<IAppLogger>();
            appLoggerServiceMock.Setup(e => e.LogCritical(It.IsAny<string>(), It.IsAny<string>()));

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

            object parameter = new object();
            var currentlySmokedCount = 2;
            var timeSenceLastSmoke = new TimeSpan(1, 1, 1);
            var testLeftTime = new TimeSpan(1, 1, 1);
            var currentSmokeId = "ID";
            var currentSmokeTime = new TimeSpan(1, 1, 1);
            var testId = "TEST_ID";

            var user = new User()
            {
                Id = Globals.UserId,
                TestId = testId
            };

            realm.Write(() =>
            {
                realm.Add(user);
            });

            var testCalculationResultDTO = new CurrentTestDataCalculationDTO(
                currentlySmokedCount,
                timeSenceLastSmoke,
                testLeftTime,
                currentSmokeId,
                currentSmokeTime, true);

            testCalculationServiceMock.Setup(e => e.GetCurrentTestDataCalculation(It.IsAny<DateTime>(), It.IsAny<Test>()))
                .Returns(testCalculationResultDTO);

            // Act
            await underTestViewModel.AppearingInitializeAsync();

            //Assert
            appLoggerServiceMock.Verify(e => e.LogCritical(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(1));
        }




    }
}
