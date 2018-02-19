using System;

namespace MagnumPhotos.API.Entities
{
    public class Book
    {
        public Guid Id { get; set; }

        public Guid PhotographerId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime? CopyrightDate { get; set; }

        public Photographer Photographer { get; set; }
    }
}
