using SmokeFree.Abstraction.Services.Data.Test;
using SmokeFree.Data.Models;
using SmokeFree.Models.Services.Data.Challenge;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmokeFree.Services.Data.Challenge
{
    /// <summary>
    /// Challenge Calculation Service
    /// </summary>
    public class ChallengeCalculationService : IChallengeCalculationService
    {
        /// <summary>
        /// Calculate Challenge Result by Challenge
        /// </summary>
        /// <param name="challenge">Challenge entity for calculations</param>
        /// <returns>Response Model</returns>
        public CalculateChallengeResultResponse CalculateChallengeResult(SmokeFree.Data.Models.Challenge challenge)
        {
            try
            {
                if (challenge == null)
                {
                    return new CalculateChallengeResultResponse(false, "Parrameter Challenge was null");
                }

                var totalChallengeDays = challenge.GoalCompletitionTime.LocalDateTime.Subtract(challenge.ChallengeStart.LocalDateTime).TotalDays;

                var totalSmokedCigarsUnderChallengeSet = challenge.ChallengeSmokes
                    .SelectMany(e => e.DaySmokes)
                    .Where(e => (!e.StartSmokeTime.Equals(new DateTimeOffset()) && !e.EndSmokeTime.Equals(new DateTimeOffset())) || e.IsSkiped);

                var avarageSmokedUnderChallenge = totalSmokedCigarsUnderChallengeSet == null ? 0 : totalSmokedCigarsUnderChallengeSet.Count() / totalChallengeDays;

                var expectedToBeSmoked = challenge.ChallengeSmokes
                    .Sum(e => e.DayMaxSmokesLimit);

                var actualyToBeSmoked = challenge.ChallengeSmokes
                    .Sum(e => e.DaySmoked);

                var successRate = 0.0;
                if (actualyToBeSmoked < expectedToBeSmoked)
                {
                    successRate = (actualyToBeSmoked / expectedToBeSmoked) * 100;
                   
                }
                else
                {
                    successRate = 100 - (actualyToBeSmoked / expectedToBeSmoked);
                }

                var skiped = challenge.ChallengeSmokes
                    .SelectMany(e => e.DaySmokes)
                    .Where(e => e.IsSkiped);

                var totalSkiped = skiped != null ? skiped.Count() : 0;
                var rating = successRate + totalSkiped + totalChallengeDays;

                var challengeResult = new ChallengeResult()
                {
                    ChallengeStart = challenge.ChallengeStart,
                    GoalCompletitionTime = challenge.GoalCompletitionTime,
                    TotalChallengeDays = (int)totalChallengeDays,
                    TotalSmokedCigarsUnderChallenge = totalSmokedCigarsUnderChallengeSet != null ? totalSmokedCigarsUnderChallengeSet.Count() : 0,
                    AvarageSmokedUnderChallenge = avarageSmokedUnderChallenge,
                    SuccessRate = successRate,
                    Skiped = totalSkiped,
                    Rating = rating
                };

                return new CalculateChallengeResultResponse(true, challengeResult);
            }
            catch (Exception ex)
            {
                return new CalculateChallengeResultResponse(false, ex.Message);
            }
        }

        /// <summary>
        /// Calculate all challenge smokes for each day
        /// </summary>
        /// <returns>Response Model</returns>
        public CalculatedChallengeSmokesResponse CalculatedChallengeSmokes(
            DateTime goalTime,
            double avarageSmokedADay,
            double avarageSmokeActiveTime,
            string challengeId,
            DateTime timeNow)
        {
            try
            {
                var totalGoalDays = (int)Math.Abs((goalTime - timeNow).Days);

                // Amount of smokes to remove for each day 
                // in order to complete goal time
                var avarageRemoveSmokeADay = avarageSmokedADay / totalGoalDays;

                // Smoked for day
                var dailySmoked = avarageSmokedADay;

                var result = new List<DayChallengeSmoke>();

                // For each day
                for (int i = 0; i < totalGoalDays; i++)
                {
                    var currentDaySmokeDistanceMinutes = Math.Abs((avarageSmokeActiveTime / dailySmoked) * 60);

                    var newChallengeSmoke = new DayChallengeSmoke()
                    {
                        DistanceToNextInSeconds = (int)currentDaySmokeDistanceMinutes * 60,
                        DayOfChallenge = i + 1,
                        ChallengeId = challengeId,
                        DayMaxSmokesLimit = (int)dailySmoked,
                    };

                    result.Add(newChallengeSmoke);

                    dailySmoked = dailySmoked - avarageRemoveSmokeADay;
                }

                if (result.Count == 0)
                {
                    return new CalculatedChallengeSmokesResponse(
                        false,$"Can't Generate Challenge Daily Smoke Data! Challenge Id: {challengeId}!");
                }

                return new CalculatedChallengeSmokesResponse(true, result, totalGoalDays);

            }
            catch (Exception ex)
            {
                return new CalculatedChallengeSmokesResponse(false, ex.Message);
            }

        }
    }
}
