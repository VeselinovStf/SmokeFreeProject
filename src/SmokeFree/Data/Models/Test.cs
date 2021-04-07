using Realms;
using System;
using System.Collections.Generic;

namespace SmokeFree.Data.Models
{
    /// <summary>
    /// User Test Entity
    /// </summary>
    public class Test : RealmObject
    {
        [PrimaryKey]
        [MapTo("_id")]
        public int Id { get; set; }
        /// <summary>
        /// Start Date Of Test
        /// </summary>
        public DateTimeOffset TestStartDate { get; set; }

        /// <summary>
        /// End Date Of Test
        /// </summary>
        public DateTimeOffset TestEndDate { get; set; }

        /// <summary>
        /// Test Completition
        /// </summary>
        public bool IsCompleted { get; set; }

        /// <summary>
        /// User Under Test Id
        /// </summary>      
        public int UserId { get; set; }

        /// <summary>
        /// Smoked Cigares Under Test
        /// </summary>
        public IList<Smoke> SmokedCigaresUnderTest { get; }

        /// <summary>
        /// Completed Test Results Persistence Data
        /// </summary>    
        public TestResult CompletedTestResult { get; set; }

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
    }
}
