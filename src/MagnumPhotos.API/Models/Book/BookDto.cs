using System;

namespace MagnumPhotos.API.Models
{
    public class BookDto : LinkedResourceBase
    {
        public Guid Id { get; set; }
        public Guid PhotographerId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CopyrightDate { get; set; }
    }
}
