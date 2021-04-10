using Realms;
using System;

namespace SmokeFree.Data.Models
{
    public class DayChallengeSmoke : RealmObject
    {
        [PrimaryKey]
        [MapTo("_id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        /// <summary>
        /// Order Of Smokes by day
        /// </summary>
        public int DayOfChallenge { get; set; }

        /// <summary>
        /// Distance from current to next smoke
        /// </summary>
        public DateTimeOffset DistanceToNext { get; set; }

        /// <summary>
        /// Smoke Start Time
        /// </summary>
        public DateTimeOffset StartSmokeTime { get; set; }

        /// <summary>
        /// Smoke End Time
        /// </summary>
        public DateTimeOffset EndSmokeTime { get; set; }

        /// <summary>
        /// Max Smokes Limit For Current Day
        /// </summary>
        public int DayMaxSmokesLimit { get; set; }

        /// <summary>
        /// Count of actualy smoked for a day
        /// </summary>
        public int DaySmoked { get; set; }

        /// <summary>
        /// Completition Flag
        /// </summary>
        public bool IsCompleted { get; set; }

        /// <summary>
        /// Foreign Key To Challenge
        /// </summary>
        public int ChallengeId { get; set; }

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