

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

        public DateTime string CopyrightDate { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = new BookForManipulationDto()
            var result = validator.Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));
        }
    }
}
