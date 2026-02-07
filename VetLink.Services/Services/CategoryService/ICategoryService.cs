using VetLink.Repository.Specifications.Paginated;
using VetLink.Services.Helper;
using VetLink.Services.Services.CategoryService.Dtos;

namespace VetLink.Services.Services.CategoryService
{
    public interface ICategoryService
    {
        Task<ServiceResult<CategoryWithProductsDto>> GetCategoryByIdAsync(int id);
        Task<ServiceResult<PaginatedResultDto<CategoryDto>>> GetAllCategoriesAsync(
            string? search,
            PaginationSpecification pagination);
        Task<ServiceResult<CategoryDto>> CreateCategoryAsync(CreateCategoryDto dto);
        Task<ServiceResult<CategoryDto>> UpdateCategoryAsync(int id, UpdateCategoryDto dto);
        Task<ServiceResult> DeleteCategoryAsync(int id);
        Task<ServiceResult<List<CategoryHierarchyDto>>> GetCategoryHierarchyAsync();
    }
}