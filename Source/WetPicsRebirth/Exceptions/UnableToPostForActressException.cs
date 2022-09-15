namespace WetPicsRebirth.Exceptions;

public class UnableToPostForActressException : Exception
{
    public UnableToPostForActressException(int postId, Exception innerException) 
        : base($"Unable to post with id {postId}", innerException)
    {
    }
}