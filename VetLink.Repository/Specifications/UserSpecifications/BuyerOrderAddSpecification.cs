using VetLink.Data.Entities;
using VetLink.Data.Enums;

namespace VetLink.Repository.Specifications.UserSpecifications
{
    public class BuyerOrderAddSpecification:BaseSpecification<User>
    {
        public BuyerOrderAddSpecification(int BuyerId)
            :base(b=>b.Id==BuyerId && b.Status == AccountStatus.active)
        {
            AddInclude(b => b.Addresses);
        }
    }
}
