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

        public string Localozation { get; set; }
        /// <summary>
        /// Default Application Color Theme Skin
        /// </summary>
        //public int AppColorThemeIndex { get; set; }

        /// <summary>
        /// State for Notification Service
        /// </summary>
        public bool NotificationState { get; set; } = true;

        /// <summary>
        /// Current Test Id
        /// </summary>
        public string TestId { get; set; }

        /// <summary>
        /// Completed Challenge Result
        /// </summary>
        public string ChallengeResultId { get; set; }

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
        public IList<Challenge> Challenges { get; }

        /// <summary>
        /// User State In Application
        /// </summary>
        public string UserState { get; set; } = UserStates.Initial.ToString();

        /// <summary>
        /// User Smoking Habbit Status
        /// </summary>
        public string UserSmokeStatuses { get; set; }
    }
}
