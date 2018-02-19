using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MagnumPhotos.API.Models
{
    public class BookDto
    {
        public Guid Id { get; set; }
        public Guid PhotographerId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CopyrightDate { get; set; }
    }
}
