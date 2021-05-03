using Realms;
using System;

namespace SmokeFree.Data.Models
{
    public class ChallengeSmoke : RealmObject
    {
        [PrimaryKey]
        [MapTo("_id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        /// <summary>
        /// Smoke Start Time
        /// </summary>
        public DateTimeOffset StartSmokeTime { get; set; }

        /// <summary>
        /// Smoke End Time
        /// </summary>
        public DateTimeOffset EndSmokeTime { get; set; }

        /// <summary>
        /// Challenge Relation Id
        /// </summary>
        public string ChallengeId { get; set; }

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
        /// Is Smoke Completed Flag
        /// </summary>
        public bool IsCompleted { get; set; }

        /// <summary>
        /// Skip Smoke Flag - Note that this marks Start-End as invalid
        /// </summary>
        public bool IsSkiped { get; set; }
    }
}
