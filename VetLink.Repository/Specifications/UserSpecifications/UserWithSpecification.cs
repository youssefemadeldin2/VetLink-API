using VetLink.Data.Entities;
using VetLink.Data.Enums;
using VetLink.Repository.Specifications.Paginated;

namespace VetLink.Repository.Specifications.UserSpecifications
{
    public class UserWithSpecification : BaseSpecification<User>
    {
        public UserWithSpecification(int UserId)
            :base(u=>u.Id==UserId)
        {
			AddInclude(u => u.Orders);
			AddInclude(u => u.Notifications);
			AddInclude(u => u.Addresses);
			AddInclude("Orders.OrderItems");
			AddInclude("Orders.OrderItems.Product");
			AddInclude("Orders.Shipments");
			AddInclude("Orders.Returns");
			AddInclude("Orders.Returns.ReturnItems");
			AddInclude("Orders.Returns.ReturnItems.Product");
			EnableTracking(false);
		}
        public UserWithSpecification(string UserEmail)
            :base(u=>u.Email==UserEmail)
        {
            
        }

        public UserWithSpecification(PaginationSpecification paging ,string search)
            :base(u=> (u.FullName.Contains(search) || u.Email!.Contains(search)))
        {
			ApplyPaging(paging.PageSize * (paging.PageIndex - 1), paging.PageSize);
		}
    }
}
