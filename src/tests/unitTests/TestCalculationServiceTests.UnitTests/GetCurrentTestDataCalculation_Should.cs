using NUnit.Framework;
using SmokeFree.Data.Models;
using SmokeFree.Services.Data.Test;
using System;

namespace TestCalculationServiceTests.UnitTests
{
    /// <summary>
    /// TestCalculationService - GetCurrentTestDataCalculation Tests
    /// </summary>
    public class GetCurrentTestDataCalculation_Should
    {        
        /// <summary>
        /// Returns Correnct Result Model when test parrameter is new
        /// </summary>
        [Test]
        public void Returns_Correct_Result_Model_When_Passing_Empty_Test()
        {
            // Arrange
            var testCalculationService = new TestCalculationService();
            
            var dateTime = DateTime.Now;
            var currentlySmokedCount = 0;
            var timeSenceLastSmoke = new TimeSpan(0, 0, 0);
            var testLeftTime = new TimeSpan(0, 0, 0);
            var currentSmokeTime = new TimeSpan(0, 0, 0);
            var currentSmokeId = string.Empty;

            var test = new Test();

            // Act
            var serviceResult = testCalculationService
                .GetCurrentTestDataCalculation(dateTime, test);

            //Assert
            Assert.AreEqual(currentlySmokedCount, serviceResult.CurrentSmokedCount, "CurrentlySmokedCount not equal");
            Assert.AreEqual(currentSmokeId, serviceResult.CurrentSmokeId, "CurrentSmokeId not equal");
            Assert.IsTrue(serviceResult.TimeSinceLastSmoke.Equals(timeSenceLastSmoke), "TimeSenceLastSmoke not equal");
            Assert.IsTrue(serviceResult.TestTimeLeft.Equals(testLeftTime), "TestLeftTime not equal");
            Assert.IsTrue(serviceResult.CurrentSmokeTime.Equals(currentSmokeTime), "CurrentSmokeTime not equal");

        }

        /// <summary>
        /// Returns One smoked cigar but not finished
        /// </summary>
        [Test]
        public void One_Smoked_Cigar_But_Not_Finised()
        {
            // Arrange
            var testCalculationService = new TestCalculationService();

            var dateTime = DateTime.Now;
            var currentlySmokedCount = 0;
            var timeSenceLastSmoke = new TimeSpan(0, 0, 0);
            var testLeftTime = new TimeSpan(0, 0, 0);
            var currentSmokeTime = dateTime.Subtract(dateTime.AddDays(-1));
            var currentSmokeId = string.Empty;

            var test = new Test();
            var startSmokeTime = dateTime.AddDays(-1);

            var startedSmoke = new Smoke()
            {
                StartSmokeTime = startSmokeTime,
                Id = currentSmokeId
            };

            test.SmokedCigaresUnderTest.Add(startedSmoke);

            // Act
            var serviceResult = testCalculationService
                .GetCurrentTestDataCalculation(dateTime, test);

            //Assert
            Assert.AreEqual(currentlySmokedCount, serviceResult.CurrentSmokedCount, "CurrentlySmokedCount not equal");
            Assert.AreEqual(currentSmokeId, serviceResult.CurrentSmokeId, "CurrentSmokeId not equal");
            Assert.IsTrue(serviceResult.TimeSinceLastSmoke.Equals(timeSenceLastSmoke), "TimeSenceLastSmoke not equal");
            Assert.IsTrue(serviceResult.TestTimeLeft.Equals(testLeftTime), "TestLeftTime not equal");
            Assert.IsTrue(serviceResult.CurrentSmokeTime.Equals(currentSmokeTime), "CurrentSmokeTime not equal");

        }
    }
}