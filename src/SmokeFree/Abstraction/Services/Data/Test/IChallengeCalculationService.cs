using SmokeFree.Data.Models;
using SmokeFree.Models.Services.Data.Challenge;

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
        /// <param name="totalGoalDays">Total days for goal completition</param>
        /// <param name="avarageSmokedADay">Avarage Smokes For day</param>
        /// <param name="avarageSmokeActiveTime">Avarage active smoke time - (first smoke time - last smoke time avarage)</param>
        /// <param name="challengeId">Challenge Id</param>
        /// <returns>Response Model</returns>
        CalculatedChallengeSmokesResponse CalculatedChallengeSmokes(
            int totalGoalDays,
            double avarageSmokedADay,
            double avarageSmokeActiveTime,
            string challengeId);

        /// <summary>
        /// Calculate Challenge Result by Challenge
        /// </summary>
        /// <param name="challenge">Challenge entity for calculations</param>
        /// <returns>Response Model</returns>
        CalculateChallengeResultResponse CalculateChallengeResult(Challenge challenge);
    }
}
