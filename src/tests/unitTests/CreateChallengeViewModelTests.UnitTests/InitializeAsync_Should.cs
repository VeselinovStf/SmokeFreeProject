using Moq;
using NUnit.Framework;
using Realms;
using SmokeFree;
using SmokeFree.Abstraction.Services.Data.Test;
using SmokeFree.Abstraction.Services.General;
using SmokeFree.Abstraction.Utility.Logging;
using SmokeFree.Abstraction.Utility.Wrappers;
using SmokeFree.Data.Models;
using SmokeFree.Models.Views.Challenge;
using SmokeFree.Resx;
using SmokeFree.Utilities.UserStateHelpers;
using SmokeFree.ViewModels.Challenge;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CreateChallengeViewModelTests.UnitTests
{
    public class InitializeAsync_Should
    {
        /// <summary>
        /// Initilize Async Should Set All Properties
        /// </summary>
        [Test]
        public async Task Initialize_Initial_State_Of_CreateChallengeViewModel_With_Values()
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
            var challengeCalculationServiceMock = new Mock<IChallengeCalculationService>();
            var notificationCenterMock = new Mock<INotificationCenterService>();


            var createChallengeViewModel = new CreateChallengeViewModel(
                realm,
                navigationServiceMock.Object,
                dateTimeWrapperMock.Object,
                appLoggerServiceMock.Object,
                dialogServiceMock.Object,
                challengeCalculationServiceMock.Object,
                notificationCenterMock.Object
                );

            object parameter = new object();
            var challengeDays = 5;
            var challengeGoalTime = dateTime.AddDays(challengeDays);
            var challengeId = "Challenge_ID";

            var userChallenge = new Challenge()
            {
                Id = challengeId,
                UserId = Globals.UserId,
                GoalCompletitionTime = challengeGoalTime
            };

            var user = new User()
            {
                Id = Globals.UserId,
                UserSmokeStatuses = UserSmokeStatuses.Bad.ToString()
            };

            realm.Write(() =>
            {
                realm.Add(user);

                user.Challenges.Add(userChallenge);
            });

            var userSmokerStatus = StringToEnum.ToUserState<UserSmokeStatuses>(user.UserSmokeStatuses);
            var userSmokerStatusIcon = Globals.UserSmokeStatusesSet[userSmokerStatus].First().Item1;
            var userSmokerStatusMessage = Globals.UserSmokeStatusesSet[userSmokerStatus].First().Item2;

            var userSmokeStatus = new UserSmokeStatusItem()
            {
                Icon = userSmokerStatusIcon,
                Title = ResourceManagerMock.GetResourceManager().GetString("UserSmokeStatusTitle"),
                Message = userSmokerStatusMessage
            };


            // Act
            await createChallengeViewModel.InitializeAsync(parameter);

            //Assert
            Assert.True(createChallengeViewModel.GoalCompletitionTime.Equals(dateTime.AddDays(challengeDays)));
            Assert.True(createChallengeViewModel.CurrentChallengeId.Equals(challengeId));
            Assert.True(userSmokeStatus.Icon.Equals(createChallengeViewModel.UserSmokeStatus.Icon));
            Assert.True(userSmokeStatus.Title.Equals(createChallengeViewModel.UserSmokeStatus.Title));
            Assert.True(userSmokeStatus.Message.Equals(createChallengeViewModel.UserSmokeStatus.Message));
        }

        /// <summary>
        /// Notify User that goal time is less the current
        /// </summary>
        [Test]
        public async Task Show_Dialog_If_Challenge_GoalTime_Is_Less_Then_Current()
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
            dialogServiceMock.Setup(e => e.ShowDialog(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
                .Returns(Task.FromResult(true));

            var challengeCalculationServiceMock = new Mock<IChallengeCalculationService>();
            var notificationCenterMock = new Mock<INotificationCenterService>();


            var createChallengeViewModel = new CreateChallengeViewModel(
                realm,
                navigationServiceMock.Object,
                dateTimeWrapperMock.Object,
                appLoggerServiceMock.Object,
                dialogServiceMock.Object,
                challengeCalculationServiceMock.Object,
                notificationCenterMock.Object
                );

            object parameter = new object();
            var challengeDays = -5;
            var challengeGoalTime = dateTime.AddDays(challengeDays);
            var challengeId = "Challenge_ID";

            var userChallenge = new Challenge()
            {
                Id = challengeId,
                UserId = Globals.UserId,
                GoalCompletitionTime = challengeGoalTime
            };

            var user = new User()
            {
                Id = Globals.UserId,
            };

            realm.Write(() =>
            {
                realm.Add(user);

                user.Challenges.Add(userChallenge);
            });


            // Act
            await createChallengeViewModel.InitializeAsync(parameter);

            //Assert
            dialogServiceMock.Verify(e => e.ShowDialog(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()), Times.Once());
        }
    }
}