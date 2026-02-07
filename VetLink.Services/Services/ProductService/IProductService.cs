using VetLink.Repository.Specifications.Paginated;
using VetLink.Services.Helper;
using VetLink.Services.Services.ProductService.Dtos;

namespace VetLink.Services.Services.ProductService
{
    public interface IProductService
    {
        Task<ServiceResult<ProductDetailsDto>> GetProductDetailsAsync(int productId);
        Task<ServiceResult<PaginatedResultDto<ProductProfileDto>>> GetAllProductsAsync(
            PaginationSpecification pagination,
            string? search);
        Task<ServiceResult<ProductDetailsDto>> CreateProductAsync(CreateProductDto dto);
        Task<ServiceResult> DeleteProductAsync(int productId);
        Task<ServiceResult<ProductDetailsDto>> UpdateProductAsync(UpdateProductDto dto);
        Task<ServiceResult> UpdateProductImagesAsync(UpdateProductImagesDto dto);
        Task<ServiceResult<List<ImageDto>>> AddProductImagesAsync(AddProductImagesDto dto);
        Task<ServiceResult> ChangePrimaryImageAsync(ChangePrimaryImageDto dto);
        Task<ServiceResult> DeleteProductImageAsync(DeleteProductImageDto dto);
    }
}