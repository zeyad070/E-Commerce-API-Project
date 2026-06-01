using FluentValidation;

namespace ProductApp.BLL
{
    public class ProductEditDtoValidator : AbstractValidator<ProductEditDto>
    {
        public ProductEditDtoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("A valid product ID is required");

            RuleFor(x => x.Name)
                .Length(2, 100).WithMessage("Name must be between 2 and 100 characters")
                .When(x => x.Name != null);

            RuleFor(x => x.Description)
                .MaximumLength(200).WithMessage("Description cannot exceed 200 characters")
                .When(x => x.Description != null);

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0")
                .When(x => x.Price.HasValue);

            RuleFor(x => x.StockQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("Stock quantity cannot be negative")
                .When(x => x.StockQuantity.HasValue);

            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("A valid Category is required")
                .When(x => x.CategoryId.HasValue);
        }
    }
}
