using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MagnumPhotos.API.Models
{
    public abstract class BookForManipulationDto
    {
        public string Title { get; set; }
        public virtual string Description { get; set; }
        public DateTime CopyrightDate { get; set; }
    }
}
