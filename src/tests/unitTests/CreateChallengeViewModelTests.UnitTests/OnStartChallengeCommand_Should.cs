using Moq;
using NUnit.Framework;
using Realms;
using SmokeFree;
using SmokeFree.Abstraction.Services.Data.Test;
using SmokeFree.Abstraction.Services.General;
using SmokeFree.Abstraction.Utility.Logging;
using SmokeFree.Abstraction.Utility.Wrappers;
using SmokeFree.Data.Models;
using SmokeFree.Models.Services.Data.Challenge;
using SmokeFree.ViewModels.Challenge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CreateChallengeViewModelTests.UnitTests
{
    /// <summary>
    /// CreateChallengeViewModel - OnStartChallengeCommand Tests
    /// </summary>
    public class OnStartChallengeCommand_Should
    {
        /// <summary>
        /// Test Min Challenge Days Rule
        /// </summary>
        [Test]
        public void Show_Dialog_When_GoalTime_Is_Less_Then_MinChallengeDays()
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
                It.IsAny<string>()));

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

            createChallengeViewModel.GoalCompletitionTime = dateTime.AddDays(2);

            // ACT
            createChallengeViewModel.OnStartChallengeCommand.Execute(new object());

            // ASSERT
            dialogServiceMock.Verify(e => e.ShowDialog(It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()), Times.Once);
        }

        /// <summary>
        /// Updates Challenge
        /// </summary>
        [Test]
        public void Update_Challenge_When_Correct_Data_Is_Present()
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
                It.IsAny<string>()));

            dialogServiceMock.Setup(e => e.ConfirmAsync(
               It.IsAny<string>(),
               It.IsAny<string>(),
               It.IsAny<string>(),
               It.IsAny<string>())).Returns(Task.FromResult(true));

            var notificationCenterMock = new Mock<INotificationCenterService>();

            var userId = Globals.UserId;
            var testId = "TEST_ID";
            var challengeId = "CHALLENGE_ID";
            var testResultId = "TEST_RESULT_ID";
            var ascpd = 10;
            var asats = 4200;

            var test = new Test()
            {
                UserId = userId,
                Id = testId,
                CompletedTestResult = new TestResult()
                {
                    TestId = testId,
                    Id = testResultId,
                    AvarageSmokedCigarsPerDay = ascpd,
                    AvarageSmokeActiveTimeSeconds = asats
                },
                IsCompleted = true
            };

            var challenge = new Challenge()
            {
                Id = challengeId,
                UserId = userId,
            };

            var user = new User()
            {
                Id = userId,
                NotificationState = false,
                UserState = UserStates.InCreateChallenge.ToString()
            };

            user.Tests.Add(test);
            user.Challenges.Add(challenge);

            realm.Write(() =>
            {
                realm.Add(user);
            });


            var goalTimeInDays = 5;
            var goalTime = dateTime.AddDays(goalTimeInDays);

            List<DayChallengeSmoke> dayChallengeSmokesResponse = new List<DayChallengeSmoke>()
            {
                new DayChallengeSmoke()
                {
                    DistanceToNextInSeconds = 10,
                    DayOfChallenge = 1,
                    ChallengeId = challengeId,
                    DayMaxSmokesLimit = 10,
                    CreatedOn = dateTime
                },
                new DayChallengeSmoke()
                {
                    DistanceToNextInSeconds = 20,
                    DayOfChallenge = 2,
                    ChallengeId = challengeId,
                    DayMaxSmokesLimit = 20,
                    CreatedOn = dateTime
                }
            };

            var challengeServiceResponse = new CalculatedChallengeSmokesResponse(true,
                dayChallengeSmokesResponse, goalTimeInDays);

            var challengeCalculationServiceMock = new Mock<IChallengeCalculationService>();
            challengeCalculationServiceMock.Setup(e => e.CalculatedChallengeSmokes(
                It.IsAny<DateTime>(),
                It.IsAny<double>(),
                It.IsAny<double>(),
                It.IsAny<string>(),
                It.IsAny<DateTime>())).Returns(challengeServiceResponse);

            var createChallengeViewModel = new CreateChallengeViewModel(
                realm,
                navigationServiceMock.Object,
                dateTimeWrapperMock.Object,
                appLoggerServiceMock.Object,
                dialogServiceMock.Object,
                challengeCalculationServiceMock.Object,
                notificationCenterMock.Object
                );

            createChallengeViewModel.GoalCompletitionTime = dateTime.AddDays(goalTimeInDays);

            // ACT
            createChallengeViewModel.OnStartChallengeCommand.Execute(new object());

            var updatedUser = realm.Find<User>(userId);
            var updatedChallenge = realm.Find<Challenge>(challengeId);

            // ASSERT
            Assert.True(updatedUser.UserState.Equals(UserStates.InChallenge.ToString()));
            Assert.True(updatedChallenge.ChallengeStart.LocalDateTime.Equals(dateTime));
            Assert.True(updatedChallenge.ModifiedOn.LocalDateTime.Equals(dateTime));
            Assert.True(updatedChallenge.TotalChallengeDays.Equals(goalTimeInDays));
            Assert.True(updatedChallenge.GoalCompletitionTime.LocalDateTime.Equals(dateTime.AddDays(goalTimeInDays)));
            Assert.True(updatedChallenge.ChallengeSmokes.Count() == dayChallengeSmokesResponse.Count());

            var firstDayChallengeExpectedSmokesResponse = dayChallengeSmokesResponse.First();
            var lastDayChallengeExpectedSmokesResponse = dayChallengeSmokesResponse.Last();

            var firstDayChallengeActualSmokesResponse = updatedChallenge.ChallengeSmokes.First();
            var lastDayChallengeActualSmokesResponse = updatedChallenge.ChallengeSmokes.Last();

            Assert.True(firstDayChallengeExpectedSmokesResponse.DistanceToNextInSeconds == firstDayChallengeActualSmokesResponse.DistanceToNextInSeconds);
            Assert.True(firstDayChallengeExpectedSmokesResponse.DayOfChallenge == firstDayChallengeActualSmokesResponse.DayOfChallenge);
            Assert.True(firstDayChallengeExpectedSmokesResponse.ChallengeId.Equals(firstDayChallengeActualSmokesResponse.ChallengeId));
            Assert.True(firstDayChallengeExpectedSmokesResponse.DayMaxSmokesLimit == firstDayChallengeActualSmokesResponse.DayMaxSmokesLimit);
            Assert.True(firstDayChallengeExpectedSmokesResponse.CreatedOn.LocalDateTime.Equals(firstDayChallengeActualSmokesResponse.CreatedOn.LocalDateTime));

            Assert.True(lastDayChallengeExpectedSmokesResponse.DistanceToNextInSeconds == lastDayChallengeActualSmokesResponse.DistanceToNextInSeconds);
            Assert.True(lastDayChallengeExpectedSmokesResponse.DayOfChallenge == lastDayChallengeActualSmokesResponse.DayOfChallenge);
            Assert.True(lastDayChallengeExpectedSmokesResponse.ChallengeId.Equals(lastDayChallengeActualSmokesResponse.ChallengeId));
            Assert.True(lastDayChallengeExpectedSmokesResponse.DayMaxSmokesLimit == lastDayChallengeActualSmokesResponse.DayMaxSmokesLimit);
            Assert.True(lastDayChallengeExpectedSmokesResponse.CreatedOn.LocalDateTime.Equals(lastDayChallengeActualSmokesResponse.CreatedOn.LocalDateTime));
        }
    }
}
