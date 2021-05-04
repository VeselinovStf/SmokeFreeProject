using SmokeFree.Data.Models;
using SmokeFree.Models.Services.Data.Challenge;
using System;

namespace SmokeFree.Abstraction.Services.Data.Test
{
    /// <summary>
    /// Challenge Calculation Service Abstraction
    /// </summary>
    public interface IChallengeCalculationService
    {
        // <summary>
        /// Calculate all challenge smokes for each day
        /// </summary>
        CalculatedChallengeSmokesResponse CalculatedChallengeSmokes(
            DateTime goalTime,
            double avarageSmokedCigarsPerDay,
            double avarageSmokeActiveTimeSeconds,
            string challengeId,
            DateTime timeNow);

        /// <summary>
        /// Calculate Challenge Result by Challenge
        /// </summary>
        /// <param name="challenge">Challenge entity for calculations</param>
        /// <returns>Response Model</returns>
        CalculateChallengeResultResponse CalculateChallengeResult(Challenge challenge);
    }
}
