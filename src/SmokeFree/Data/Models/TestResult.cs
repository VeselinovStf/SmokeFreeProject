using Realms;
using System;

namespace SmokeFree.Data.Models
{
    /// <summary>
    /// Completed Test Results
    /// </summary>
    public class TestResult : RealmObject
    {
        [PrimaryKey]
        [MapTo("_id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        /// <summary>
        /// Total Time of Test
        /// </summary>
        public DateTimeOffset TotalTestTime { get; set; }

        /// <summary>
        /// Start Date Of Test
        /// </summary>
        public DateTimeOffset TestStartDate { get; set; }

        /// <summary>
        /// End Date Of Test
        /// </summary>
        public DateTimeOffset EndStartDate { get; set; }

        /// <summary>
        /// Total Smoked Cigars
        /// </summary>
        public int TotalSmokedCigars { get; set; }

        /// <summary>
        /// Avarage Smoked Cigars Per Day
        /// </summary>
        public double AvarageSmokedCigarsPerDay { get; set; }

        /// <summary>
        /// Avarage No smoking time
        /// </summary>
        public double AvarageCleanOxygenTimeSeconds { get; set; }

        /// <summary>
        /// Total Smoking Time 
        /// </summary>
        public double TotalSmokeGasTimeTimeSeconds { get; set; }

        /// <summary>
        /// Avarage Distance Between smokes
        /// </summary>
        public double AvarageSmokeDistanceSeconds { get; set; }

        /// <summary>
        /// Test Result Clafication for Smoker Type
        /// </summary>
        public string SmokerType { get; set; }

        /// <summary>
        /// Time from first smoke for day to last smoke for day
        /// </summary>
        public double AvarageSmokeActiveTimeSeconds { get; set; }

        /// <summary>
        /// Last Modifiet date
        /// </summary>
        public DateTimeOffset ModifiedOn { get; set; }

        /// <summary>
        /// Creation Date
        /// </summary>
        public DateTimeOffset CreatedOn { get; set; }

        /// <summary>
        /// Is Deleted State
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Deletiton Time
        /// </summary>
        public DateTimeOffset DeletedOn { get; set; }

        /// <summary>
        /// Id Of Test
        /// </summary>
        public int TestId { get; set; }
    }
}
