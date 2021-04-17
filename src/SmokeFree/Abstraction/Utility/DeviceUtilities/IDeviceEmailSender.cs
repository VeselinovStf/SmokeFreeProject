using SmokeFree.Models.Utilities.Device.EmailSender;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmokeFree.Abstraction.Utility.DeviceUtilities
{
    /// <summary>
    /// Device Specific Email Sender Abstraction
    /// </summary>
    public interface IDeviceEmailSender
    {
        /// <summary>
        /// Async Send Email
        /// </summary>
        /// <param name="subject">Email Subject</param>
        /// <param name="body">Email Text Content</param>
        /// <param name="recipients">Email addresses to send to</param>
        /// <returns>Response Model</returns>
        Task<SendEmailAsyncResponse> SendEmailAsync(string subject, string body, List<string> recipients);

        /// <summary>
        /// Async Send Email with file attachment
        /// </summary>
        /// <param name="subject">Email Subject</param>
        /// <param name="body">Email Text Content</param>
        /// <param name="recipients">Email addresses to send to</param>
        /// <param name="filePath">Complete file path to attachment</param>
        /// <returns>Response Model</returns>
        Task<SendEmailAsyncResponse> SendEmailAsync(string subject, string body, List<string> recipients, string filePath);
    }
}
