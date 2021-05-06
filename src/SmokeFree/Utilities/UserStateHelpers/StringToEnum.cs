using System;

namespace SmokeFree.Utilities.UserStateHelpers
{
    /// <summary>
    /// User State Converter
    /// </summary>
    public static class StringToEnum
    {
        /// <summary>
        /// Convert String To User State Enum
        /// </summary>
        /// <param name="userState">User State Enum As String</param>
        /// <returns></returns>
        public static T ToUserState<T>(string userState)
        {
            return (T)Enum.Parse(typeof(T), userState);
        }
    }
}
