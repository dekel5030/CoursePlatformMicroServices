namespace StorageService.S3;

#pragma warning disable CA1515 // Consider making public types internal
public class VideoProcessingException : Exception
#pragma warning restore CA1515
{
    public VideoProcessingException() { }

    public VideoProcessingException(string message) : base(message) { }

    public VideoProcessingException(string message, Exception inner) : base(message, inner) { }
}
