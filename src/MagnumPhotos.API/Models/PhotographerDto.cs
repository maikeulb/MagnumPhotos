using System;

namespace MagnumPhotos.API.Models
{
    public class PhotographerDto : LinkedResourceBaseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Genre { get; set; }
    }
}