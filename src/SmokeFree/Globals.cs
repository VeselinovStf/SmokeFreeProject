﻿using System.Collections.Generic;

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
        public static bool MockRun = false;

        // Notification Ids
        public static int TestingTimeNotificationId = 666;
        public static int DelayedSmokeNotificationId = 999;

        /// <summary>
        /// App Hex Color Themes Set
        /// </summary>
        public static Dictionary<string, string> AppColorThemes = new Dictionary<string, string>()
        {
           {"Attention","#f54e5e"},
           {"Coulm","#2f72e4"},
           {"Mellow","#5d4cf7"},
           {"Goal","#06846a"},
           {"Win","#d54008"}
        };

        /// <summary>
        /// Build In User Id
        /// </summary>
        public static int UserId { get; set; } = 1;

        /// <summary>
        /// Application Rank Web Site URL
        /// </summary>
        public static string AppRankWebSiteUrl = "https://docs.microsoft.com/en-us/xamarin/essentials/open-browser?tabs=android";

        /// <summary>
        /// Application Web Site URL
        /// </summary>
        public static string AppWebSiteUrl = "https://docs.microsoft.com/en-us/xamarin/essentials/open-browser?tabs=android";

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
