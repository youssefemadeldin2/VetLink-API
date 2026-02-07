using FluentValidation;
using VetLink.Services.Services.OrderService.Dtos;

public class OrderActivationRequestValidator
	: AbstractValidator<OrderActivationRequest>
{
	public OrderActivationRequestValidator()
	{
		RuleFor(x => x.BuyerId)
			.GreaterThan(0);

		RuleFor(x => x.PaymentMethod)
			.IsInEnum();

		RuleFor(x => x.Street)
			.NotEmpty()
			.MaximumLength(250);
	}
}
