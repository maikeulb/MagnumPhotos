using System.Collections.Generic;

namespace MagnumPhotos.API.Models
{
    public abstract class LinkedResourceBaseDto
    {
        public List<LinkDto> Links { get; set; } = new List<LinkDto> ();
    }
}