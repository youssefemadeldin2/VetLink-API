using VetLink.Repository.Specifications.Paginated;
using VetLink.Services.Helper;
using VetLink.Services.Services.BrandService.Dtos;
using VetLink.Services.Services.ProductService.Dtos;

namespace VetLink.Services.Services.BrandService
{
	public interface IBrandService
	{
		Task<ServiceResult<BrandWithProductsDto>> GetBrandByIdAsync(int id);
		Task<ServiceResult<PaginatedResultDto<BrandDto>>> GetAllBrandsAsync(
			string? search,
			PaginationSpecification pagination);
		Task<ServiceResult<BrandDto>> CreateBrandAsync(string brandName);
		Task<ServiceResult<BrandDto>> UpdateBrandAsync(int id, string brandName);
		Task<ServiceResult> DeleteBrandAsync(int id);
	}
}