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

        public CalculatedChallengeSmokesResponse(bool success, List<DayChallengeSmoke> dayChallengeSmokes) : this(success)
        {
            DayChallengeSmokes = dayChallengeSmokes;
        }

        public CalculatedChallengeSmokesResponse(bool success, string message) : this(success)
        {
            Message = message;
        }

        public List<DayChallengeSmoke> DayChallengeSmokes { get; set; }

        public bool Success { get; }
        public string Message { get; }
    }
}
