using SmokeFree.Data.Models;

namespace SmokeFree.Models.Services.Data.Challenge
{
    public class CalculateChallengeResultResponse
    {
        public CalculateChallengeResultResponse(bool success)
        {
            Success = success;
        }

        public CalculateChallengeResultResponse(bool success, ChallengeResult challengeResult) : this(success)
        {
            ChallengeResult = challengeResult;
        }

        public CalculateChallengeResultResponse(bool success, string message) : this(success)
        {
            Message = message;
        }

        public bool Success { get; }
        public string Message { get; }
        public ChallengeResult ChallengeResult { get; }
    }
}
