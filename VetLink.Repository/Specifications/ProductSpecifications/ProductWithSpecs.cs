using VetLink.Data.Entities;
using VetLink.Repository.Specifications.Paginated;

namespace VetLink.Repository.Specifications.ProductSpecifications
{
    public class ProductWithSpecs:BaseSpecification<Product>
    {
        public ProductWithSpecs(string ProductName, int SellerId )
            :base(p =>
				p.Name.ToLower() == ProductName.ToLower()
				&& p.SellerId == SellerId
				&& p.DeletedAt == null)
        {
            
        }
        public ProductWithSpecs(string? search,PaginationSpecification paging)
			: base(p => string.IsNullOrEmpty(search) ||
					p.Name.Contains(search) ||
					p.SKU.Contains(search) ||
					p.Description.Contains(search)||
					p.Category.Name.Contains(search)||
					p.Seller.StoreName.Contains(search)||
                    p.Brand.Name.Contains(search))
		{
            AddInclude(X=>X.Images);
            AddInclude(X=>X.Brand);
            AddInclude(X=>X.Category);
            AddInclude(X=>X.Seller);
			ApplyPaging(paging.PageSize * (paging.PageIndex - 1), paging.PageSize);
			ApplyOrderByDescending(p => p.CreatedAt);
		}
        public ProductWithSpecs(string SKU)
            :base(p=>p.SKU==SKU)
        {
            AddInclude(x=>x.Seller);
            AddInclude(x=>x.Brand);
            AddInclude(x=>x.Category);
            AddInclude(x=>x.Images);
            AddInclude(x=>x.ProductStats);
            AddInclude(x=>x.Reviews);
        }
        public ProductWithSpecs(int Id)
            :base(p=>p.Id==Id && p.DeletedAt == null)
        {
            AddInclude(x=>x.Seller);
            AddInclude(x=>x.Brand);
            AddInclude(x=>x.Category);
            AddInclude(x=>x.Images);
            AddInclude(x=>x.ProductStats);
        }
        public ProductWithSpecs(ProductsOf productsOf, PaginationSpecification paging)
            :base(p=> p.DeletedAt == null&&
			(!productsOf.BrandId.HasValue || p.BrandId == productsOf.BrandId) &&
				(!productsOf.CategoryId.HasValue || p.CategoryId == productsOf.CategoryId) &&
				(!productsOf.SellerId.HasValue || p.SellerId == productsOf.SellerId))
        {
            AddInclude(x=>x.Seller);
            AddInclude(x=>x.Images);
			ApplyPaging(paging.PageSize * (paging.PageIndex - 1), paging.PageSize);
			ApplyOrderByDescending(p => p.CreatedAt);

		}
        
    }
}
