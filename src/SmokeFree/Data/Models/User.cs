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
        /// Default Application Color Theme Skin
        /// </summary>
        public int AppColorThemeIndex { get; set; }

        /// <summary>
        /// State for Notification Service
        /// </summary>
        public bool NotificationState { get; set; }

        /// <summary>
        /// Current Test Id
        /// </summary>
        public int TestId { get; set; }

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
        public string UserState { get; set; }
    }
}
