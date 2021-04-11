using System;

namespace SmokeFree.Models.Services.Data.Test
{
    /// <summary>
    /// TestCalculationService - GetCurrentTestDataCalculation DTO Return Model
    /// </summary>
    public class CurrentTestDataCalculationDTO
    {
        public CurrentTestDataCalculationDTO(int currentSmokedCount, TimeSpan? timeSinceLastSmoke, TimeSpan? testTimeLeft, string currentSmokeId, TimeSpan currentSmokeTime)
        {
            CurrentSmokedCount = currentSmokedCount;
            TimeSinceLastSmoke = timeSinceLastSmoke;
            TestTimeLeft = testTimeLeft;
            CurrentSmokeId = currentSmokeId;
            CurrentSmokeTime = currentSmokeTime;
        }


        public int CurrentSmokedCount { get; set; }

        public TimeSpan? TimeSinceLastSmoke { get; set; }

        public TimeSpan? TestTimeLeft { get; set; }

        public TimeSpan CurrentSmokeTime { get; set; }

        public string CurrentSmokeId { get; set; }
    }
}
