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
        /// Internal Error Message Displayed to User
        /// </summary>
        public const string InternalErrorUserMessage = @"Application is bussy right now.
Please close and restart the application to fix this issue.
If problem persists, cleare application mobile data or connect to application development team.
Sorry for inconvenience.";

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
