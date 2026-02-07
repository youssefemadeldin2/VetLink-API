using VetLink.Data.Entities;
using VetLink.Data.Enums;
using VetLink.Repository.Specifications;

public class ActiveOrderByBuyerSpec : BaseSpecification<Order>
{
	public ActiveOrderByBuyerSpec(int buyerId)
		: base(o =>
			o.BuyerId == buyerId &&
			o.DeletedAt == null &&
			o.Status != OredrStatus.Paid)
	{
		AddInclude(o => o.OrderItems);
		AddInclude(o => o.Address);
	}
}
