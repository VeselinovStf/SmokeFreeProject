using Moq;
using NUnit.Framework;
using Realms;
using SmokeFree;
using SmokeFree.Abstraction.Services.General;
using SmokeFree.Abstraction.Utility.Logging;
using SmokeFree.Abstraction.Utility.Wrappers;
using SmokeFree.Data.Models;
using SmokeFree.Models.Views.Test;
using SmokeFree.ViewModels.Test;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CreateTestViewModelTests.UnitTests
{
    /// <summary>
    /// OnStartTestingCommand Tests
    /// </summary>
    public class OnStartTestingCommand_Should
    {
        /// <summary>
        /// Start Testing Should Navigate to next view when user confirm is OK
        /// </summary>
        //[Test]
        //public void Navigate_To_Next_View_When_User_Confirm_Ok()
        //{
        //    //Arrange
        //    var config = new InMemoryConfiguration("Navigate_To_Next_View_When_User_Confirm_Ok");
        //    var realm = Realm.GetInstance(config);

        //    var navigationServiceMock = new Mock<INavigationService>();
        //    navigationServiceMock.Setup(e => e.NavigateToAsync<UnderTestViewModel>());
        //    var dateTimeWrapperMock = new Mock<IDateTimeWrapper>();
        //    var appLoggerServiceMock = new Mock<IAppLogger>();
        //    var dialogServiceMock = new Mock<IDialogService>();

        //    dialogServiceMock.Setup(e => e.ConfirmAsync(
        //            It.IsAny<string>(), 
        //            It.IsAny<string>(), 
        //            It.IsAny<string>(), 
        //            It.IsAny<string>()))
        //        .Returns(Task.FromResult(true));

        //    var createTestViewModel = new CreateTestViewModel(
        //        realm,
        //        navigationServiceMock.Object,
        //        dateTimeWrapperMock.Object,
        //        appLoggerServiceMock.Object,
        //        dialogServiceMock.Object
        //        );

        //    var testDuration = new TestDurationItem()
        //    {
        //        DayValue = 1,
        //        DisplayText = "text"
        //    };

        //    createTestViewModel.SelectedTestTimeDurationItem = testDuration;

        //    // Act
        //    var call = createTestViewModel.OnStartTestingCommand.ExecuteAsync().ConfigureAwait(false);

        //    //Assert
        //    navigationServiceMock.Verify(e => e.NavigateToAsync<UnderTestViewModel>(), Times.Once);
        //}

        /// <summary>
        /// Stay on same view when user confirm is cancel
        /// </summary>
        [Test]
        public void Stay_On_Same_View_When_User_Confirm_Cancel()
        {
            //Arrange
            var config = new InMemoryConfiguration("Stay_On_Same_View_When_User_Confirm_Cancel");
            var realm = Realm.GetInstance(config);

            var navigationServiceMock = new Mock<INavigationService>();
            navigationServiceMock.Setup(e => e.NavigateToAsync<UnderTestViewModel>());
            var dateTimeWrapperMock = new Mock<IDateTimeWrapper>();
            var appLoggerServiceMock = new Mock<IAppLogger>();
            var dialogServiceMock = new Mock<IDialogService>();

            dialogServiceMock.Setup(e => e.ConfirmAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(Task.FromResult(false));

            var createTestViewModel = new CreateTestViewModel(
                realm,
                navigationServiceMock.Object,
                dateTimeWrapperMock.Object,
                appLoggerServiceMock.Object,
                dialogServiceMock.Object
                );

            var testDuration = new TestDurationItem()
            {
                DayValue = 1,
                DisplayText = "text"
            };

            createTestViewModel.SelectedTestTimeDurationItem = testDuration;

            // Act
            var call = createTestViewModel.OnStartTestingCommand.ExecuteAsync().ConfigureAwait(false);

            //Assert
            navigationServiceMock.Verify(e => e.NavigateToAsync<UnderTestViewModel>(), Times.Exactly(0));
        }

        /// <summary>
        /// Start Testing Should Create User Test
        /// </summary>
        [Test]
        public void Create_User_Test()
        {
            //Arrange
            var config = new InMemoryConfiguration("Create_User_Test");
            var realm = Realm.GetInstance(config);

            var navigationServiceMock = new Mock<INavigationService>();
            var dateTimeWrapperMock = new Mock<IDateTimeWrapper>();
            var appLoggerServiceMock = new Mock<IAppLogger>();
            var dialogServiceMock = new Mock<IDialogService>();

            dialogServiceMock.Setup(e => e.ConfirmAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(Task.FromResult(true));

            var createTestViewModel = new CreateTestViewModel(
                realm,
                navigationServiceMock.Object,
                dateTimeWrapperMock.Object,
                appLoggerServiceMock.Object,
                dialogServiceMock.Object
                );

            var dateTime = DateTime.Now;

            dateTimeWrapperMock.Setup(e => e.Now()).Returns(dateTime);

            var testDurationCount = 1;
            var userId = Globals.UserId;
            var goalTime = dateTime.AddDays(Globals.MinChallengeDays + 1);

            var testDuration = new TestDurationItem()
            {
                DayValue = testDurationCount,
                DisplayText = "text"
            };

            var user = new User()
            {
                Id = userId,
                UserState = UserStates.CompletedOnBoarding.ToString()
            };

            createTestViewModel.SelectedTestTimeDurationItem = testDuration;
            createTestViewModel.GoalDateTime = goalTime;

            realm.Write(() =>
            {
                realm.Add(user);
            });

            // Act
            var call = createTestViewModel.OnStartTestingCommand.ExecuteAsync();

            //Assert
            var addedTests = user.Tests.Where(t => t.UserId == userId).ToList();

            Assert.True(addedTests.Count == 1);

            var addedTest = addedTests.FirstOrDefault(e => e.UserId == userId);
            
            Assert.True(addedTest.TestStartDate.Equals(dateTime));
            Assert.True(addedTest.CreatedOn.Equals(dateTime));
            Assert.True(addedTest.IsCompleted == false);
        }
    }
}
