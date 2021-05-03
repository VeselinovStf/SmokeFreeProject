using Realms;
using System;

namespace SmokeFree.Data.Models
{
    public class ChallengeResult : RealmObject
    {
        [PrimaryKey]
        [MapTo("_id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Id Of Challenge
        /// </summary>
        public string ChallengeId { get; set; }
    }
}
