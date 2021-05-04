using NUnit.Framework;
using SmokeFree.Services.Data.Challenge;
using System;

namespace ChallengeCalculationServiceTests.UnitTests
{
    /// <summary>
    /// ChallengeCalculationService - CalculatedChallengeSmokes Tests
    /// </summary>
    public class CalculatedChallengeSmokes_Should
    {
        [Test]
        public void Calculate_Correctly_When_Correct_Data_Is_Passed()
        {
            // Arrange
            var timeNow = DateTime.Now;
            var totalGoalTimeDays = 8;
            var goalTime = timeNow.AddDays(totalGoalTimeDays);
            double avarageSmokedADay = 20.0;
            double avarageSmokeActiveTime = 4200.0;
            string challengeId = "CH_ID";

            var calculationService = new ChallengeCalculationService();

            // Act
            var calculationResult = calculationService.CalculatedChallengeSmokes(
                goalTime, 
                avarageSmokedADay, 
                avarageSmokeActiveTime, 
                challengeId, 
                timeNow);

            // Assert
            Assert.True(calculationResult.Success);
            Assert.NotNull(calculationResult.DayChallengeSmokes);
            Assert.True(calculationResult.GoalTimeInDays == totalGoalTimeDays);

        }
    }
}