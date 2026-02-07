using System.Net.NetworkInformation;
using VetLink.Data.Entities;
using VetLink.Data.Enums;
using VetLink.Repository.Specifications.Paginated;

namespace VetLink.Repository.Specifications.SellerSpecifications
{
    public class SellerWithSpecification : BaseSpecification<Seller>
    {
        public SellerWithSpecification(AccountStatus? status,PaginationSpecification paging,string? search)
			: base(s => (status == null || s.User.Status == status) &&
		        (string.IsNullOrEmpty(search) ||
			(s.User.FullName != null && s.User.FullName.Contains(search)) ||
			(s.User.Email != null && s.User.Email.Contains(search))))
		{
            AddInclude(x => x.User);
			ApplyPaging(paging.PageSize * (paging.PageIndex - 1), paging.PageSize);
		}
        public SellerWithSpecification(AccountStatus? status)
        {
            
        }
        public SellerWithSpecification(int SellerId)
            :base(s=>s.UserId==SellerId)
        {
			AddInclude("User.AuditLogs");

			AddInclude(s => s.Products);

			AddInclude("Orders");
			AddInclude("Orders.OrderItems");
			AddInclude(x => x.User);

			ApplyOrderByDescending(s => s.User.CreatedAt);
		}
    }
}
