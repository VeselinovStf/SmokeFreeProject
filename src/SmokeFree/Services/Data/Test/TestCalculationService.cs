using SmokeFree.Abstraction.Services.Data.Test;
using SmokeFree.Data.Models;
using SmokeFree.Models.Services.Data.Test;
using System;
using System.Collections.Generic;
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
            // Default Values
            var currentSmokeId = string.Empty;
            TimeSpan currentSmokeTime = new TimeSpan(0, 0, 0);       
            TimeSpan timeSinceLastSmoke = new TimeSpan(0, 0, 0);     
            TimeSpan testTimeLeft = new TimeSpan(0, 0, 0);
            var totalSmoked = 0;

            var smokedCigaresUnderTest = new List<Smoke>();

            // Value to check is Offset is valid
            var defaultTimeOffset = new DateTimeOffset();

            // Calculate test time left
            if (!test.TestEndDate.Equals(defaultTimeOffset))
            {
                testTimeLeft = test.TestEndDate.DateTime.Subtract(time);
            }

            if (test.SmokedCigaresUnderTest != null)
            {
                // Get Only Valid Smokes
                smokedCigaresUnderTest = test.SmokedCigaresUnderTest
                    .Where(e => !e.IsDeleted)
                    .ToList();

                // If got enaugth smokes for calculations
                if (smokedCigaresUnderTest.Count > 0)
                {
                    // Calculate total smokes
                    // Validate App Smokes State
                    var leftUnfinished = smokedCigaresUnderTest
                        .Where(e => !e.StartSmokeTime.Equals(defaultTimeOffset) && e.EndSmokeTime.Equals(defaultTimeOffset))
                        .ToList();

                    if (leftUnfinished.Count > 1)
                    {
                        // App State Exception
                        // More then one Started but not finished breaks app state
                        throw new Exception($"[APP STATE EXCEPTION] : More then one Started SMOKE but not finished detected! In Test : {test.Id}");
                    }
                    else if(leftUnfinished.Count == 0)
                    {
                        // One is left
                        // Calculate total smokes
                        totalSmoked = smokedCigaresUnderTest.Count;
                    }

                    // Calculate Time Since Last Smoke
                    var lastSmokeTime = smokedCigaresUnderTest
                        .OrderByDescending(e => e.EndSmokeTime)
                        .FirstOrDefault()
                        .EndSmokeTime;

                    if (!lastSmokeTime.Equals(defaultTimeOffset))
                    {
                        timeSinceLastSmoke = time.Subtract(lastSmokeTime.DateTime);
                    }

                    // Calculate last smoked time
                    var lastSmoke = leftUnfinished
                        .OrderByDescending(e => e.StartSmokeTime)
                        .FirstOrDefault();

                    if (lastSmoke != null)
                    {
                        currentSmokeId = lastSmoke.Id;
                        currentSmokeTime = time.Subtract(lastSmoke.StartSmokeTime.DateTime);
                    }
                }
            }

            // Return DTO Model 
            return new CurrentTestDataCalculationDTO(totalSmoked, timeSinceLastSmoke, testTimeLeft, currentSmokeId, currentSmokeTime);
        }
    }
}
