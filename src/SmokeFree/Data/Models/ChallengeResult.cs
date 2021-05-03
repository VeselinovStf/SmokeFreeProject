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
        /// Total Smoked
        /// </summary>
        public int TotalSmokedCigarsUnderChallenge { get; set; }

        /// <summary>
        /// Avarage smoked
        /// </summary>
        public double AvarageSmokedUnderChallenge { get; set; }

        /// <summary>
        /// Success Rate
        /// </summary>
        public double SuccessRate { get; set; }

        /// <summary>
        /// Skiped Smokes
        /// </summary>
        public int Skiped { get; set; }

        /// <summary>
        /// Rating
        /// </summary>
        public double Rating { get; set; }

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
        /// Completition Flag
        /// </summary>
        public bool IsCompleted { get; set; }
    }
}
