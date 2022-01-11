namespace WetPicsRebirth.Data.Entities;

public class UserAccount : IEntityBase
{
    public UserAccount(long userId, ImageSource source, string login, string apiKey)
    {
        UserId = userId;
        Source = source;
        Login = login;
        ApiKey = apiKey;
    }

    public int Id { get; set; }

    public long UserId { get; set; }

    public ImageSource Source { get; set; }

    public string Login { get; set; }

    public string ApiKey { get; set; }

    public Instant AddedDate { get; set; }

    public Instant ModifiedDate { get; set; }


    public User? User { get; set; }
}
