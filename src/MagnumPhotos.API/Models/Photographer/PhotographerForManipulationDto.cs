using System;
using System.Collections.Generic;

namespace MagnumPhotos.API.Models
{
    public abstract class PhotographerForManipulationDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTimeOffset DateOfBirth { get; set; }
        public string Genre { get; set; }
        public ICollection<BookForCreationDto> Books { get; set; } = new List<BookForCreationDto> ();
    }
}
