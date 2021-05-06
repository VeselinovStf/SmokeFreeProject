namespace SmokeFree.Models.Utilities.Zip
{
    public class CreateDbZipFileResponse
    {

        public CreateDbZipFileResponse(bool created)
        {
            Created = created;
        }


        public CreateDbZipFileResponse(bool created, string message) : this(created)
        {
            Message = message;
        }

        public bool Created { get; }

        public string Message { get; }
    }
}
