using FluentValidation;

namespace ProductApp.BLL
{
    public class UpdateCartDtoValidator : AbstractValidator<UpdateCartDto>
    {
        public UpdateCartDtoValidator()
        {
            RuleFor(x => x.ProductId)
                .GreaterThan(0).WithMessage("A valid product ID is required");

            RuleFor(x => x.Quantity)
                .GreaterThanOrEqualTo(0).WithMessage("Quantity cannot be negative");
        }
    }
}
