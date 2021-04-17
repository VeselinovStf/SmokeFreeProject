using SmokeFree.Data.Models;

namespace SmokeFree.Models.Services.Data.Test
{
    /// <summary>
    /// TestCalculationService - CalculateTestResult Response Model
    /// </summary>
    public class CalculateTestResultDTO
    {
        public CalculateTestResultDTO(bool success)
        {
            Success = success;
        }

        public CalculateTestResultDTO(bool success, string message) : this(success)
        {
            Success = success;
        }

        public CalculateTestResultDTO(bool success, TestResult testResultCalculation):this(success)
        {
            TestResultCalculation = testResultCalculation;
        }

        /// <summary>
        /// Method Result
        /// </summary>
        public TestResult TestResultCalculation { get; }

        public bool Success { get; }
        public string Message { get; }
    }
}
