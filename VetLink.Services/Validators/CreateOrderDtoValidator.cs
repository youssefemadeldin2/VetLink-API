using FluentValidation;
using VetLink.Services.Services.OrderService.Dtos;

namespace VetLink.Services.Validators
{
    public class CreateOrderDtoValidator: AbstractValidator<CreateOrderDto>
	{
		public CreateOrderDtoValidator()
		{
			RuleFor(x => x.AddressId)
				.GreaterThan(0);

			RuleFor(x => x.Items)
				.NotEmpty()
				.WithMessage("Order must contain at least one item");

			RuleForEach(x => x.Items).ChildRules(items =>
			{
				items.RuleFor(i => i.ProductId).GreaterThan(0);
				items.RuleFor(i => i.Quantity).GreaterThan(0);
			});
		}
	}
}
