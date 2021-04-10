using SmokeFree.Data.Models;
using System;

namespace SmokeFree.Utilities.UserStateHelpers
{
    /// <summary>
    /// User State Converter
    /// </summary>
    public static class UserStateConverter
    {
        /// <summary>
        /// Convert String To User State
        /// </summary>
        /// <param name="userState">User State As String</param>
        /// <returns></returns>
        public static UserStates ToUserState(string userState)
        {
            return (UserStates)Enum.Parse(typeof(UserStates), userState);
        }
    }
}
