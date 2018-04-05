using System.Collections.Generic;

namespace MagnumPhotos.API.Models
{
    public abstract class LinkedResourceBase
    {
        public List<Link> Links { get; set; } = new List<Link> ();
    }
}
