using NUnit.Framework;
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
    }
}
