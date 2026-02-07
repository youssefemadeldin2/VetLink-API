using AutoMapper;
using Microsoft.Extensions.Logging;
using VetLink.Data.Entities;
using VetLink.Repository.Interfaces;
using VetLink.Repository.Specifications.BrandSpecifications;
using VetLink.Repository.Specifications.Paginated;
using VetLink.Services.Helper;
using VetLink.Services.Services.BrandService.Dtos;
using VetLink.Services.Services.ProductService.Dtos;

namespace VetLink.Services.Services.BrandService
{
    public class BrandService : IBrandService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<BrandService> _logger;

        public BrandService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<BrandService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ServiceResult<BrandDto>> CreateBrandAsync(string brandName)
        {
            try
            {
                _logger.LogInformation("Creating brand with name: {BrandName}", brandName);

                if (string.IsNullOrWhiteSpace(brandName))
                    return ServiceResult<BrandDto>.Fail("Brand name cannot be empty.", 400);

                if (brandName.Length > 200)
                    return ServiceResult<BrandDto>.Fail("Brand name cannot exceed 200 characters.", 400);

                var brandRepo = _unitOfWork.Repository<Brand, int>();
                var specs = new BrandWithSpecs(brandName);
                var exist = await brandRepo.CountWithSpecAsync(specs);

                if (exist > 0)
                {
                    _logger.LogWarning("Brand already exists: {BrandName}", brandName);
                    return ServiceResult<BrandDto>.Fail("Brand already exists.", 409);
                }

                var brand = new Brand { Name = brandName.Trim() };
                var addedBrand = await brandRepo.AddAsync(brand);
                await _unitOfWork.SaveChangesAsync();

                if (addedBrand is null)
                {
                    _logger.LogError("Failed to create brand: {BrandName}", brandName);
                    return ServiceResult<BrandDto>.Fail("Failed to create brand.", 500);
                }

                var mapped = _mapper.Map<BrandDto>(addedBrand);
                _logger.LogInformation("Brand created successfully: {BrandId}", addedBrand.Id);
                return ServiceResult<BrandDto>.Created(mapped, "Brand created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating brand: {BrandName}", brandName);
                return ServiceResult<BrandDto>.ServerError("An error occurred while creating the brand.");
            }
        }

        public async Task<ServiceResult> DeleteBrandAsync(int id)
        {
            try
            {
                _logger.LogInformation("Deleting brand with ID: {BrandId}", id);

                if (id <= 0)
                    return ServiceResult.Fail("Invalid brand ID.", 400);

                var brandRepo = _unitOfWork.Repository<Brand, int>();
                var brand = await brandRepo.GetByIdAsync(id);

                if (brand is null)
                {
                    _logger.LogWarning("Brand not found: {BrandId}", id);
                    return ServiceResult.NotFound("Brand not found.");
                }

                brandRepo.Delete(brand);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Brand deleted successfully: {BrandId}", id);
                return ServiceResult.Ok("Brand deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting brand: {BrandId}", id);
                return ServiceResult.ServerError("An error occurred while deleting the brand.");
            }
        }

        public async Task<ServiceResult<PaginatedResultDto<BrandDto>>> GetAllBrandsAsync(
            string? search,
            PaginationSpecification pagination)
        {
            try
            {
                _logger.LogInformation("Getting brands with search: {Search}, page: {PageIndex}, size: {PageSize}",
                    search, pagination.PageIndex, pagination.PageSize);

                if (pagination.PageIndex < 1)
                    pagination.PageIndex = 1;

                if (pagination.PageSize < 1 || pagination.PageSize > 100)
                    pagination.PageSize = 20;

                var brandRepo = _unitOfWork.Repository<Brand, int>();
                var specs = new BrandWithSpecs(search, pagination);

                var brands = await brandRepo.ListAllWithSpecAsync(specs);
                var mapped = _mapper.Map<IReadOnlyList<BrandDto>>(brands);

                var totalItems = await brandRepo.CountWithSpecAsync(specs);
                var totalPages = (int)Math.Ceiling(totalItems / (double)pagination.PageSize);

                var paging = new PaginatedResultDto<BrandDto>(
                    mapped,
                    pagination.PageIndex,
                    pagination.PageSize,
                    totalItems,
                    totalPages
                );

                _logger.LogInformation("Retrieved {Count} brands", mapped.Count);

                // FIXED: Changed from "Featched" to "Fetched"
                return ServiceResult<PaginatedResultDto<BrandDto>>.Ok(
                    paging,
                    $"Brands fetched successfully. Total: {totalItems}"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting brands");
                return ServiceResult<PaginatedResultDto<BrandDto>>.ServerError(
                    "An error occurred while fetching brands.");
            }
        }

        public async Task<ServiceResult<BrandWithProductsDto>> GetBrandByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Getting brand by ID: {BrandId}", id);

                if (id <= 0)
                    return ServiceResult<BrandWithProductsDto>.Fail("Invalid brand ID.", 400);

                var brandRepo = _unitOfWork.Repository<Brand, int>();
                var specs = new BrandWithSpecs(id, true);

                var brand = await brandRepo.GetByIdWithSpecAsync(specs);

                if (brand is null)
                {
                    _logger.LogWarning("Brand not found: {BrandId}", id);
                    return ServiceResult<BrandWithProductsDto>.NotFound("Brand not found.");
                }

                var mapped = _mapper.Map<BrandWithProductsDto>(brand);
                _logger.LogInformation("Brand retrieved: {BrandId}", id);
                return ServiceResult<BrandWithProductsDto>.Ok(mapped, "Brand fetched successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting brand by ID: {BrandId}", id);
                return ServiceResult<BrandWithProductsDto>.ServerError(
                    "An error occurred while fetching the brand.");
            }
        }

        public async Task<ServiceResult<BrandDto>> UpdateBrandAsync(int id, string brandName)
        {
            try
            {
                _logger.LogInformation("Updating brand {BrandId} with name: {BrandName}", id, brandName);

                if (id <= 0)
                    return ServiceResult<BrandDto>.Fail("Invalid brand ID.", 400);

                if (string.IsNullOrWhiteSpace(brandName))
                    return ServiceResult<BrandDto>.Fail("Brand name cannot be empty.", 400);

                if (brandName.Length > 200)
                    return ServiceResult<BrandDto>.Fail("Brand name cannot exceed 200 characters.", 400);

                var brandRepo = _unitOfWork.Repository<Brand, int>();
                var brand = await brandRepo.GetByIdAsync(id);

                if (brand is null)
                {
                    _logger.LogWarning("Brand not found for update: {BrandId}", id);
                    return ServiceResult<BrandDto>.NotFound("Brand not found.");
                }

                var specs = new BrandWithSpecs(brandName);
                var existingBrands = await brandRepo.ListAllWithSpecAsync(specs);
                var duplicateBrand = existingBrands.FirstOrDefault(b => b.Id != id);

                if (duplicateBrand != null)
                {
                    _logger.LogWarning("Duplicate brand name: {BrandName}", brandName);
                    return ServiceResult<BrandDto>.Conflict("Another brand with the same name already exists.");
                }

                if (string.Equals(brand.Name, brandName, StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogInformation("Brand name unchanged: {BrandId}", id);
                    return ServiceResult<BrandDto>.Ok(_mapper.Map<BrandDto>(brand), "Brand name unchanged.");
                }

                brand.Name = brandName.Trim();
                brandRepo.Update(brand);
                await _unitOfWork.SaveChangesAsync();

                var mapped = _mapper.Map<BrandDto>(brand);
                _logger.LogInformation("Brand updated successfully: {BrandId}", id);
                return ServiceResult<BrandDto>.Ok(mapped, "Brand updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating brand {BrandId}", id);
                return ServiceResult<BrandDto>.ServerError("An error occurred while updating the brand.");
            }
        }
    }
}