using FluentValidation;
using IMDBClone.API.Dtos.MovieDtos;

namespace IMDBClone.API.Validators
{
    public class MovieCreateDtoValidator : AbstractValidator<MovieCreateDto>
    {
        public MovieCreateDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");

            RuleFor(x => x.Year)
                .InclusiveBetween(1888, DateTime.Now.Year)
                .WithMessage("Year must be valid");

            RuleFor(x => x.Overview)
                .NotEmpty().WithMessage("Overview is required");

            RuleFor(x => x.PosterUrl)
                .NotEmpty().WithMessage("Poster URL is required")
                .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
                .WithMessage("Poster URL must be a valid URL");


        }
    }
}
