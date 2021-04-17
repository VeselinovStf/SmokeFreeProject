using SmokeFree.Abstraction.Utility.DeviceUtilities;
using SmokeFree.Models.Utilities.Device.EmailSender;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace SmokeFree.Utilities.DeviceUtilities
{
    /// <summary>
    /// Device Specific Email Sender
    /// </summary>
    public class DeviceEmailSender : IDeviceEmailSender
    {
        /// <summary>
        /// Async Send Email
        /// </summary>
        /// <param name="subject">Email Subject</param>
        /// <param name="body">Email Text Content</param>
        /// <param name="recipients">Email addresses to send to</param>
        /// <returns>Response Model</returns>
        public async Task<SendEmailAsyncResponse> SendEmailAsync(string subject, string body, List<string> recipients)
        {
            try
            {
                var message = new EmailMessage
                {
                    Subject = subject,
                    Body = body,
                    To = recipients,
                };

                await Email.ComposeAsync(message);

                return new SendEmailAsyncResponse(true, "Email Send");

            }
            catch (Exception ex)
            {
                return new SendEmailAsyncResponse(false, ex.Message);
            }
        }

        /// <summary>
        /// Async Send Email with file attachment
        /// </summary>
        /// <param name="subject">Email Subject</param>
        /// <param name="body">Email Text Content</param>
        /// <param name="recipients">Email addresses to send to</param>
        /// <param name="filePath">Complete file path to attachment</param>
        /// <returns>Response Model</returns>
        public async Task<SendEmailAsyncResponse> SendEmailAsync(string subject, string body, List<string> recipients, string filePath)
        {
            try
            {
                var message = new EmailMessage
                {
                    Subject = subject,
                    Body = body,
                    To = recipients,
                };

                message.Attachments.Add(new EmailAttachment(filePath));

                await Email.ComposeAsync(message);

                return new SendEmailAsyncResponse(true, "Email Send");
            }         
            catch (Exception ex)
            {
                return new SendEmailAsyncResponse(false, ex.Message);
            }
        }
    }
}
