using Moq;
using NUnit.Framework;
using Realms;
using SmokeFree;
using SmokeFree.Abstraction.Services.General;
using SmokeFree.Abstraction.Utility.Logging;
using SmokeFree.Abstraction.Utility.Wrappers;
using SmokeFree.Data.Models;
using SmokeFree.ViewModels.ErrorAndEmpty;
using SmokeFree.ViewModels.Test;
using System;
using System.Threading.Tasks;

namespace TestResultViewModelTests.UnitTests
{
    /// <summary>
    /// TestResultViewModel - InitializeAsync Tests
    /// </summary>
    public class InitializeAsync_Should
    {
        /// <summary>
        /// Show SomethingWentWrongViewModel to user if user is not found in DB
        /// </summary>
        [Test]
        public async Task Navagate_To_SomethingWentWrongViewModel_When_User_Is_Not_Found()
        {
            //Arrange
            var config = new InMemoryConfiguration(Guid.NewGuid().ToString());
            var realm = Realm.GetInstance(config);

            var navigationServiceMock = new Mock<INavigationService>();
            navigationServiceMock.Setup(e => e.NavigateToAsync<SomethingWentWrongViewModel>());
            var dateTimeWrapperMock = new Mock<IDateTimeWrapper>();
            var appLoggerServiceMock = new Mock<IAppLogger>();
            var dialogServiceMock = new Mock<IDialogService>();

           
            var testResultViewModel = new TestResultViewModel(
                realm,
                navigationServiceMock.Object,
                dateTimeWrapperMock.Object,
                appLoggerServiceMock.Object,
                dialogServiceMock.Object);

            // Act
            await testResultViewModel.InitializeAsync(new object());

            //Assert
            navigationServiceMock.Verify(e => e.NavigateToAsync<SomethingWentWrongViewModel>(), Times.Once);
        }

        /// <summary>
        /// Show SomethingWentWrongViewModel to user if user test is not found in DB
        /// </summary>
        [Test]
        public async Task Navagate_To_SomethingWentWrongViewModel_When_User_Test_Is_Not_Found()
        {
            //Arrange
            var config = new InMemoryConfiguration(Guid.NewGuid().ToString());
            var realm = Realm.GetInstance(config);

            var navigationServiceMock = new Mock<INavigationService>();
            navigationServiceMock.Setup(e => e.NavigateToAsync<SomethingWentWrongViewModel>());
            var dateTimeWrapperMock = new Mock<IDateTimeWrapper>();
            var appLoggerServiceMock = new Mock<IAppLogger>();
            var dialogServiceMock = new Mock<IDialogService>();

            var userId = Globals.UserId;
            var user = new User()
            {
                Id = userId
            };

            realm.Write(() =>
            {
                realm.Add(user);
            });

            var testResultViewModel = new TestResultViewModel(
                realm,
                navigationServiceMock.Object,
                dateTimeWrapperMock.Object,
                appLoggerServiceMock.Object,
                dialogServiceMock.Object);
            
            // Act
            await testResultViewModel.InitializeAsync(new object());

            //Assert
            navigationServiceMock.Verify(e => e.NavigateToAsync<SomethingWentWrongViewModel>(), Times.Once);
        }

        /// <summary>
        /// Show SomethingWentWrongViewModel to user if user test result is not found in DB
        /// </summary>
        [Test]
        public async Task Navagate_To_SomethingWentWrongViewModel_When_User_Test_Result_Is_Not_Found()
        {
            //Arrange
            var config = new InMemoryConfiguration(Guid.NewGuid().ToString());
            var realm = Realm.GetInstance(config);

            var navigationServiceMock = new Mock<INavigationService>();
            navigationServiceMock.Setup(e => e.NavigateToAsync<SomethingWentWrongViewModel>());
            var dateTimeWrapperMock = new Mock<IDateTimeWrapper>();
            var appLoggerServiceMock = new Mock<IAppLogger>();
            var dialogServiceMock = new Mock<IDialogService>();

            var testId = "TEST_ID";
            var userId = Globals.UserId;
            var user = new User()
            {
                Id = userId,
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

            var testResultViewModel = new TestResultViewModel(
                realm,
                navigationServiceMock.Object,
                dateTimeWrapperMock.Object,
                appLoggerServiceMock.Object,
                dialogServiceMock.Object);

            // Act
            await testResultViewModel.InitializeAsync(new object());

            //Assert
            navigationServiceMock.Verify(e => e.NavigateToAsync<SomethingWentWrongViewModel>(), Times.Once);
        }

        /// <summary>
        /// Sets TestResult Property when User TestResult is created
        /// </summary>
        [Test]
        public async Task Set_TestResult_Property_When_User_State_Is_Correct()
        {
            //Arrange
            var config = new InMemoryConfiguration(Guid.NewGuid().ToString());
            var realm = Realm.GetInstance(config);

            var navigationServiceMock = new Mock<INavigationService>();
            var dateTimeWrapperMock = new Mock<IDateTimeWrapper>();
            var appLoggerServiceMock = new Mock<IAppLogger>();
            var dialogServiceMock = new Mock<IDialogService>();

            var testId = "TEST_ID";
            var userId = Globals.UserId;
            var user = new User()
            {
                Id = userId,
                TestId = testId
            };

            var userTest = new Test()
            {
                Id = testId,
                UserId = Globals.UserId
            };

            var testResult = new TestResult()
            {
                TestId = testId
            };

            userTest.CompletedTestResult = testResult;

            realm.Write(() =>
            {
                realm.Add(user);

                user.Tests.Add(userTest);
            });

            var testResultViewModel = new TestResultViewModel(
                realm,
                navigationServiceMock.Object,
                dateTimeWrapperMock.Object,
                appLoggerServiceMock.Object,
                dialogServiceMock.Object);

            // Act
            await testResultViewModel.InitializeAsync(new object());

            //Assert
            Assert.NotNull(testResultViewModel.TestResult);
            Assert.True(testResultViewModel.TestResult.TestId.Equals(testId));
        }
    }
}
