namespace SmokeFree.Models.Utilities.Device.EmailSender
{
    /// <summary>
    /// DeviceEmailSender - SendEmailAsync Response Model
    /// </summary>
    public class SendEmailAsyncResponse
    {
        /// <summary>
        /// Create iNSTANCE
        /// </summary>
        /// <param name="success">Success/Failse</param>
        /// <param name="message">Success message/Fail Message</param>
        public SendEmailAsyncResponse(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        public bool Success { get; }
        public string Message { get; }
    }
}
