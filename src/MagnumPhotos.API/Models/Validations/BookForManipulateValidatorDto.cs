using MagnumPhotos.Api.Models
using FluentValidation;

namespace MagnumPhotos.Api.Models
{
    public class BookForManipulationDtoValidator : AbstractValidator<BookForManipulateDto>
    {
        public BookForManipulationDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Please fill out a title.");
            RuleFor(x => x.Title)
                .Length(100)
                .WithMessage("Please limit message to 100 characters.");
            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Please limit description to 500 characters");
        }
    }
}
