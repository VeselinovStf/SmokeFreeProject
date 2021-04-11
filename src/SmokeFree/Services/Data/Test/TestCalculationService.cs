using SmokeFree.Abstraction.Services.Data.Test;
using SmokeFree.Models.Services.Data.Test;
using System;
using System.Linq;

namespace SmokeFree.Services.Data.Test
{
    /// <summary>
    /// Service concern in Test Model Calculations
    /// </summary>
    public class TestCalculationService : ITestCalculationService
    {
        /// <summary>
        /// Get Test Data Based on Time
        /// </summary>
        /// <param name="time">DateTime Filter</param>
        /// <param name="test">Test to do Calculations From</param>
        /// <returns>Result data DTO</returns>
        public CurrentTestDataCalculationDTO GetCurrentTestDataCalculation(DateTime time, SmokeFree.Data.Models.Test test)
        {
            var currentSmokeId = string.Empty;
            TimeSpan currentSmokeTime = new TimeSpan(0, 0, 0);

            // Calculate last smoked time
            TimeSpan? timeSinceLastSmoke = null;

            if (test.SmokedCigaresUnderTest.Count > 0)
            {
                var lastSmokeTime = test.SmokedCigaresUnderTest
                    .OrderByDescending(e => e.EndSmokeTime)
                    .FirstOrDefault()
                    .EndSmokeTime;

                timeSinceLastSmoke = time.Subtract(DateTime.Parse(lastSmokeTime.ToString()));

                var lastSmoke = test.SmokedCigaresUnderTest
                    .OrderByDescending(e => e.StartSmokeTime)
                    .FirstOrDefault(e => !e.IsDeleted && e.StartSmokeTime != null && e.EndSmokeTime != null);

                if (lastSmoke != null)
                {
                    currentSmokeId = lastSmoke.Id;
                    currentSmokeTime = time.Subtract(DateTime.Parse(lastSmoke.StartSmokeTime.ToString()));
                }
            }

            // Calculate test time left
            TimeSpan? testTimeLeft = null;
            var testEndDate = test.TestEndDate;

            testTimeLeft = testEndDate.Subtract(time);

            var totalSmoked = test.SmokedCigaresUnderTest.Count;

            return new CurrentTestDataCalculationDTO(totalSmoked, timeSinceLastSmoke, testTimeLeft, currentSmokeId, currentSmokeTime);

        }
    }
}
