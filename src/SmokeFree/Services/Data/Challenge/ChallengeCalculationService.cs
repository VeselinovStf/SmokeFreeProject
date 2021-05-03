using SmokeFree.Abstraction.Services.Data.Test;
using SmokeFree.Data.Models;
using SmokeFree.Models.Services.Data.Challenge;
using System;
using System.Collections.Generic;

namespace SmokeFree.Services.Data.Challenge
{
    /// <summary>
    /// Challenge Calculation Service
    /// </summary>
    public class ChallengeCalculationService : IChallengeCalculationService
    {

        /// <summary>
        /// Calculate all challenge smokes for each day
        /// </summary>
        /// <param name="totalGoalDays">Total days for goal completition</param>
        /// <param name="avarageSmokedADay">Avarage Smokes For day</param>
        /// <param name="avarageSmokeActiveTime">Avarage active smoke time - (first smoke time - last smoke time avarage)</param>
        /// <param name="challengeId">Challenge Id</param>
        /// <returns>Response Model</returns>
        public CalculatedChallengeSmokesResponse CalculatedChallengeSmokes(
            int totalGoalDays,
            double avarageSmokedADay,
            double avarageSmokeActiveTime,
            string challengeId)
        {
            try
            {
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

                return new CalculatedChallengeSmokesResponse(true, result);

            }
            catch (Exception ex)
            {
                return new CalculatedChallengeSmokesResponse(false, ex.Message);
            }

        }
    }
}
