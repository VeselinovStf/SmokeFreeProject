﻿using SmokeFree.Data.Models;
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

        /// <summary>
        /// Calculate Time Sence last smoke
        /// </summary>
        /// <param name="currentTest">Test to calculate from</param>
        /// <param name="now">Current time</param>
        /// <returns>TimeSpan time sence last smoke</returns>
        TimeSpan TimeSinceLastSmoke(SmokeFree.Data.Models.Test currentTest, DateTime now);

        /// <summary>
        /// Calculate User Status From Test Results
        /// </summary>
        /// <param name="testResult">User Test Results</param>
        /// <returns>Response Model</returns>
        CalculateUserStatusDTO CalculateUserSmokeStatus(TestResult testResult);

        /// <summary>
        /// Calculate UserSmokeStatus by input cigares count
        /// </summary>
        /// <param name="smokedCigares">Count Of Smoked Cigares</param>
        /// <returns></returns>
        UserSmokeStatuses CalculateUserSmokeStatusBySmokes(int smokedCigares);
    }
}
