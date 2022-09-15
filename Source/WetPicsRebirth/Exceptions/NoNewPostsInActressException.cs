namespace WetPicsRebirth.Exceptions;

public class NoNewPostsInActressException : Exception
{
    public NoNewPostsInActressException() : base("No new images in actress")
    {
    }
}