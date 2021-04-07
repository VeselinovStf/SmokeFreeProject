using Realms;
using System;

namespace SmokeFree.Data.Models
{
    /// <summary>
    /// Single Smoke Entity
    /// </summary>
    public class Smoke : RealmObject
    {
        [PrimaryKey]
        [MapTo("_id")]
        public int Id { get; set; }
        /// <summary>
        /// Smoke Start Time
        /// </summary>
        public DateTimeOffset StartSmokeTime { get; set; }

        /// <summary>
        /// Smoke End Time
        /// </summary>
        public DateTimeOffset EndSmokeTime { get; set; }

        /// <summary>
        /// Test Relation Id
        /// </summary>
        public int TestId { get; set; }

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