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

        [Test]
        public void Returns_Correct_Result_Model_When_Passing_Full_Data()
        {
            // Arrange
            var testCalculationService = new TestCalculationService();

            var dateTime = DateTime.Now;
            var testEndDay = dateTime.AddDays(3);
            
            var startSmokeTimeOne = dateTime.AddDays(-1);
            var startSmokeTimeTwo = dateTime.AddDays(-1).AddHours(1);
            var startSmokeTimeThree = dateTime.AddDays(-1).AddHours(2);
            
            var expectedCurrentlySmokedCount = 3;
            var expectedTimeSenceLastSmoke = dateTime.Subtract(startSmokeTimeThree.AddMinutes(7));
            var expectedTestLeftTime = testEndDay.Subtract(dateTime);
            var expectedCurrentSmokeTime = dateTime.Subtract(startSmokeTimeThree);

            var currentSmokeIdOne = Guid.NewGuid().ToString();
            var currentSmokeIdTwo = Guid.NewGuid().ToString();
            var currentSmokeIdTree = Guid.NewGuid().ToString();

            var test = new Test()
            {
                TestEndDate = testEndDay
            };
            
            var startedSmokeOne = new Smoke()
            {
                StartSmokeTime = startSmokeTimeOne,
                Id = currentSmokeIdOne,
                EndSmokeTime = startSmokeTimeOne.AddMinutes(7)

            };

            var startedSmokeTwo = new Smoke()
            {
                StartSmokeTime = startSmokeTimeTwo,
                Id = currentSmokeIdTwo
                ,
                EndSmokeTime = startSmokeTimeTwo.AddMinutes(7)
            };

            var startedSmokeTree = new Smoke()
            {
                StartSmokeTime = startSmokeTimeThree,
                Id = currentSmokeIdTree,
                EndSmokeTime = startSmokeTimeThree.AddMinutes(7)
            };

            test.SmokedCigaresUnderTest.Add(startedSmokeOne);
            test.SmokedCigaresUnderTest.Add(startedSmokeTwo);
            test.SmokedCigaresUnderTest.Add(startedSmokeTree);

            // Act
            var serviceResult = testCalculationService
                .GetCurrentTestDataCalculation(dateTime, test);

            //Assert
            Assert.AreEqual(expectedCurrentlySmokedCount, serviceResult.CurrentSmokedCount, "CurrentlySmokedCount not equal");
            Assert.AreEqual(currentSmokeIdTree, serviceResult.CurrentSmokeId, "CurrentSmokeId1 not equal");
            Assert.IsTrue(serviceResult.TimeSinceLastSmoke.Equals(expectedTimeSenceLastSmoke), "TimeSenceLastSmoke not equal");
            Assert.IsTrue(serviceResult.TestTimeLeft.Equals(expectedTestLeftTime), "TestLeftTime not equal");
            Assert.IsTrue(serviceResult.CurrentSmokeTime.Equals(expectedCurrentSmokeTime), "CurrentSmokeTime not equal");

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

        /// <summary>
        /// Returns Zero smoked cigar but not finished
        /// </summary>
        [Test]
        public void Returns_Zero_Smoked_Cigar()
        {
            // Arrange
            var testCalculationService = new TestCalculationService();

            var dateTime = DateTime.Now;
            var currentlySmokedCount = 0;

            var test = new Test();

            // Act
            var serviceResult = testCalculationService
                .GetCurrentTestDataCalculation(dateTime, test);

            //Assert
            Assert.AreEqual(currentlySmokedCount, serviceResult.CurrentSmokedCount, "CurrentlySmokedCount not equal");
        }

        /// <summary>
        /// Validate App Smokes State - If Left Unifinished Smokes Are more Then One
        /// </summary>
        [Test]
        public void Throws_Exception_When_Left_Unfinished_Smokes_Are_More_Then_One()
        {
            // Arrange
            var testCalculationService = new TestCalculationService();

            var dateTime = DateTime.Now;
            var currentlySmokedCount = 0;
            var timeSenceLastSmoke = new TimeSpan(0, 0, 0);
            var testLeftTime = new TimeSpan(0, 0, 0);
            var currentSmokeTime = dateTime.Subtract(dateTime.AddDays(-1));
            var currentSmokeIdOne = string.Empty;
            var currentSmokeIdTwo = string.Empty;

            var test = new Test();
            var startSmokeTime = dateTime.AddDays(-1);

            var startedSmokeOne = new Smoke()
            {
                StartSmokeTime = startSmokeTime,
                Id = currentSmokeIdOne
            };

            var startedSmokeTwo = new Smoke()
            {
                StartSmokeTime = startSmokeTime,
                Id = currentSmokeIdTwo
            };

            test.SmokedCigaresUnderTest.Add(startedSmokeOne);
            test.SmokedCigaresUnderTest.Add(startedSmokeTwo);

            //Assert
            Assert.Throws<Exception>(() => testCalculationService
                .GetCurrentTestDataCalculation(dateTime, test));

        }

        /// <summary>
        /// Returns Tree smoked cigar
        /// </summary>
        [Test]
        public void Tree_Smoked_Cigar()
        {
            // Arrange
            var testCalculationService = new TestCalculationService();

            var dateTime = DateTime.Now;
            var currentlySmokedCount = 3;
            var currentSmokeIdOne = string.Empty;
            var currentSmokeIdTwo = string.Empty;
            var currentSmokeIdTree = string.Empty;

            var test = new Test();
            var startSmokeTime = dateTime.AddDays(-1);

            var startedSmokeOne = new Smoke()
            {
                StartSmokeTime = startSmokeTime,
                Id = currentSmokeIdOne,
                EndSmokeTime = dateTime.AddDays(1)
                
            };

            var startedSmokeTwo = new Smoke()
            {
                StartSmokeTime = startSmokeTime,
                Id = currentSmokeIdTwo
                ,
                EndSmokeTime = dateTime.AddDays(1)
            };

            var startedSmokeTree = new Smoke()
            {
                StartSmokeTime = startSmokeTime,
                Id = currentSmokeIdTree
               ,
                EndSmokeTime = dateTime.AddDays(1)
            };

            test.SmokedCigaresUnderTest.Add(startedSmokeOne);
            test.SmokedCigaresUnderTest.Add(startedSmokeTwo);
            test.SmokedCigaresUnderTest.Add(startedSmokeTree);

            // Act
            var serviceResult = testCalculationService
                .GetCurrentTestDataCalculation(dateTime, test);

            //Assert
            Assert.AreEqual(currentlySmokedCount, serviceResult.CurrentSmokedCount, "CurrentlySmokedCount not equal");

        }

        /// <summary>
        /// Returns Correct Test Time Left
        /// </summary>
        [Test]
        public void Calculate_Correct_Test_Time_Left()
        {
            // Arrange
            var testCalculationService = new TestCalculationService();

            var dateTime = DateTime.Now;
            var testDays = 4;
            var endDate = dateTime.AddDays(testDays);
            var expectedResult = endDate.Subtract(dateTime);

            var test = new Test() 
            {
                TestEndDate = endDate
            };

            // Act
            var serviceResult = testCalculationService
                .GetCurrentTestDataCalculation(dateTime, test);

            //Assert
            Assert.IsTrue(serviceResult.TestTimeLeft.Equals(expectedResult), "TestLeftTime not equal");

        }

        /// <summary>
        /// Returns Correct Last Smoke 
        /// </summary>
        [Test]
        public void Return_Correct_Last_Smoke()
        {
            // Arrange
            var testCalculationService = new TestCalculationService();

            var dateTime = DateTime.Now;
            var currentSmokeId = Guid.NewGuid().ToString();

            var test = new Test();
            var startSmokeTime = dateTime.AddDays(-1);
            var currentSmokeTime = dateTime.Subtract(startSmokeTime);

            var startedSmoke = new Smoke()
            {
                StartSmokeTime = startSmokeTime,
                Id = currentSmokeId,
                EndSmokeTime = startSmokeTime.AddDays(1)
            };

            test.SmokedCigaresUnderTest.Add(startedSmoke);

            // Act
            var serviceResult = testCalculationService
                .GetCurrentTestDataCalculation(dateTime, test);

            //Assert
            Assert.AreEqual(currentSmokeId, serviceResult.CurrentSmokeId, "CurrentSmokeId not equal");
            Assert.IsTrue(serviceResult.CurrentSmokeTime.Equals(currentSmokeTime), "CurrentSmokeTime not equal");

        }

        /// <summary>
        /// Returns Correct Time Sence Last Smoke 
        /// </summary>
        [Test]
        public void Return_Correct_Time_Sence_Last_Smoke()
        {
            // Arrange
            var testCalculationService = new TestCalculationService();

            var dateTime = DateTime.Now;
            var currentSmokeId = Guid.NewGuid().ToString();

            var test = new Test();
            var startSmokeTime = dateTime.AddDays(-1);
            var endSmokeTime = dateTime.AddDays(1);
            var timeSenceLastSmoke = dateTime.Subtract(endSmokeTime);

            var startedSmoke = new Smoke()
            {
                StartSmokeTime = startSmokeTime,
                Id = currentSmokeId,
                EndSmokeTime = endSmokeTime
            };

            test.SmokedCigaresUnderTest.Add(startedSmoke);

            // Act
            var serviceResult = testCalculationService
                .GetCurrentTestDataCalculation(dateTime, test);

            //Assert
            Assert.AreEqual(currentSmokeId, serviceResult.CurrentSmokeId, "CurrentSmokeId not equal");
            Assert.IsTrue(serviceResult.TimeSinceLastSmoke.Equals(timeSenceLastSmoke), "Time Sence Last Smoke not equal");

        }
    }
}