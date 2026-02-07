using System.Net.NetworkInformation;
using VetLink.Data.Entities;
using VetLink.Repository.Specifications;
using VetLink.Repository.Specifications.Paginated;

public class BuyerOrdersSpecification : BaseSpecification<Order>
{
    public BuyerOrdersSpecification(int buyerId,PaginationSpecification paging)
        : base(o => o.BuyerId == buyerId)
    {
		AddInclude(o => o.OrderItems);
		AddInclude("OrderItems.Product");

		AddInclude(o => o.Address);
		AddInclude(o => o.Shipments);

		AddInclude("Returns.ReturnItems");
		AddInclude("Returns.ReturnItems.Product");

		ApplyOrderByDescending(o => o.CreatedAt);
		EnableTracking(false);
		ApplyPaging(paging.PageSize * (paging.PageIndex - 1), paging.PageSize);
	}
}
