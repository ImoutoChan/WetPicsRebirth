using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WetPicsRebirth.Data.Entities
{
    public class User : IEntityBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Username { get; set; }

        public DateTimeOffset AddedDate { get; set; }

        public DateTimeOffset ModifiedDate { get; set; }


        public IReadOnlyCollection<Vote>? Votes { get; set; }
    }
}
