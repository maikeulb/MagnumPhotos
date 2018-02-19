using System;

namespace MagnumPhotos.API.Entities
{
    public class Book
    {
        public int Id { get; set; }

        public int PhotographerId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime CopyrightDate { get; set; }

        public Photographer Photographer { get; set; }
    }
}
