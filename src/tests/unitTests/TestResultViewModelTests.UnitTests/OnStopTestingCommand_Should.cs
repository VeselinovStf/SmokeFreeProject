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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TestResultViewModelTests.UnitTests
{
    /// <summary>
    /// TestResultViewModel - OnStopTestingCommand Tests
    /// </summary>
    public class OnStopTestingCommand_Should
    {
        /// <summary>
        /// Change all values connected to current test, and mark test as stopped
        /// </summary>
        [Test]
        public void Change_Db_State_To_Stopped_Testing_When_User_Confirms()
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

            dialogServiceMock.Setup(e => e.ConfirmAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(Task.FromResult(true));


            var testResultViewModel = new TestResultViewModel(
                realm,
                navigationServiceMock.Object,
                dateTimeWrapperMock.Object,
                appLoggerServiceMock.Object,
                dialogServiceMock.Object
                );

            var userId = Globals.UserId;
            var testId = "1";

            var user = new User()
            {
                Id = userId,
                TestId = testId,
                NotificationState = false,
                UserState = UserStates.UserUnderTesting.ToString()
            };

            var userTestsOne = new Test()
            {
                Id = testId,
                UserId = userId
            };

            var testResult = new TestResult()
            {
                TestId = testId
            };

            userTestsOne.CompletedTestResult = testResult;

            var userTestsTwo = new Test()
            {
                Id = "2",
                UserId = 2
            };

            var testSmokesOne = new Smoke()
            {
                Id = "1",
                TestId = testId
            };

            var testSmokesTwo = new Smoke()
            {
                Id = "2",
                TestId = testId
            };

            var userChallenge = new Challenge()
            {
                Id = "CHALLENGE1",
                UserId = userId
            };

            var userChallengeTwo = new Challenge()
            {
                Id = "CHALLENGE2",
                UserId = 2
            };

            realm.Write(() =>
            {
                realm.Add(user);
                user.Tests.Add(userTestsOne);
                userTestsOne.SmokedCigaresUnderTest.Add(testSmokesOne);
                userTestsOne.SmokedCigaresUnderTest.Add(testSmokesTwo);
                realm.Add(userTestsTwo);
                user.Challenges.Add(userChallenge);
                realm.Add(userChallengeTwo);
            });

            testResultViewModel.OnStopTestingCommand.Execute(new object());

            var userAfterCommand = realm.Find<User>(userId);
            var userTestAfterCommand = userAfterCommand.Tests.FirstOrDefault(t => t.UserId == userId);
            //Assert
            Assert.True(userTestAfterCommand.IsDeleted);
            Assert.True(userTestAfterCommand.DeletedOn.LocalDateTime.Equals(dateTime));
            Assert.True(userTestAfterCommand.ModifiedOn.LocalDateTime.Equals(dateTime));

            foreach (var smoke in userTestAfterCommand.SmokedCigaresUnderTest)
            {
                Assert.True(smoke.IsDeleted);
                Assert.True(smoke.DeletedOn.LocalDateTime.Equals(dateTime));
                Assert.True(smoke.ModifiedOn.LocalDateTime.Equals(dateTime));
            }

            var userTestChallengeAdterCommand = userAfterCommand.Challenges.FirstOrDefault(c => c.UserId == userId);

            Assert.True(userTestChallengeAdterCommand.IsDeleted);
            Assert.True(userTestChallengeAdterCommand.DeletedOn.LocalDateTime.Equals(dateTime));
            Assert.True(userTestChallengeAdterCommand.ModifiedOn.LocalDateTime.Equals(dateTime));

            Assert.True(userAfterCommand.TestId.Equals(string.Empty));
            Assert.True(userAfterCommand.UserState.Equals(UserStates.CompletedOnBoarding.ToString()));
        }

       

        /// <summary>
        /// Navigates to correct view model when command is executed correctly
        /// </summary>
        [Test]
        public void Navigate_To_CreateTestViewModel_When_Execution_Process_Is_Correct()
        {
            //Arrange
            var config = new InMemoryConfiguration(Guid.NewGuid().ToString());
            var realm = Realm.GetInstance(config);

            var dateTime = DateTime.Now;

            var navigationServiceMock = new Mock<INavigationService>();
            navigationServiceMock.Setup(e => e.NavigateToAsync<CreateTestViewModel>());

            var dateTimeWrapperMock = new Mock<IDateTimeWrapper>();
            dateTimeWrapperMock.Setup(e => e.Now()).Returns(dateTime);

            var appLoggerServiceMock = new Mock<IAppLogger>();
            var dialogServiceMock = new Mock<IDialogService>();

            dialogServiceMock.Setup(e => e.ConfirmAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(Task.FromResult(true));

            var testResultViewModel = new TestResultViewModel(
                realm,
                navigationServiceMock.Object,
                dateTimeWrapperMock.Object,
                appLoggerServiceMock.Object,
                dialogServiceMock.Object
                );

            var userId = Globals.UserId;
            var testId = "1";

            var user = new User()
            {
                Id = userId,
                TestId = testId,
                NotificationState = false,
                UserState = UserStates.UserUnderTesting.ToString()
            };

            var userTestsOne = new Test()
            {
                Id = testId,
                UserId = userId
            };

            var userChallenge = new Challenge()
            {
                Id = "CHALLENGE1",
                UserId = userId
            };

            var testResult = new TestResult()
            {
                TestId = testId
            };

            userTestsOne.CompletedTestResult = testResult;

            realm.Write(() =>
            {
                realm.Add(user);
                user.Tests.Add(userTestsOne);
                user.Challenges.Add(userChallenge);
            });

            // Act
            testResultViewModel.OnStopTestingCommand.Execute(new object());

            // Assert
            navigationServiceMock.Verify(e => e.NavigateToAsync<CreateTestViewModel>(), Times.Exactly(1));

        }

        /// <summary>
        /// Logs when user is not found
        /// </summary>
        [Test]
        public void Log_Critical_If_User_Is_Not_Found_In_DB()
        {
            //Arrange
            var config = new InMemoryConfiguration(Guid.NewGuid().ToString());
            var realm = Realm.GetInstance(config);

            var dateTime = DateTime.Now;

            var navigationServiceMock = new Mock<INavigationService>();

            var dateTimeWrapperMock = new Mock<IDateTimeWrapper>();
            dateTimeWrapperMock.Setup(e => e.Now()).Returns(dateTime);

            var appLoggerServiceMock = new Mock<IAppLogger>();
            appLoggerServiceMock.Setup(e => e.LogCritical(It.IsAny<string>(), It.IsAny<string>()));

            var dialogServiceMock = new Mock<IDialogService>();

            dialogServiceMock.Setup(e => e.ConfirmAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(Task.FromResult(true));

            var testResultViewModel = new TestResultViewModel(
                realm,
                navigationServiceMock.Object,
                dateTimeWrapperMock.Object,
                appLoggerServiceMock.Object,
                dialogServiceMock.Object
                );

            var user = new User()
            {
                Id = 2,
                NotificationState = false,
                UserState = UserStates.UserUnderTesting.ToString()
            };

            realm.Write(() =>
            {
                realm.Add(user);
            });

            // Act
            testResultViewModel.OnStopTestingCommand.Execute(new object());

            // Assert
            appLoggerServiceMock.Verify(e => e.LogCritical(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(1));

        }

        /// <summary>
        /// Logs when user test result is not found
        /// </summary>
        [Test]
        public void Log_Error_If_User_Test_Result_Is_Not_Found_In_DB()
        {
            //Arrange
            var config = new InMemoryConfiguration(Guid.NewGuid().ToString());
            var realm = Realm.GetInstance(config);

            var dateTime = DateTime.Now;

            var navigationServiceMock = new Mock<INavigationService>();

            var dateTimeWrapperMock = new Mock<IDateTimeWrapper>();
            dateTimeWrapperMock.Setup(e => e.Now()).Returns(dateTime);

            var appLoggerServiceMock = new Mock<IAppLogger>();
            appLoggerServiceMock.Setup(e => e.LogError(It.IsAny<string>(), It.IsAny<string>()));

            var dialogServiceMock = new Mock<IDialogService>();

            dialogServiceMock.Setup(e => e.ConfirmAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(Task.FromResult(true));

            var testResultViewModel = new TestResultViewModel(
                realm,
                navigationServiceMock.Object,
                dateTimeWrapperMock.Object,
                appLoggerServiceMock.Object,
                dialogServiceMock.Object
                );

            var userId = Globals.UserId;
            var testId = "1";

            var user = new User()
            {
                Id = userId,
                TestId = testId,
                NotificationState = false,
                UserState = UserStates.UserUnderTesting.ToString()
            };

            var userTestsOne = new Test()
            {
                Id = testId,
                UserId = userId
            };

            var userChallenge = new Challenge()
            {
                Id = "CHALLENGE1",
                UserId = userId
            };


            realm.Write(() =>
            {
                realm.Add(user);
                user.Tests.Add(userTestsOne);
                user.Challenges.Add(userChallenge);
            });

            // Act
            testResultViewModel.OnStopTestingCommand.Execute(new object());

            // Assert
            appLoggerServiceMock.Verify(e => e.LogError(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(1));

        }

        /// <summary>
        /// Logs when user test is not found
        /// </summary>
        [Test]
        public void Log_Critical_If_User_Test_Is_Not_Found_In_DB()
        {
            //Arrange
            var config = new InMemoryConfiguration(Guid.NewGuid().ToString());
            var realm = Realm.GetInstance(config);

            var dateTime = DateTime.Now;

            var navigationServiceMock = new Mock<INavigationService>();

            var dateTimeWrapperMock = new Mock<IDateTimeWrapper>();
            dateTimeWrapperMock.Setup(e => e.Now()).Returns(dateTime);

            var appLoggerServiceMock = new Mock<IAppLogger>();
            appLoggerServiceMock.Setup(e => e.LogError(It.IsAny<string>(), It.IsAny<string>()));

            var dialogServiceMock = new Mock<IDialogService>();

            dialogServiceMock.Setup(e => e.ConfirmAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(Task.FromResult(true));

            var testResultViewModel = new TestResultViewModel(
                realm,
                navigationServiceMock.Object,
                dateTimeWrapperMock.Object,
                appLoggerServiceMock.Object,
                dialogServiceMock.Object
                );

            var userId = Globals.UserId;


            var user = new User()
            {
                Id = userId,
                NotificationState = false,
                UserState = UserStates.UserUnderTesting.ToString()
            };

            realm.Write(() =>
            {
                realm.Add(user);
            });

            // Act
            testResultViewModel.OnStopTestingCommand.Execute(new object());

            // Assert
            appLoggerServiceMock.Verify(e => e.LogError(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(1));

        }

        /// <summary>
        /// Logs when user test is not found
        /// </summary>
        [Test]
        public void Notifies_User_If_User_Test_Is_Not_Found_In_DB()
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

            dialogServiceMock.Setup(e => e.ConfirmAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(Task.FromResult(true));

            var testResultViewModel = new TestResultViewModel(
                realm,
                navigationServiceMock.Object,
                dateTimeWrapperMock.Object,
                appLoggerServiceMock.Object,
                dialogServiceMock.Object
                );

            var userId = Globals.UserId;

            var user = new User()
            {
                Id = userId,
                NotificationState = false,
                UserState = UserStates.UserUnderTesting.ToString()
            };

            realm.Write(() =>
            {
                realm.Add(user);
            });

            // Act
            testResultViewModel.OnStopTestingCommand.Execute(new object());

            //Assert
            navigationServiceMock.Verify(e => e.NavigateToAsync<SomethingWentWrongViewModel>(), Times.Exactly(1));

        }

        /// <summary>
        /// Logs when user test result is not found
        /// </summary>
        [Test]
        public void Notifies_User_If_User_Test_Result_Is_Not_Found_In_DB()
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

            dialogServiceMock.Setup(e => e.ConfirmAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(Task.FromResult(true));

            var testResultViewModel = new TestResultViewModel(
                realm,
                navigationServiceMock.Object,
                dateTimeWrapperMock.Object,
                appLoggerServiceMock.Object,
                dialogServiceMock.Object
                );

            var userId = Globals.UserId;
            var testId = "1";

            var user = new User()
            {
                Id = userId,
                TestId = testId,
                NotificationState = false,
                UserState = UserStates.UserUnderTesting.ToString()
            };

            var userTestsOne = new Test()
            {
                Id = testId,
                UserId = userId
            };

            var userChallenge = new Challenge()
            {
                Id = "CHALLENGE1",
                UserId = userId
            };


            realm.Write(() =>
            {
                realm.Add(user);
                user.Tests.Add(userTestsOne);
                user.Challenges.Add(userChallenge);
            });

            // Act
            testResultViewModel.OnStopTestingCommand.Execute(new object());

            //Assert
            navigationServiceMock.Verify(e => e.NavigateToAsync<SomethingWentWrongViewModel>(), Times.Exactly(1));

        }

        /// <summary>
        /// Logs when user  is not found
        /// </summary>
        [Test]
        public void Notifies_User_If_User_Is_Not_Found_In_DB()
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

            dialogServiceMock.Setup(e => e.ConfirmAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(Task.FromResult(true));

            var notificationManagerMock = new Mock<INotificationManager>();
            var testCalculationServiceMock = new Mock<ITestCalculationService>();
            var deviceTimerMock = new Mock<IDeviceTimer>();

            var testResultViewModel = new TestResultViewModel(
                realm,
                navigationServiceMock.Object,
                dateTimeWrapperMock.Object,
                appLoggerServiceMock.Object,
                dialogServiceMock.Object
                );


            var user = new User()
            {
                Id = 2,
                NotificationState = false,
                UserState = UserStates.UserUnderTesting.ToString()
            };

            realm.Write(() =>
            {
                realm.Add(user);
            });

            // Act
            testResultViewModel.OnStopTestingCommand.Execute(new object());

            //Assert
            navigationServiceMock.Verify(e => e.NavigateToAsync<SomethingWentWrongViewModel>(), Times.Exactly(1));

        }
    }
}

