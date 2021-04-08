using Acr.UserDialogs;
using SmokeFree.Abstraction.Services.General;
using System;
using System.Threading.Tasks;

namespace SmokeFree.Services.General
{
    /// <summary>
    /// Dialog Service - Uses Acr.UserDialogs
    /// </summary>
    public class DialogService : IDialogService
    {
        /// <summary>
        /// Async Show Dialog To User
        /// </summary>
        /// <param name="message">Message To Display</param>
        /// <param name="title">Title of Message</param>
        /// <param name="buttonLabel">Button Title</param>
        /// <returns>Task</returns>
        public async Task ShowDialog(string message, string title, string buttonLabel)
        {
            await UserDialogs.Instance.AlertAsync(message, title, buttonLabel);
        }

        /// <summary>
        /// Show Toast To user
        /// </summary>
        /// <param name="message">Message of Toast</param>
        public void ShowToast(string message)
        {
            UserDialogs.Instance.Toast(message, new TimeSpan(0, 0, 20));
        }

        /// <summary>
        /// Async Show Confirmation to user
        /// </summary>
        /// <param name="message">Message To Display</param>
        /// <param name="title">Title of Confirm Message</param>
        /// <param name="okText">Ok Button Text</param>
        /// <param name="cancelText">Cancel Button Text</param>
        /// <returns>Cormifm result</returns>
        public async Task<bool> ConfirmAsync(string message, string title, string okText, string cancelText)
        {
            return await UserDialogs.Instance.ConfirmAsync(message, title, okText, cancelText);
        }
    }
}
