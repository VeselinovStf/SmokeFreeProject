using System.Threading.Tasks;

namespace SmokeFree.Abstraction.Services.General
{
    /// <summary>
    /// Application Dialog Service Abstraction
    /// </summary>
    public interface IDialogService
    {
        /// <summary>
        /// Async Show Dialog To User
        /// </summary>
        /// <param name="message">Message To Display</param>
        /// <param name="title">Title of Message</param>
        /// <param name="buttonLabel">Button Title</param>
        /// <returns>Task</returns>
        Task ShowDialog(string message, string title, string buttonLabel);

        /// <summary>
        /// Show Toast To user
        /// </summary>
        /// <param name="message">Message of Toast</param>
        void ShowToast(string message);

        /// <summary>
        /// Async Show Confirmation to user
        /// </summary>
        /// <param name="message">Message To Display</param>
        /// <param name="title">Title of Confirm Message</param>
        /// <param name="okText">Ok Button Text</param>
        /// <param name="cancelText">Cancel Button Text</param>
        /// <returns>Cormifm result</returns>
        Task<bool> ConfirmAsync(string message, string title, string okText, string cancelText);
    }
}
