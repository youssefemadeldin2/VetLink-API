using VetLink.Data.Entities;
using VetLink.Repository.Specifications.Paginated;

namespace VetLink.Repository.Specifications.CategorySpecifications
{
    public class CategoryWithSpecification : BaseSpecification<Category>
    {
        public CategoryWithSpecification(int categoryId,bool track=false) //by id
            : base((p => p.Id == categoryId))
        {
            AddInclude(c => c.SubCategories);

            EnableTracking(track);
        }
        public CategoryWithSpecification(string CategoryName): //craete cat
            base(c=>c.Name.ToLower() == CategoryName.ToLower())
        {
            EnableTracking(false);
        }
        public CategoryWithSpecification(string? search,PaginationSpecification paging) //getall cat
            :base(p => string.IsNullOrEmpty(search) ||
			p.Name.Contains(search) )
        {
			ApplyPaging(paging.PageSize * (paging.PageIndex - 1), paging.PageSize);
			AddInclude(c => c.SubCategories);
			EnableTracking(false);
		}
    }
}