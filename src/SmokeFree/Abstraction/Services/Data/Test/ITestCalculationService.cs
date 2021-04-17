using SmokeFree.Models.Services.Data.Test;
using System;

namespace SmokeFree.Abstraction.Services.Data.Test
{
    /// <summary>
    /// Test Calculation Service Abstraction
    /// </summary>
    public interface ITestCalculationService
    {
        /// <summary>
        /// Get Test Data Based on Time
        /// </summary>
        /// <param name="time">DateTime Filter</param>
        /// <param name="test">Test to do Calculations From</param>
        /// <returns>Result data DTO</returns>
        CurrentTestDataCalculationDTO GetCurrentTestDataCalculation(DateTime time, SmokeFree.Data.Models.Test test);

        /// <summary>
        /// Calculate Test Results
        /// </summary>
        /// <param name="test">Test to calculate results from</param>
        /// <returns>Result data DTO</returns>
        CalculateTestResultDTO CalculateTestResult(SmokeFree.Data.Models.Test test);

    }
}
