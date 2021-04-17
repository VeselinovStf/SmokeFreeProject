namespace SmokeFree.Models.Utilities.Zip
{
    /// <summary>
    /// LocalLogUtility - CreateLogZipFile Response Model
    /// </summary>
    public class CreateLogZipFileResponse
    {
        /// <summary>
        /// Create instance 
        /// </summary>
        /// <param name="created">Created/Not Created</param>
        public CreateLogZipFileResponse(bool created)
        {
            Created = created;
        }

        /// <summary>
        /// Create Instance
        /// </summary>
        /// <param name="created">Created/Not Created</param>
        /// <param name="message">Content/Error Message</param>
        public CreateLogZipFileResponse(bool created, string message) : this(created)
        {
            Message = message;
        }

        public bool Created { get; }

        public string Message { get; }
    }
}
