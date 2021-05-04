using SmokeFree.Data.Models;
using System.Collections.Generic;

namespace SmokeFree.Models.Services.Data.Challenge
{
    public class CalculatedChallengeSmokesResponse
    {
        public CalculatedChallengeSmokesResponse(bool success)
        {
            Success = success;
        }

        public CalculatedChallengeSmokesResponse(bool success, List<DayChallengeSmoke> dayChallengeSmokes, int goalTimeInDays) : this(success)
        {
            DayChallengeSmokes = dayChallengeSmokes;
            GoalTimeInDays = goalTimeInDays;
        }

        public CalculatedChallengeSmokesResponse(bool success, string message) : this(success)
        {
            Message = message;
        }

        public List<DayChallengeSmoke> DayChallengeSmokes { get; set; }
        public int GoalTimeInDays { get; }
        public bool Success { get; }
        public string Message { get; }
    }
}
