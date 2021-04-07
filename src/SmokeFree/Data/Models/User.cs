using Realms;
using System;
using System.Collections.Generic;

namespace SmokeFree.Data.Models
{
    public class User : RealmObject
    {
        [PrimaryKey]
        [MapTo("_id")]
        public int Id { get; set; }

        /// <summary>
        /// Flags On Boarding Completition.
        /// </summary>
        public bool CompletedOnBoarding { get; set; }

        /// <summary>
        /// Default Application Color Theme Skin
        /// </summary>
        public int AppColorThemeIndex { get; set; }

        /// <summary>
        /// State for Notification Service
        /// </summary>
        public bool NotificationState { get; set; }

        /// <summary>
        /// Flags User Under Test
        /// </summary>
        public bool UserUnderTesting { get; set; }

        /// <summary>
        /// Current Test Id
        /// </summary>
        public int TestId { get; set; }

        /// <summary>
        /// Flags Complete Testing
        /// </summary>
        public bool IsTestComplete { get; set; }

        /// <summary>
        /// Create Test View - Furst Run Description
        /// </summary>
        public bool CreateTestFirstRun { get; set; }

        /// <summary>
        /// Is User In Challenge
        /// </summary>
        public bool InChallenge { get; set; }

        /// <summary>
        /// User Tests
        /// </summary>
        public IList<Test> Tests { get; }

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
        /// User Challenges
        /// </summary>
        public IList<Challenge> Challenges { get;  }
    }
}
