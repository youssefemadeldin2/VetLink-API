using System.Net.NetworkInformation;
using VetLink.Data.Entities;
using VetLink.Data.Enums;
using VetLink.Repository.Specifications.Paginated;

namespace VetLink.Repository.Specifications.OrderSpecifications
{
    public class OrderWithSpecification:BaseSpecification<Order>
    {
		public OrderWithSpecification(int orderId)
		   : base(o => o.Id == orderId && o.DeletedAt == null)
		{
			AddInclude(o => o.OrderItems);
			AddInclude(o => o.OrderCoupons);
		}
        public OrderWithSpecification(int? buyerId,OredrStatus? status, PaginationSpecification paging)
			: base(o =>
				(!buyerId.HasValue || o.BuyerId == buyerId) &&
				(!status.HasValue || o.Status == status) &&
				o.DeletedAt == null)
		{
			AddInclude(o => o.OrderItems);
			AddInclude(o => o.OrderCoupons);

			ApplyOrderByDescending(o => o.CreatedAt);
			ApplyPaging(paging.PageSize * (paging.PageIndex - 1), paging.PageSize);
		}
        public OrderWithSpecification(int buyerId,PaymentMethod paymentMethod)
			:base(o=> 
				o.BuyerId == buyerId &&
				o.PaymentStatus == PaymentStatus.pending &&
				o.Status == OredrStatus.PartiallyShipped)
        {
            
        }
    }
}
