﻿using NUnit.Framework;
using SmokeFree.Data.Models;
using SmokeFree.Services.Data.Test;
using System;

namespace TestCalculationServiceTests.UnitTests
{
    /// <summary>
    /// TestCalculationService - CalculateTestResult Tests
    /// </summary>
    public class CalculateTestResult_Should
    {
        /// <summary>
        /// Return false success, when passed argumeter is invalid
        /// </summary>
        [Test]
        public void Return_False_Success_When_Passed_Arg_Is_Invalid()
        {
            // Arrange
            var testCalculationService = new TestCalculationService();

            // Act
            var resultModel = testCalculationService.CalculateTestResult(null);

            // Assert
            Assert.False(resultModel.Success);
        }

        /// <summary>
        /// Return false success, when test smokes are less then 1
        /// </summary>
        [Test]
        public void Return_False_Success_When_Passed_Test_Smokes_Are_Zero()
        {
            // Arrange
            var testCalculationService = new TestCalculationService();
            var test = new Test();

            // Act
            var resultModel = testCalculationService.CalculateTestResult(test);

            // Assert
            Assert.False(resultModel.Success);
        }

        /// <summary>
        /// Return success true when test is valid
        /// </summary>
        [Test]
        public void Return_Success_When_Test_Is_Valid()
        {
            // Arrange
            var testCalculationService = new TestCalculationService();

            var dateTime = DateTime.Now;
            var testId = "Test_ID";
            var testStartTime = dateTime;
            var endTestTime = dateTime.AddDays(1);

            var test = new Test()
            {
                Id = testId,
                TestStartDate = testStartTime,
                TestEndDate = endTestTime
            };

            var smoke = new Smoke()
            {
                StartSmokeTime = dateTime.AddMinutes(5),
                EndSmokeTime = dateTime.AddMinutes(10)
            };

            test.SmokedCigaresUnderTest.Add(smoke);

            // Act
            var resultModel = testCalculationService.CalculateTestResult(test);

            // Assert
            Assert.True(resultModel.Success);
        }

        /// <summary>
        /// Calculate Five Smokes for one day Correctly.
        /// Five smokes for one day, 7min for smoke, 60 mins distance between last and next smoke
        /// </summary>
        [Test]
        public void Calculate_One_Day_Five_Smokes_Seven_Mins_Each_60Mins_Distance_TestResult_Correctly()
        {
            // Arrange
            var testCalculationService = new TestCalculationService();

            var dateTime = DateTime.Now;
            var testId = "Test_ID";
            var testStartTime = dateTime;

            var testDays = 1;
            var smokesDistanceInMinutes = 60;
            var oneSmokeTimeInMinutes = 7;

            var testSmokes = 5;
            var totalSmokeTime = oneSmokeTimeInMinutes * testSmokes;

            var testTimeElapsed = dateTime;

            var smoke1start = dateTime;
            var smoke1end = smoke1start.AddMinutes(oneSmokeTimeInMinutes);

            testTimeElapsed = testTimeElapsed.AddMinutes(oneSmokeTimeInMinutes);

            var smoke2start = smoke1end.AddMinutes(smokesDistanceInMinutes);
            var smoke2end = smoke2start.AddMinutes(oneSmokeTimeInMinutes);

            testTimeElapsed = testTimeElapsed.AddMinutes(smokesDistanceInMinutes);
            testTimeElapsed = testTimeElapsed.AddMinutes(oneSmokeTimeInMinutes);

            var smoke3start = smoke2end.AddMinutes(smokesDistanceInMinutes);
            var smoke3end = smoke3start.AddMinutes(oneSmokeTimeInMinutes);

            testTimeElapsed = testTimeElapsed.AddMinutes(smokesDistanceInMinutes);
            testTimeElapsed = testTimeElapsed.AddMinutes(oneSmokeTimeInMinutes);

            var smoke4start = smoke3end.AddMinutes(smokesDistanceInMinutes);
            var smoke4end = smoke4start.AddMinutes(oneSmokeTimeInMinutes);

            testTimeElapsed = testTimeElapsed.AddMinutes(smokesDistanceInMinutes);
            testTimeElapsed = testTimeElapsed.AddMinutes(oneSmokeTimeInMinutes);

            var smoke5start = smoke4end.AddMinutes(smokesDistanceInMinutes);
            var smoke5end = smoke5start.AddMinutes(oneSmokeTimeInMinutes);

            testTimeElapsed = testTimeElapsed.AddMinutes(smokesDistanceInMinutes);
            testTimeElapsed = testTimeElapsed.AddMinutes(oneSmokeTimeInMinutes);

            var testEndTime = ((testSmokes - 1) * smokesDistanceInMinutes) + totalSmokeTime;
            var endTestTime = dateTime.AddMinutes(testEndTime);

            var smoke1 = new Smoke()
            {
                StartSmokeTime = smoke1start,
                EndSmokeTime = smoke1end
            };
            var smoke2 = new Smoke()
            {
                StartSmokeTime = smoke2start,
                EndSmokeTime = smoke2end
            };
            var smoke3 = new Smoke()
            {
                StartSmokeTime = smoke3start,
                EndSmokeTime = smoke3end
            };
            var smoke4 = new Smoke()
            {
                StartSmokeTime = smoke4start,
                EndSmokeTime = smoke4end
            };
            var smoke5 = new Smoke()
            {
                StartSmokeTime = smoke5start,
                EndSmokeTime = smoke5end
            };

            var test = new Test()
            {
                Id = testId,
                TestStartDate = testStartTime,
                TestEndDate = endTestTime
            };

            test.SmokedCigaresUnderTest.Add(smoke1);
            test.SmokedCigaresUnderTest.Add(smoke2);
            test.SmokedCigaresUnderTest.Add(smoke3);
            test.SmokedCigaresUnderTest.Add(smoke4);
            test.SmokedCigaresUnderTest.Add(smoke5);


            var totalSmokes = test.SmokedCigaresUnderTest.Count;
            var avarageSmokesPerDay = totalSmokes / testDays;
            var avarageCleanTimeSeconds = (((totalSmokes - 1) * smokesDistanceInMinutes) * smokesDistanceInMinutes) / testDays;
            var totalSmokeGasTimeSeconds = (totalSmokes * oneSmokeTimeInMinutes) * 60;
            var avarageSmokeDistanceSeconds = ((smokesDistanceInMinutes + oneSmokeTimeInMinutes) * 60) / testDays;
            var totalTestTime = testTimeElapsed.Subtract(dateTime);
            var totalTestTimeOffset = totalTestTime.TotalSeconds;

            var expectedTestResult = new TestResult()
            {
                TotalTestTimeSeconds = totalTestTimeOffset,
                TestId = testId,
                TestStartDate = test.TestStartDate,
                EndStartDate = endTestTime,
                TotalSmokedCigars = totalSmokes,
                AvarageSmokedCigarsPerDay = avarageSmokesPerDay,
                AvarageCleanOxygenTimeSeconds = avarageCleanTimeSeconds,
                TotalSmokeGasTimeTimeSeconds = totalSmokeGasTimeSeconds,
                AvarageSmokeDistanceSeconds = avarageSmokeDistanceSeconds,
                AvarageSmokeActiveTimeSeconds = avarageCleanTimeSeconds + totalSmokeGasTimeSeconds
            };
            // Act
            var actual = testCalculationService.CalculateTestResult(test);

            // Assert
            Assert.IsTrue(expectedTestResult.TotalTestTimeSeconds == actual.TestResultCalculation.TotalTestTimeSeconds, "TotalTestTime not valid");
            Assert.IsTrue(expectedTestResult.TestId == actual.TestResultCalculation.TestId, "TestId not valid");
            Assert.IsTrue(expectedTestResult.TestStartDate.DateTime == actual.TestResultCalculation.TestStartDate, "TestStartDate not valid");
            Assert.IsTrue(expectedTestResult.EndStartDate.DateTime == actual.TestResultCalculation.EndStartDate, "EndStartDate not valid");
            Assert.IsTrue(expectedTestResult.TotalSmokedCigars == actual.TestResultCalculation.TotalSmokedCigars, "TotalSmokedCigars not valid");
            Assert.IsTrue(expectedTestResult.AvarageSmokedCigarsPerDay == actual.TestResultCalculation.AvarageSmokedCigarsPerDay, "AvarageSmokedCigarsPerDay not valid");
            Assert.IsTrue(expectedTestResult.AvarageCleanOxygenTimeSeconds == actual.TestResultCalculation.AvarageCleanOxygenTimeSeconds, "AvarageCleanOxygenTimeSeconds not valid");
            Assert.IsTrue(expectedTestResult.TotalSmokeGasTimeTimeSeconds == actual.TestResultCalculation.TotalSmokeGasTimeTimeSeconds, "TotalSmokeGasTimeTimeSeconds not valid");
            Assert.IsTrue(expectedTestResult.AvarageSmokeDistanceSeconds == actual.TestResultCalculation.AvarageSmokeDistanceSeconds, "AvarageSmokeDistanceSeconds not valid");
            Assert.IsTrue(expectedTestResult.AvarageSmokeActiveTimeSeconds == actual.TestResultCalculation.AvarageSmokeActiveTimeSeconds, "AvarageSmokeActiveTimeSeconds not valid");
        }

        /// <summary>
        /// Calculate Five Smokes for one day Correctly.
        /// Five smokes for one day, 7min for smoke, 40 mins distance between last and next smoke
        /// </summary>
        [Test]
        public void Calculate_One_Day_Five_Smokes_Seven_Mins_Each_40Mins_Distance_TestResult_Correctly()
        {
            // Arrange
            var testCalculationService = new TestCalculationService();

            var dateTime = DateTime.Now;
            var testId = "Test_ID";
            var testStartTime = dateTime;

            var testDays = 1;
            var smokesDistanceInMinutes = 40;
            var oneSmokeTimeInMinutes = 7;

            var testSmokes = 5;
            var totalSmokeTime = oneSmokeTimeInMinutes * testSmokes;

            var testTimeElapsed = dateTime;

            var smoke1start = dateTime;
            var smoke1end = smoke1start.AddMinutes(oneSmokeTimeInMinutes);

            testTimeElapsed = testTimeElapsed.AddMinutes(oneSmokeTimeInMinutes);

            var smoke2start = smoke1end.AddMinutes(smokesDistanceInMinutes);
            var smoke2end = smoke2start.AddMinutes(oneSmokeTimeInMinutes);

            testTimeElapsed = testTimeElapsed.AddMinutes(smokesDistanceInMinutes);
            testTimeElapsed = testTimeElapsed.AddMinutes(oneSmokeTimeInMinutes);

            var smoke3start = smoke2end.AddMinutes(smokesDistanceInMinutes);
            var smoke3end = smoke3start.AddMinutes(oneSmokeTimeInMinutes);

            testTimeElapsed = testTimeElapsed.AddMinutes(smokesDistanceInMinutes);
            testTimeElapsed = testTimeElapsed.AddMinutes(oneSmokeTimeInMinutes);

            var smoke4start = smoke3end.AddMinutes(smokesDistanceInMinutes);
            var smoke4end = smoke4start.AddMinutes(oneSmokeTimeInMinutes);

            testTimeElapsed = testTimeElapsed.AddMinutes(smokesDistanceInMinutes);
            testTimeElapsed = testTimeElapsed.AddMinutes(oneSmokeTimeInMinutes);

            var smoke5start = smoke4end.AddMinutes(smokesDistanceInMinutes);
            var smoke5end = smoke5start.AddMinutes(oneSmokeTimeInMinutes);

            testTimeElapsed = testTimeElapsed.AddMinutes(smokesDistanceInMinutes);
            testTimeElapsed = testTimeElapsed.AddMinutes(oneSmokeTimeInMinutes);

            var testEndTime = ((testSmokes - 1) * smokesDistanceInMinutes) + totalSmokeTime;
            var endTestTime = dateTime.AddMinutes(testEndTime);

            var smoke1 = new Smoke()
            {
                StartSmokeTime = smoke1start,
                EndSmokeTime = smoke1end
            };
            var smoke2 = new Smoke()
            {
                StartSmokeTime = smoke2start,
                EndSmokeTime = smoke2end
            };
            var smoke3 = new Smoke()
            {
                StartSmokeTime = smoke3start,
                EndSmokeTime = smoke3end
            };
            var smoke4 = new Smoke()
            {
                StartSmokeTime = smoke4start,
                EndSmokeTime = smoke4end
            };
            var smoke5 = new Smoke()
            {
                StartSmokeTime = smoke5start,
                EndSmokeTime = smoke5end
            };

            var test = new Test()
            {
                Id = testId,
                TestStartDate = testStartTime,
                TestEndDate = endTestTime
            };

            test.SmokedCigaresUnderTest.Add(smoke1);
            test.SmokedCigaresUnderTest.Add(smoke2);
            test.SmokedCigaresUnderTest.Add(smoke3);
            test.SmokedCigaresUnderTest.Add(smoke4);
            test.SmokedCigaresUnderTest.Add(smoke5);


            var totalSmokes = test.SmokedCigaresUnderTest.Count;
            var avarageSmokesPerDay = totalSmokes / testDays;
            var avarageCleanTimeSeconds = (((totalSmokes - 1) * smokesDistanceInMinutes) * 60) / testDays;
            var totalSmokeGasTimeSeconds = (totalSmokes * oneSmokeTimeInMinutes) * 60;
            var avarageSmokeDistanceSeconds = ((smokesDistanceInMinutes + oneSmokeTimeInMinutes) * 60) / testDays;
            var totalTestTime = testTimeElapsed.Subtract(dateTime);
            var totalTestTimeOffset = totalTestTime.TotalSeconds;

            var expectedTestResult = new TestResult()
            {
                TotalTestTimeSeconds = totalTestTimeOffset,
                TestId = testId,
                TestStartDate = test.TestStartDate,
                EndStartDate = endTestTime,
                TotalSmokedCigars = totalSmokes,
                AvarageSmokedCigarsPerDay = avarageSmokesPerDay,
                AvarageCleanOxygenTimeSeconds = avarageCleanTimeSeconds,
                TotalSmokeGasTimeTimeSeconds = totalSmokeGasTimeSeconds,
                AvarageSmokeDistanceSeconds = avarageSmokeDistanceSeconds,
                AvarageSmokeActiveTimeSeconds = avarageCleanTimeSeconds + totalSmokeGasTimeSeconds
            };
            // Act
            var actual = testCalculationService.CalculateTestResult(test);

            // Assert
            Assert.IsTrue(expectedTestResult.TotalTestTimeSeconds == actual.TestResultCalculation.TotalTestTimeSeconds, "TotalTestTime not valid");
            Assert.IsTrue(expectedTestResult.TestId == actual.TestResultCalculation.TestId, "TestId not valid");
            Assert.IsTrue(expectedTestResult.TestStartDate.DateTime == actual.TestResultCalculation.TestStartDate, "TestStartDate not valid");
            Assert.IsTrue(expectedTestResult.EndStartDate.DateTime == actual.TestResultCalculation.EndStartDate, "EndStartDate not valid");
            Assert.IsTrue(expectedTestResult.TotalSmokedCigars == actual.TestResultCalculation.TotalSmokedCigars, "TotalSmokedCigars not valid");
            Assert.IsTrue(expectedTestResult.AvarageSmokedCigarsPerDay == actual.TestResultCalculation.AvarageSmokedCigarsPerDay, "AvarageSmokedCigarsPerDay not valid");
            Assert.IsTrue(expectedTestResult.AvarageCleanOxygenTimeSeconds == actual.TestResultCalculation.AvarageCleanOxygenTimeSeconds, "AvarageCleanOxygenTimeSeconds not valid");
            Assert.IsTrue(expectedTestResult.TotalSmokeGasTimeTimeSeconds == actual.TestResultCalculation.TotalSmokeGasTimeTimeSeconds, "TotalSmokeGasTimeTimeSeconds not valid");
            Assert.IsTrue(expectedTestResult.AvarageSmokeDistanceSeconds == actual.TestResultCalculation.AvarageSmokeDistanceSeconds, "AvarageSmokeDistanceSeconds not valid");
            Assert.IsTrue(expectedTestResult.AvarageSmokeActiveTimeSeconds == actual.TestResultCalculation.AvarageSmokeActiveTimeSeconds, "AvarageSmokeActiveTimeSeconds not valid");
        }

        /// <summary>
        /// Calculate Five Smokes for one day Correctly.
        /// Five smokes for one day, 6min for smoke, 36 mins distance between last and next smoke
        /// </summary>
        [Test]
        public void Calculate_One_Day_Five_Smokes_Six_Mins_Each_36Mins_Distance_TestResult_Correctly()
        {
            // Arrange
            var testCalculationService = new TestCalculationService();

            var dateTime = DateTime.Now;
            var testId = "Test_ID";
            var testStartTime = dateTime;

            var testDays = 1;
            var smokesDistanceInMinutes = 36;
            var oneSmokeTimeInMinutes = 6;

            var testSmokes = 5;
            var totalSmokeTime = oneSmokeTimeInMinutes * testSmokes;

            var testTimeElapsed = dateTime;

            var smoke1start = dateTime;
            var smoke1end = smoke1start.AddMinutes(oneSmokeTimeInMinutes);

            testTimeElapsed = testTimeElapsed.AddMinutes(oneSmokeTimeInMinutes);

            var smoke2start = smoke1end.AddMinutes(smokesDistanceInMinutes);
            var smoke2end = smoke2start.AddMinutes(oneSmokeTimeInMinutes);

            testTimeElapsed = testTimeElapsed.AddMinutes(smokesDistanceInMinutes);
            testTimeElapsed = testTimeElapsed.AddMinutes(oneSmokeTimeInMinutes);

            var smoke3start = smoke2end.AddMinutes(smokesDistanceInMinutes);
            var smoke3end = smoke3start.AddMinutes(oneSmokeTimeInMinutes);

            testTimeElapsed = testTimeElapsed.AddMinutes(smokesDistanceInMinutes);
            testTimeElapsed = testTimeElapsed.AddMinutes(oneSmokeTimeInMinutes);

            var smoke4start = smoke3end.AddMinutes(smokesDistanceInMinutes);
            var smoke4end = smoke4start.AddMinutes(oneSmokeTimeInMinutes);

            testTimeElapsed = testTimeElapsed.AddMinutes(smokesDistanceInMinutes);
            testTimeElapsed = testTimeElapsed.AddMinutes(oneSmokeTimeInMinutes);

            var smoke5start = smoke4end.AddMinutes(smokesDistanceInMinutes);
            var smoke5end = smoke5start.AddMinutes(oneSmokeTimeInMinutes);

            testTimeElapsed = testTimeElapsed.AddMinutes(smokesDistanceInMinutes);
            testTimeElapsed = testTimeElapsed.AddMinutes(oneSmokeTimeInMinutes);

            var testEndTime = ((testSmokes - 1) * smokesDistanceInMinutes) + totalSmokeTime;
            var endTestTime = dateTime.AddMinutes(testEndTime);

            var smoke1 = new Smoke()
            {
                StartSmokeTime = smoke1start,
                EndSmokeTime = smoke1end
            };
            var smoke2 = new Smoke()
            {
                StartSmokeTime = smoke2start,
                EndSmokeTime = smoke2end
            };
            var smoke3 = new Smoke()
            {
                StartSmokeTime = smoke3start,
                EndSmokeTime = smoke3end
            };
            var smoke4 = new Smoke()
            {
                StartSmokeTime = smoke4start,
                EndSmokeTime = smoke4end
            };
            var smoke5 = new Smoke()
            {
                StartSmokeTime = smoke5start,
                EndSmokeTime = smoke5end
            };

            var test = new Test()
            {
                Id = testId,
                TestStartDate = testStartTime,
                TestEndDate = endTestTime
            };

            test.SmokedCigaresUnderTest.Add(smoke1);
            test.SmokedCigaresUnderTest.Add(smoke2);
            test.SmokedCigaresUnderTest.Add(smoke3);
            test.SmokedCigaresUnderTest.Add(smoke4);
            test.SmokedCigaresUnderTest.Add(smoke5);


            var totalSmokes = test.SmokedCigaresUnderTest.Count;
            var avarageSmokesPerDay = totalSmokes / testDays;
            var avarageCleanTimeSeconds = (((totalSmokes - 1) * smokesDistanceInMinutes) * 60) / testDays;
            var totalSmokeGasTimeSeconds = (totalSmokes * oneSmokeTimeInMinutes) * 60;
            var avarageSmokeDistanceSeconds = ((smokesDistanceInMinutes + oneSmokeTimeInMinutes) * 60) / testDays;
            var totalTestTime = testTimeElapsed.Subtract(dateTime);
            var totalTestTimeOffset = totalTestTime.TotalSeconds;

            var expectedTestResult = new TestResult()
            {
                TotalTestTimeSeconds = totalTestTimeOffset,
                TestId = testId,
                TestStartDate = test.TestStartDate,
                EndStartDate = endTestTime,
                TotalSmokedCigars = totalSmokes,
                AvarageSmokedCigarsPerDay = avarageSmokesPerDay,
                AvarageCleanOxygenTimeSeconds = avarageCleanTimeSeconds,
                TotalSmokeGasTimeTimeSeconds = totalSmokeGasTimeSeconds,
                AvarageSmokeDistanceSeconds = avarageSmokeDistanceSeconds,
                AvarageSmokeActiveTimeSeconds = avarageCleanTimeSeconds + totalSmokeGasTimeSeconds
            };
            // Act
            var actual = testCalculationService.CalculateTestResult(test);

            // Assert
            Assert.IsTrue(expectedTestResult.TotalTestTimeSeconds == actual.TestResultCalculation.TotalTestTimeSeconds, "TotalTestTime not valid");
            Assert.IsTrue(expectedTestResult.TestId == actual.TestResultCalculation.TestId, "TestId not valid");
            Assert.IsTrue(expectedTestResult.TestStartDate.DateTime == actual.TestResultCalculation.TestStartDate, "TestStartDate not valid");
            Assert.IsTrue(expectedTestResult.EndStartDate.DateTime == actual.TestResultCalculation.EndStartDate, "EndStartDate not valid");
            Assert.IsTrue(expectedTestResult.TotalSmokedCigars == actual.TestResultCalculation.TotalSmokedCigars, "TotalSmokedCigars not valid");
            Assert.IsTrue(expectedTestResult.AvarageSmokedCigarsPerDay == actual.TestResultCalculation.AvarageSmokedCigarsPerDay, "AvarageSmokedCigarsPerDay not valid");
            Assert.IsTrue(expectedTestResult.AvarageCleanOxygenTimeSeconds == actual.TestResultCalculation.AvarageCleanOxygenTimeSeconds, "AvarageCleanOxygenTimeSeconds not valid");
            Assert.IsTrue(expectedTestResult.TotalSmokeGasTimeTimeSeconds == actual.TestResultCalculation.TotalSmokeGasTimeTimeSeconds, "TotalSmokeGasTimeTimeSeconds not valid");
            Assert.IsTrue(expectedTestResult.AvarageSmokeDistanceSeconds == actual.TestResultCalculation.AvarageSmokeDistanceSeconds, "AvarageSmokeDistanceSeconds not valid");
            Assert.IsTrue(expectedTestResult.AvarageSmokeActiveTimeSeconds == actual.TestResultCalculation.AvarageSmokeActiveTimeSeconds, "AvarageSmokeActiveTimeSeconds not valid");
        }

        /// <summary>
        /// Calculate 2 Smokes for 30 sec Correctly.
        /// Two smokes for 30 sec test
        /// </summary>
        [Test]
        public void Calculate_30Sec_Two_Smokes_TestResult_Correctly()
        {
            // Arrange
            var testCalculationService = new TestCalculationService();

            var dateTime = DateTime.Now;
            var testId = "Test_ID";
            var testStartTime = dateTime;

            var testTimeElapsed = dateTime;

            // First Starts NOW
            var smoke1start = dateTime;
            var smoke1end = smoke1start.AddSeconds(6);
            // First smoked for 6s
            // 10sec free oxygen
            // Second starts + 10 sec
            var smoke2start = smoke1end.AddSeconds(10);
            var smoke2end = smoke2start.AddSeconds(10);
            // 4sec free oxygen
            var endTestTIme = smoke2end.AddSeconds(4);

            var smoke1 = new Smoke()
            {
                StartSmokeTime = smoke1start,
                EndSmokeTime = smoke1end
            };
            var smoke2 = new Smoke()
            {
                StartSmokeTime = smoke2start,
                EndSmokeTime = smoke2end
            };


            var test = new Test()
            {
                Id = testId,
                TestStartDate = testStartTime,
                TestEndDate = endTestTIme
            };

            test.SmokedCigaresUnderTest.Add(smoke1);
            test.SmokedCigaresUnderTest.Add(smoke2);

            var totalSmokes = test.SmokedCigaresUnderTest.Count;
            var avarageSmokesPerDay = totalSmokes / 1;
            var avarageCleanTimeSeconds = 14 / 1;
            var totalSmokeGasTimeSeconds = 16;
            var avarageSmokeDistanceSeconds = 10 / 1;
            var totalTestTime = endTestTIme.Subtract(testTimeElapsed);
            var totalTestTimeOffset = totalTestTime.TotalSeconds;

            var expectedTestResult = new TestResult()
            {
                TotalTestTimeSeconds = totalTestTimeOffset,
                TestId = testId,
                TestStartDate = test.TestStartDate,
                EndStartDate = endTestTIme,
                TotalSmokedCigars = totalSmokes,
                AvarageSmokedCigarsPerDay = avarageSmokesPerDay,
                AvarageCleanOxygenTimeSeconds = avarageCleanTimeSeconds,
                TotalSmokeGasTimeTimeSeconds = totalSmokeGasTimeSeconds,
                AvarageSmokeDistanceSeconds = avarageSmokeDistanceSeconds,
                AvarageSmokeActiveTimeSeconds = avarageCleanTimeSeconds + totalSmokeGasTimeSeconds
            };
            // Act
            var actual = testCalculationService.CalculateTestResult(test);

            // Assert
            Assert.IsTrue(expectedTestResult.TotalTestTimeSeconds == actual.TestResultCalculation.TotalTestTimeSeconds, "TotalTestTime not valid");
            Assert.IsTrue(expectedTestResult.TestId == actual.TestResultCalculation.TestId, "TestId not valid");
            Assert.IsTrue(expectedTestResult.TestStartDate.DateTime == actual.TestResultCalculation.TestStartDate, "TestStartDate not valid");
            Assert.IsTrue(expectedTestResult.EndStartDate.DateTime == actual.TestResultCalculation.EndStartDate, "EndStartDate not valid");
            Assert.IsTrue(expectedTestResult.TotalSmokedCigars == actual.TestResultCalculation.TotalSmokedCigars, "TotalSmokedCigars not valid");
            Assert.IsTrue(expectedTestResult.AvarageSmokedCigarsPerDay == actual.TestResultCalculation.AvarageSmokedCigarsPerDay, "AvarageSmokedCigarsPerDay not valid");
            Assert.IsTrue(expectedTestResult.AvarageCleanOxygenTimeSeconds == actual.TestResultCalculation.AvarageCleanOxygenTimeSeconds, "AvarageCleanOxygenTimeSeconds not valid");
            Assert.IsTrue(expectedTestResult.TotalSmokeGasTimeTimeSeconds == actual.TestResultCalculation.TotalSmokeGasTimeTimeSeconds, "TotalSmokeGasTimeTimeSeconds not valid");
            Assert.IsTrue(expectedTestResult.AvarageSmokeDistanceSeconds == actual.TestResultCalculation.AvarageSmokeDistanceSeconds, "AvarageSmokeDistanceSeconds not valid");
            Assert.IsTrue(expectedTestResult.AvarageSmokeActiveTimeSeconds == actual.TestResultCalculation.AvarageSmokeActiveTimeSeconds, "AvarageSmokeActiveTimeSeconds not valid");
        }
    }
}
