using Realms;
using System;
using System.Collections.Generic;

namespace SmokeFree.Data.Models
{
    /// <summary>
    /// User Stop Smoking Challenge
    /// </summary>
    public class Challenge : RealmObject
    {
        [PrimaryKey]
        [MapTo("_id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        /// <summary>
        /// Start of the challenge
        /// </summary>
        public DateTimeOffset ChallengeStart { get; set; }

        /// <summary>
        /// User Smoking Challenge Completition Date
        /// </summary>
        public DateTimeOffset GoalCompletitionTime { get; set; }

        /// <summary>
        /// Total Challenge Days
        /// </summary>
        public int TotalChallengeDays { get; set; }

        /// <summary>
        /// Counter of Challenge
        /// </summary>
        public int CurrentDayOfChallenge { get; set; }

        /// <summary>
        /// Set of Smokes for Challenge
        /// </summary>     
        public IList<DayChallengeSmoke> ChallengeSmokes { get; }

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
        /// User Id
        /// </summary>
        public int UserId { get; set; }
    }
}
