using System.Net;
using AutoMapper;
using Microsoft.Extensions.Logging;
using VetLink.Data.Entities;
using VetLink.Repository.Interfaces;
using VetLink.Repository.Specifications.CategorySpecifications;
using VetLink.Repository.Specifications.Paginated;
using VetLink.Services.Helper;
using VetLink.Services.Services.CategoryService.Dtos;

namespace VetLink.Services.Services.CategoryService
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CategoryService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ServiceResult<CategoryWithProductsDto>> GetCategoryByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Getting category by ID: {CategoryId}", id);

                if (id <= 0)
                    return ServiceResult<CategoryWithProductsDto>.Fail("Invalid category ID.", 400);

                var spec = new CategoryWithSpecification(id, true);
                var category = await _unitOfWork.Repository<Category, int>().GetByIdWithSpecAsync(spec);

                if (category is null)
                {
                    _logger.LogWarning("Category not found: {CategoryId}", id);
                    return ServiceResult<CategoryWithProductsDto>.NotFound("Category not found.");
                }

                var mapped = _mapper.Map<CategoryWithProductsDto>(category);
                _logger.LogInformation("Category retrieved: {CategoryId}", id);
                return ServiceResult<CategoryWithProductsDto>.Ok(mapped, "Category fetched successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting category by ID: {CategoryId}", id);
                return ServiceResult<CategoryWithProductsDto>.ServerError(
                    "An error occurred while fetching the category.");
            }
        }

        public async Task<ServiceResult<PaginatedResultDto<CategoryDto>>> GetAllCategoriesAsync(
            string? search,
            PaginationSpecification pagination)
        {
            try
            {
                _logger.LogInformation("Getting categories with search: {Search}, page: {PageIndex}, size: {PageSize}",
                    search, pagination.PageIndex, pagination.PageSize);
                var categoryRepo = _unitOfWork.Repository<Category, int>();

				if (pagination.PageIndex < 1)
                    pagination.PageIndex = 1;

                if (pagination.PageSize < 1 || pagination.PageSize > 100)
                    pagination.PageSize = 20;

                var spec = new CategoryWithSpecification(search, pagination);
                var categories = await categoryRepo.ListAllWithSpecAsync(spec);

                var mapped = _mapper.Map<IReadOnlyList<CategoryDto>>(categories);

                var totalItems = await _unitOfWork.Repository<Category, int>().CountWithSpecAsync(spec);
                var totalPages = (int)Math.Ceiling(totalItems / (double)pagination.PageSize);

                var paging = new PaginatedResultDto<CategoryDto>(
                    mapped,
                    pagination.PageIndex,
                    pagination.PageSize,
                    totalItems,
                    totalPages
                );

                _logger.LogInformation("Retrieved {Count} categories", mapped.Count);
                return ServiceResult<PaginatedResultDto<CategoryDto>>.Ok(
                    paging,
                    "Categories fetched successfully"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting categories");
                return ServiceResult<PaginatedResultDto<CategoryDto>>.ServerError(
                    "An error occurred while fetching categories.");
            }
        }

        public async Task<ServiceResult<CategoryDto>> CreateCategoryAsync(CreateCategoryDto dto)
        {
            try
            {
                _logger.LogInformation("Creating category: {CategoryName}", dto.Name);

                var categoryRepo = _unitOfWork.Repository<Category, int>();

                // Validate parent category if specified
                if (dto.ParentId.HasValue && dto.ParentId.Value > 0)
                {
                    var parentExists = await categoryRepo.ExistsAsync(dto.ParentId.Value);
                    if (!parentExists)
                    {
                        _logger.LogWarning("Parent category not found: {ParentId}", dto.ParentId.Value);
                        return ServiceResult<CategoryDto>.Fail("Parent category not found.");
                    }
                }

                // Check for duplicate category name
                var spec = new CategoryWithSpecification(dto.Name);
                var count = await categoryRepo.CountWithSpecAsync(spec);

                if (count > 0)
                {
                    _logger.LogWarning("Category with this name already exists: {CategoryName}", dto.Name);
                    return ServiceResult<CategoryDto>.Conflict("Category with this name already exists.");
                }

                var category = _mapper.Map<Category>(dto);

                await categoryRepo.AddAsync(category);
                await _unitOfWork.SaveChangesAsync();

                var mapped = _mapper.Map<CategoryDto>(category);
                _logger.LogInformation("Category created successfully: {CategoryId}", category.Id);
                return ServiceResult<CategoryDto>.Created(mapped, "Category created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category: {CategoryName}", dto.Name);
                return ServiceResult<CategoryDto>.ServerError("An error occurred while creating the category.");
            }
        }

        public async Task<ServiceResult> DeleteCategoryAsync(int id)
        {
            try
            {
                _logger.LogInformation("Deleting category: {CategoryId}", id);

                if (id <= 0)
                    return ServiceResult.Fail("Invalid category ID.", (int)HttpStatusCode.BadRequest);

                var categoryRepo = _unitOfWork.Repository<Category, int>();
                var category = await categoryRepo.GetByIdAsync(id);

                if (category is null)
                {
                    _logger.LogWarning("Category not found: {CategoryId}", id);
                    return ServiceResult.NotFound("Category not found.");
                }

                // Check if category has products (optional, based on business rules)
                if (category.Products?.Any() == true)
                {
                    _logger.LogWarning("Cannot delete category with existing products: {CategoryId}", id);
                    return ServiceResult.Fail("Cannot delete category with existing products.", 409);
                }

                categoryRepo.Delete(category);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Category deleted successfully: {CategoryId}", id);
                return ServiceResult.Ok("Category deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category: {CategoryId}", id);
                return ServiceResult.ServerError("An error occurred while deleting the category.");
            }
        }

        public async Task<ServiceResult<CategoryDto>> UpdateCategoryAsync(int id, UpdateCategoryDto dto)
        {
            try
            {
                _logger.LogInformation("Updating category {CategoryId} with name: {CategoryName}", id, dto.Name);

                if (id <= 0)
                    return ServiceResult<CategoryDto>.Fail("Invalid category ID.", 400);

                var categoryRepo = _unitOfWork.Repository<Category, int>();

                // Get the category with its current state
                var spec = new CategoryWithSpecification(id);
                var category = await categoryRepo.GetByIdWithSpecAsync(spec);

                if (category is null)
                {
                    _logger.LogWarning("Category not found for update: {CategoryId}", id);
                    return ServiceResult<CategoryDto>.NotFound("Category not found.");
                }

                // Check if name is being changed
                if (!string.Equals(category.Name, dto.Name, StringComparison.OrdinalIgnoreCase))
                {
                    // Check for duplicate name
                    var nameSpec = new CategoryWithSpecification(dto.Name);
                    var duplicateCategories = await categoryRepo.ListAllWithSpecAsync(nameSpec);
                    var duplicate = duplicateCategories.FirstOrDefault(c => c.Id != id);

                    if (duplicate != null)
                    {
                        _logger.LogWarning("Duplicate category name: {CategoryName}", dto.Name);
                        return ServiceResult<CategoryDto>.Conflict("Another category with the same name already exists.");
                    }
                }

                // Update properties
                category.Name = dto.Name;
                if (dto.ParentId is not null)
                {
                    var exist = await categoryRepo.ExistsAsync((int)dto.ParentId);
                    if (!exist)
                    {
                        _logger.LogWarning("Parent category not found: {ParentId}", dto.ParentId);
                        return ServiceResult<CategoryDto>.Fail("Parent Category not found.");
                    }
                }
                category.ParentId = dto.ParentId;

                categoryRepo.Update(category);
                await _unitOfWork.SaveChangesAsync();

                var mapped = _mapper.Map<CategoryDto>(category);
                _logger.LogInformation("Category updated successfully: {CategoryId}", id);
                return ServiceResult<CategoryDto>.Ok(mapped, "Category updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category {CategoryId}", id);
                return ServiceResult<CategoryDto>.ServerError("An error occurred while updating the category.");
            }
        }

        public async Task<ServiceResult<List<CategoryHierarchyDto>>> GetCategoryHierarchyAsync()
        {
            try
            {
                _logger.LogInformation("Getting category hierarchy");

                var categoryRepo = _unitOfWork.Repository<Category, int>();
                var categories = await categoryRepo.ListAllAsync();

                var rootCategories = categories.Where(c => !c.ParentId.HasValue).ToList();
                var hierarchy = new List<CategoryHierarchyDto>();

                foreach (var root in rootCategories)
                {
                    hierarchy.Add(BuildCategoryTree(root, categories));
                }

                _logger.LogInformation("Category hierarchy retrieved with {Count} root categories", rootCategories.Count);
                return ServiceResult<List<CategoryHierarchyDto>>.Ok(hierarchy, "Category hierarchy fetched.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting category hierarchy");
                return ServiceResult<List<CategoryHierarchyDto>>.ServerError(
                    "An error occurred while fetching category hierarchy.");
            }
        }

        private CategoryHierarchyDto BuildCategoryTree(Category category, IEnumerable<Category> allCategories)
        {
            var dto = _mapper.Map<CategoryHierarchyDto>(category);

            var children = allCategories.Where(c => c.ParentId == category.Id).ToList();
            foreach (var child in children)
            {
                dto.Children.Add(BuildCategoryTree(child, allCategories));
            }

            return dto;
        }
    }
}