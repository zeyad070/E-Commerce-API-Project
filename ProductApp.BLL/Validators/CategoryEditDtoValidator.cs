using FluentValidation;

namespace ProductApp.BLL
{
    public class CategoryEditDtoValidator : AbstractValidator<CategoryEditDto>
    {
        public CategoryEditDtoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("A valid category ID is required");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .Length(2, 100).WithMessage("Name must be between 2 and 100 characters");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");
        }
    }
}
