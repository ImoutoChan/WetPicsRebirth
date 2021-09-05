using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WetPicsRebirth.Data.Entities
{
    public class User : IEntityBase
    {
        public User(
            int id,
            string firstName,
            string lastName,
            string username)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Username = username;
        }
        
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; private set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Username { get; set; }

        public DateTimeOffset AddedDate { get; set; }

        public DateTimeOffset ModifiedDate { get; set; }


        public IReadOnlyCollection<Vote>? Votes { get; set; }
    }
}