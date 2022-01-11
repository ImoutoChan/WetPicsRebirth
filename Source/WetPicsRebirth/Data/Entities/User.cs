using System.ComponentModel.DataAnnotations.Schema;

namespace WetPicsRebirth.Data.Entities;

public class User : IEntityBase
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public long Id { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Username { get; set; }

    public Instant AddedDate { get; set; }

    public Instant ModifiedDate { get; set; }


    public IReadOnlyCollection<Vote>? Votes { get; set; }

    public IReadOnlyCollection<UserAccount>? UserAccounts { get; set; }
}
