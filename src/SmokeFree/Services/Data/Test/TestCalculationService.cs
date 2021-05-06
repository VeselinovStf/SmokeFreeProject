using SmokeFree.Abstraction.Services.Data.Test;
using SmokeFree.Data.Models;
using SmokeFree.Models.Services.Data.Test;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmokeFree.Services.Data.Test
{
    public class DayCalculation
    {
        public int DaySmokedCount { get; set; }

        public TimeSpan DayAvarageSmokingTimeSpan { get; set; }
        public TimeSpan DayAvarageNotSmokingTimeSpan { get; set; }
        public TimeSpan DayAvarageSmokeDistance { get; set; }

        public TimeSpan TotalSmokeTime { get; set; }

        public double SmokeActiveTimeSeconds { get; set; }
        public TimeSpan DayCleanOxygen { get; internal set; }
    }

    /// <summary>
    /// Service concern in Test Model Calculations
    /// </summary>
    public class TestCalculationService : ITestCalculationService
    {
        /// <summary>
        /// Calculate Test Results
        /// </summary>
        /// <param name="test">Test to calculate results from</param>
        /// <returns>Result data DTO</returns>
        public CalculateTestResultDTO CalculateTestResult(SmokeFree.Data.Models.Test test)
        {
            try
            {
                // Get Test Smokes
                var testSmokes = test.SmokedCigaresUnderTest;
                var dayResults = new List<DayCalculation>();

                // Validate App State
                if (testSmokes.Count == 0)
                {
                    return new CalculateTestResultDTO(false, $"Can't calculate test result without actual smokes! Test Id {test.Id}");
                }

                var newTestResults = new TestResult();
                var testEndDate = test.TestEndDate.LocalDateTime;
                var testStartDate = test.TestStartDate.LocalDateTime;

                newTestResults.TotalTestTimeSeconds = testEndDate.Subtract(testStartDate).TotalSeconds;
                newTestResults.TestId = test.Id;
                newTestResults.TestStartDate = test.TestStartDate.LocalDateTime;
                newTestResults.EndStartDate = test.TestEndDate.LocalDateTime;
                newTestResults.TotalSmokedCigars = testSmokes.Count();

                var dayBasedSmokes = testSmokes
                    .GroupBy(
                        smoke => smoke.StartSmokeTime.LocalDateTime.Day)
                    .OrderBy(e => e.Key)
                    .ThenBy(e => e.OrderBy(v => v.StartSmokeTime));

                foreach (var daySmoke in dayBasedSmokes)
                {
                    var daySmokedCount = daySmoke.Count();

                    var dayFirstLastSmokeTimeMinutes = daySmoke.Last().EndSmokeTime.LocalDateTime
                            .Subtract(daySmoke.First().StartSmokeTime.LocalDateTime).TotalSeconds;

                    var daySmokingTimeSpanAVG = CalculateSmokeTimeAVG(daySmoke.ToList());

                    var dayNotSmokingTimeSpanAVG = CalculateNotSmokeTimeAVG(daySmoke.ToList());

                    var daySmokeDistanceAVG = CalculateSmokeDistanceAVG(daySmoke.ToList());

                    var dayTotalSmokeTime = CalculateTotalSmokeTime(daySmoke.ToList());

                    var dayCleanOxygen = CalculateTotalCleanOxygenTime(daySmoke.ToList());

                    dayResults.Add(new DayCalculation()
                    {
                        DayAvarageSmokeDistance = daySmokeDistanceAVG,
                        DayAvarageNotSmokingTimeSpan = dayNotSmokingTimeSpanAVG,
                        DaySmokedCount = daySmokedCount,
                        DayAvarageSmokingTimeSpan = daySmokingTimeSpanAVG,
                        SmokeActiveTimeSeconds = dayFirstLastSmokeTimeMinutes,
                        TotalSmokeTime = dayTotalSmokeTime,
                        DayCleanOxygen = dayCleanOxygen
                    });
                }

                var avarageSmokedCigarsPerDay = dayResults.Select(e => e.DaySmokedCount);
                var avarageSmokeOxygen = dayResults.Select(e => e.DayAvarageSmokingTimeSpan.TotalSeconds);
                var avarageCleanOxygen = dayResults.Select(e => e.DayAvarageNotSmokingTimeSpan.TotalSeconds);
                var avarageSmokeDistance = dayResults.Select(e => e.DayAvarageSmokeDistance.TotalSeconds);
                var avarageSmokeActiveTimeSeconds = dayResults.Select(e => e.SmokeActiveTimeSeconds);
                var totalSmokeTime = dayResults.Select(e => e.TotalSmokeTime.TotalSeconds).Sum();
                var totalCleanOxygen = dayResults.Select(e => e.DayCleanOxygen.TotalSeconds).Sum();

                newTestResults.AvarageSmokedCigarsPerDay = avarageSmokedCigarsPerDay.Average();
                newTestResults.AvarageCleanOxygenTimeSeconds = avarageCleanOxygen.Average();
                newTestResults.TotalSmokeGasTimeTimeSeconds = totalSmokeTime;
                newTestResults.AvarageSmokeDistanceSeconds = avarageSmokeDistance.Average();
                newTestResults.AvarageSmokeActiveTimeSeconds = avarageSmokeActiveTimeSeconds.Average();
                newTestResults.AvarageSmokingTimeSeconds = avarageSmokeOxygen.Average();
                newTestResults.TotalCleanOxygenSeconds = totalCleanOxygen;

                return new CalculateTestResultDTO(true, newTestResults);
            }
            catch (Exception ex)
            {
                return new CalculateTestResultDTO(false, ex.Message);
            }
        }

        private static TimeSpan CalculateAvarageSmokeDistance(List<Smoke> smokes)
        {
            var timeSpan = new List<TimeSpan>();

            if (smokes.Count == 1)
            {
                return new TimeSpan();
            }

            var orderedSmokes = smokes.OrderBy(e => e.StartSmokeTime).ToList();

            for (int i = 0; i < orderedSmokes.Count - 1; i++)
            {

                var subs = orderedSmokes[i + 1].StartSmokeTime.LocalDateTime.Subtract(orderedSmokes[i].EndSmokeTime.LocalDateTime);

                timeSpan.Add(subs);
            }

            return new TimeSpan((long)timeSpan.Select(ts => ts.Ticks).Average());
        }

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
            var isSmoking = false;

            var smokedCigaresUnderTest = new List<Smoke>();

            // Value to check is Offset is valid
            var defaultTimeOffset = new DateTimeOffset();

            // Calculate test time left
            if (!test.TestEndDate.Equals(defaultTimeOffset))
            {
                testTimeLeft = test.TestEndDate.LocalDateTime.Subtract(time);
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
                    else if (leftUnfinished.Count == 0)
                    {
                        // One is left
                        // Calculate total smokes
                        totalSmoked = smokedCigaresUnderTest.Count;
                    }
                    else
                    {
                        isSmoking = true;

                        totalSmoked = smokedCigaresUnderTest.Count - 1;
                    }

                    // Calculate Time Since Last Smoke
                    var lastSmokeTime = smokedCigaresUnderTest
                        .OrderByDescending(e => e.EndSmokeTime)
                        .FirstOrDefault()
                        .EndSmokeTime;

                    if (!lastSmokeTime.Equals(defaultTimeOffset))
                    {
                        timeSinceLastSmoke = time.Subtract(lastSmokeTime.LocalDateTime);
                    }

                    // Calculate last smoked time
                    var lastSmoke = smokedCigaresUnderTest
                        .OrderByDescending(e => e.StartSmokeTime)
                        .FirstOrDefault();

                    if (lastSmoke != null)
                    {
                        currentSmokeId = lastSmoke.Id;
                        currentSmokeTime = time.Subtract(lastSmoke.StartSmokeTime.LocalDateTime);
                    }
                }
            }

            // Return DTO Model 
            return new CurrentTestDataCalculationDTO(totalSmoked, timeSinceLastSmoke, testTimeLeft, currentSmokeId, currentSmokeTime, isSmoking);
        }

        /// <summary>
        /// Calculate Time Sence last smoke
        /// </summary>
        /// <param name="currentTest">Test to calculate from</param>
        /// <param name="now">Current time</param>
        /// <returns>TimeSpan time sence last smoke</returns>
        public TimeSpan TimeSinceLastSmoke(SmokeFree.Data.Models.Test currentTest, DateTime now)
        {
            // Check Collection
            if (currentTest.SmokedCigaresUnderTest.Count > 0)
            {
                // Get last smoke
                var lastSmoke = currentTest
                    .SmokedCigaresUnderTest
                    .OrderByDescending(e => e.EndSmokeTime)
                    .FirstOrDefault();

                // Validate
                if (lastSmoke != null)
                {
                    // Last Smoke Time End smoke time
                    var lastSmokeTime = lastSmoke.EndSmokeTime;

                    // Validate
                    if (lastSmokeTime != new DateTimeOffset())
                    {
                        return now.Subtract(lastSmokeTime.LocalDateTime);
                    }
                }
            }

            // Default return value
            return new TimeSpan(0, 0, 0);
        }

        public CalculateUserStatusDTO CalculateUserSmokeStatus(TestResult testResult)
        {
            try
            {
                var avarageUserSmokedCigares = (int)testResult.AvarageSmokedCigarsPerDay;

                UserSmokeStatuses status = CalculateUserSmokeStatusBySmokes(avarageUserSmokedCigares);
               
                var userStatuses = Globals.UserSmokeStatusesSet;

                return new CalculateUserStatusDTO(true,status);
            }
            catch (Exception ex)
            {
                return new CalculateUserStatusDTO(false, ex.Message);
            }
        }

        public UserSmokeStatuses CalculateUserSmokeStatusBySmokes(int smokedCigares)
        {

            if (smokedCigares >= 1 && smokedCigares <= 5)
            {
                return UserSmokeStatuses.Quiter;
            }
            else if (smokedCigares <= 10)
            {
                return UserSmokeStatuses.Concern;
            }
            else if (smokedCigares <= 15)
            {
                return UserSmokeStatuses.Smoker;

            }
            else if (smokedCigares <= 20)
            {
                return UserSmokeStatuses.Bad;
            }
            else
            {
                return UserSmokeStatuses.Worst;
            }
        }

        private TimeSpan CalculateTotalCleanOxygenTime(List<Smoke> smokes)
        {
            var timeSpan = new TimeSpan(0, 0, 0);

            var orderedSmokes = smokes.OrderBy(e => e.StartSmokeTime).ToList();

            for (int i = 0; i < orderedSmokes.Count - 1; i++)
            {
                timeSpan += orderedSmokes[i + 1].StartSmokeTime.LocalDateTime.Subtract(orderedSmokes[i].EndSmokeTime.LocalDateTime);
            }

            return timeSpan;
        }

        private TimeSpan CalculateTotalSmokeTime(List<Smoke> smokes)
        {
            var timeSpan = new TimeSpan(0, 0, 0);

            foreach (var s in smokes)
            {
                timeSpan += s.EndSmokeTime.LocalDateTime.Subtract(s.StartSmokeTime.LocalDateTime);
            }

            return timeSpan;
        }

        private TimeSpan CalculateSmokeDistanceAVG(List<Smoke> smokes)
        {
            var timeSpan = new List<TimeSpan>();

            var orderedSmokes = smokes.OrderBy(e => e.StartSmokeTime.LocalDateTime)
                .Select(e => e.StartSmokeTime.LocalDateTime)
                .Reverse()
                .ToList();

            for (int i = 0; i < orderedSmokes.Count - 1; i++)
            {
                var subs = orderedSmokes[i].Subtract(orderedSmokes[i + 1]);

                timeSpan.Add(subs);
            }

            return new TimeSpan((long)timeSpan.Select(ts => ts.Ticks).Average());
        }

        private TimeSpan CalculateNotSmokeTimeAVG(List<Smoke> smokes)
        {
            var timeSpan = new List<TimeSpan>();

            var orderedSmokes = smokes.OrderBy(e => e.StartSmokeTime).ToList();

            for (int i = 0; i < orderedSmokes.Count - 1; i++)
            {

                var sup = orderedSmokes[i + 1].StartSmokeTime.LocalDateTime.Subtract(orderedSmokes[i].EndSmokeTime.LocalDateTime);
                timeSpan.Add(sup);
            }

            return new TimeSpan((long)timeSpan.Select(ts => ts.Ticks).Average());
        }

        private TimeSpan CalculateSmokeTimeAVG(List<Smoke> smokes)
        {
            var timeSpan = new List<TimeSpan>();

            foreach (var s in smokes)
            {
                var res = s.EndSmokeTime.LocalDateTime.Subtract(s.StartSmokeTime.LocalDateTime);
                timeSpan.Add(res);
            }

            return new TimeSpan((long)timeSpan.Select(ts => ts.Ticks).Average());
        }
    }
}
