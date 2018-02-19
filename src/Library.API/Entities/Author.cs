using System;
using System.Collections.Generic;

namespace Library.API.Entities
{
    public class Author
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTimeOffset DateOfBirth { get; set; }

        public DateTimeOffset? DateOfDeath { get; set; }

        public string Genre { get; set; }

        public ICollection<Book> Books { get; set; }
            = new List<Book>();
    }
}
