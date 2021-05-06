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
    /// UnderTestViewModel - SmokeOneCigareAsync Tests
    /// </summary>
    public class SmokeOneCigareAsync_Should
    {
        /// <summary>
        /// Navigate to SomethingWentWrongViewModel Whrong view model when user is not found
        /// </summary>
        [Test]
        public async Task Navigate_To_SomethingWentWrongViewModel_When_User_Is_Not_Found()
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
            await underTestViewModel.SmokeOneCigareAsync();

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
            await underTestViewModel.SmokeOneCigareAsync();

            //Assert
            navigationServiceMock.Verify(e => e.NavigateToAsync<SomethingWentWrongViewModel>(), Times.Exactly(1));

        }

        /// <summary>
        /// Navigate to SomethingWentWrongViewModel if user test is not found
        /// </summary>
        [Test]
        public async Task Navigate_To_SomethingWentWrongViewModel_When_User_Test_Smoke_Is_Not_Found()
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
            var testId = "Test_ID";

            var user = new User()
            {
                Id = userId,
                TestId = testId
            };

            var test = new Test()
            {
                Id = testId,
                UserId = userId
            };

            realm.Write(() =>
            {
                realm.Add(user);

                user.Tests.Add(test);
            });

            // Act
            await underTestViewModel.SmokeOneCigareAsync();

            //Assert
            navigationServiceMock.Verify(e => e.NavigateToAsync<SomethingWentWrongViewModel>(), Times.Exactly(1));

        }

        /// <summary>
        /// App State - Current smoke must be completed only once
        /// </summary>
        [Test]
        public async Task Log_Critical_If_Current_Smoke_Is_Ended_Previously()
        {
            //Arrange
            var config = new InMemoryConfiguration(Guid.NewGuid().ToString());
            var realm = Realm.GetInstance(config);

            var dateTime = DateTime.Now;
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

            var userId = Globals.UserId;
            var testId = "Test_ID";
            var smokeId = "SMOKE_ID";

            var user = new User()
            {
                Id = userId,
                TestId = testId
            };

            var test = new Test()
            {
                Id = testId,
                UserId = userId
            };

            var smoke = new Smoke()
            {
                Id = smokeId,
                StartSmokeTime = dateTime,
                TestId = testId,
                EndSmokeTime = dateTime
            };

            test.SmokedCigaresUnderTest.Add(smoke);

            realm.Write(() =>
            {
                realm.Add(user);

                user.Tests.Add(test);
            });

            underTestViewModel.CurrentSmokeId = smokeId;

            // Act
            await underTestViewModel.SmokeOneCigareAsync();

            //Assert
            appLoggerServiceMock.Verify(e => e.LogCritical(It.IsAny<string>(), It.IsAny<string>()), Times.Once);

        }

        /// <summary>
        /// Updates Current Smoke to completed
        /// </summary>
        [Test]
        public async Task Set_Current_Smoke_Completed_In_DB()
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
            testCalculationServiceMock.Setup(e => e.TimeSinceLastSmoke(It.IsAny<Test>(), It.IsAny<DateTime>()))
                .Returns(new TimeSpan(0, 0, 0));

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
            var testId = "Test_ID";
            var smokeId = "SMOKE_ID";

            var user = new User()
            {
                Id = userId,
                TestId = testId
            };

            var test = new Test()
            {
                Id = testId,
                UserId = userId
            };

            var smoke = new Smoke()
            {
                Id = smokeId,
                StartSmokeTime = dateTime,
                TestId = testId,
            };

            test.SmokedCigaresUnderTest.Add(smoke);

            realm.Write(() =>
            {
                realm.Add(user);

                user.Tests.Add(test);
            });

            underTestViewModel.CurrentSmokeId = smokeId;

            // Act
            await underTestViewModel.SmokeOneCigareAsync();

            var userTest = realm.Find<Smoke>(smokeId);
            //Assert
            Assert.True(smoke.TestId.Equals(testId));
            Assert.True(smoke.EndSmokeTime.LocalDateTime.Equals(dateTime));
            Assert.True(smoke.ModifiedOn.LocalDateTime.Equals(dateTime));

        }

        /// <summary>
        /// Updates View Model State
        /// </summary>
        [Test]
        public async Task Update_ViewModel_State_When_Smoke_One_Cigare()
        {
            //Arrange
            var config = new InMemoryConfiguration(Guid.NewGuid().ToString());
            var realm = Realm.GetInstance(config);

            var dateTime = DateTime.Now;
            var timeSenceLastSmoke = new TimeSpan(1, 2, 3);
            var navigationServiceMock = new Mock<INavigationService>();

            var dateTimeWrapperMock = new Mock<IDateTimeWrapper>();
            dateTimeWrapperMock.Setup(e => e.Now()).Returns(dateTime);

            var appLoggerServiceMock = new Mock<IAppLogger>();
            var dialogServiceMock = new Mock<IDialogService>();


            var testCalculationServiceMock = new Mock<ITestCalculationService>();
            testCalculationServiceMock.Setup(e => e.TimeSinceLastSmoke(It.IsAny<Test>(), It.IsAny<DateTime>()))
                .Returns(timeSenceLastSmoke);

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
            var testId = "Test_ID";
            var smokeId = "SMOKE_ID";

            var user = new User()
            {
                Id = userId,
                TestId = testId
            };

            var test = new Test()
            {
                Id = testId,
                UserId = userId
            };

            var smoke = new Smoke()
            {
                Id = smokeId,
                StartSmokeTime = dateTime,
                TestId = testId,
            };

            test.SmokedCigaresUnderTest.Add(smoke);

            realm.Write(() =>
            {
                realm.Add(user);

                user.Tests.Add(test);
            });

            underTestViewModel.CurrentSmokeId = smokeId;

            // Act
            await underTestViewModel.SmokeOneCigareAsync();

            //Assert
            //Assert.False(underTestViewModel.IsSmoking);
            //Assert.True(underTestViewModel.CurrentlySmokedCount == 1);
            //Assert.True(underTestViewModel.TimeSenceLastSmoke.Equals(timeSenceLastSmoke));

        }


    }
}
