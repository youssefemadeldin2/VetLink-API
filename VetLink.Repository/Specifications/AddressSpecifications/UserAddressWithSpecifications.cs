using VetLink.Data.Entities;

namespace VetLink.Repository.Specifications.AddressSpecifications
{
    public class UserAddressWithSpecifications:BaseSpecification<Address>
    {
        public UserAddressWithSpecifications(int buyerId)
            :base(a=>a.UserId == buyerId)
        {
            
        }
    }
}
