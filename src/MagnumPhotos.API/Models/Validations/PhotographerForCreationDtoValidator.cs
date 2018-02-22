using MagnumPhotos.API.Models;
using FluentValidation;

namespace MagnumPhotos.API.Models.Validators
{
    public class PhotographerForCreationDtoValidator : AbstractValidator<PhotographerForCreationDto>
    {
        public PhotographerForCreationDtoValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("Please fill out a firstname.");
            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("Please fill out lastname.");
            RuleFor(x => x.Genre)
                .NotEmpty()
                .WithMessage("Please fill out genre.");
        }
    }
}
