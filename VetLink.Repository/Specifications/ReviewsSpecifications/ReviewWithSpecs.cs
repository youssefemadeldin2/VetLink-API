using VetLink.Data.Entities;

namespace VetLink.Repository.Specifications.ReviewsSpecifications
{
    public class ReviewWithSpecs:BaseSpecification<Review>
    {
        public ReviewWithSpecs(int productId)
            :base(r=>r.ProductId==productId)
        {
            AddInclude(r => r.Buyer);
        }
        public ReviewWithSpecs(int productId,int BuyerId)
            :base(r=>r.ProductId==productId && r.BuyerId ==BuyerId)
        {
        }
    }
}
