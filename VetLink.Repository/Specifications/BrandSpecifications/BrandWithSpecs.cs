using VetLink.Data.Entities;
using VetLink.Repository.Specifications.Paginated;

namespace VetLink.Repository.Specifications.BrandSpecifications
{
    public class BrandWithSpecs : BaseSpecification<Brand>
    {
        public BrandWithSpecs(int Id, bool WithProduct = false)
            : base(b => b.Id == Id)
        {
            if (WithProduct)
                AddInclude(x => x.Products);
        }
        public BrandWithSpecs(string BrandName)
            : base(b => b.Name.ToLower() == BrandName.ToLower())
        {

        }
        public BrandWithSpecs(string? search, PaginationSpecification paging)
            : base(p => string.IsNullOrEmpty(search) || p.Name.Contains(search))
        {
            ApplyPaging(paging.PageSize * (paging.PageIndex - 1), paging.PageSize);
        }
    }
}
