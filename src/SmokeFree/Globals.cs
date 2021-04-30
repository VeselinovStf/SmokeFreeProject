using System;
using System.Collections.Generic;

namespace SmokeFree
{
    /// <summary>
    /// App Globals
    /// </summary>
    public static class Globals
    {
        /// <summary>
        /// Used For Testing 
        /// </summary>
        public static bool MockRun = true;

        // Notification Ids
        public static int TestingTimeNotificationId = 666;
        public static int DelayedSmokeNotificationId = 999;

        /// <summary>
        /// App Hex Color Themes Set
        /// </summary>
        public static List<string> AppColorThemes = new List<string>()
        {
            "#f54e5e",
            "#2f72e4",
            "#5d4cf7",
            "#06846a",
            "#d54008"
        };

        /// <summary>
        /// Build In User Id
        /// </summary>
        public static int UserId { get; set; } = 1;

        /// <summary>
        /// User issues email report emails
        /// </summary>
        public static List<string> ReportIssueEmails = new List<string>()
        {
            "chofexx@gmail.com"
        };

        /// <summary>
        /// Issue Report Message Title
        /// </summary>
        public static string IssueReportTitle = "! [SMOKE FREE] : [ISSUE REPORT] !";

        /// <summary>
        /// Issue report message body
        /// </summary>
        public static string IssueReportBody = "User Reports and issue, Email contains application Log Attachment";

        /// <summary>
        /// Limits if User Forgets To push Smoke Done
        /// </summary>
        public static int OneSmokeTreshHoldTimeMinutes = 9;

        /// <summary>
        /// Minimum Limit for Challenge in Days
        /// </summary>
        public static int MinChallengeDays = 3;
    }
}
