using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetLink.WebApi.Helpers;
using VetLink.Repository.Specifications.Paginated;
using VetLink.Services.Helper;
using VetLink.Services.Services.CategoryService;
using VetLink.Services.Services.CategoryService.Dtos;

namespace VetLink.WebApi.Controllers.CategoryControllers
{
	[ApiController]
	[ApiVersion("1.0")]
	[Route("api/v{version:apiVersion}/categories")] // Fixed: lowercase "categories" (plural)
	public class CategoryController : ControllerBase
	{
		private readonly ICategoryService _categoryService;

		public CategoryController(ICategoryService categoryService)
		{
			_categoryService = categoryService;
		}

		[HttpGet("{id:int}")]
		public async Task<IActionResult> GetCategoryById(int id)
		{
			if (id <= 0)
				return BadRequest(new { message = "Id must be a positive integer" });

			var result = await _categoryService.GetCategoryByIdAsync(id);
			return new ResultActionResult<CategoryWithProductsDto>(result);
		}

		[HttpGet]
		// [Cache(1)]
		public async Task<IActionResult> GetCategories(
			[FromQuery] string? search,
			[FromQuery] PaginationSpecification pagination)
		{
			if (pagination.PageIndex < 1)
				return BadRequest(new { message = "Page must be greater than 0" });

			if (pagination.PageSize < 1 || pagination.PageSize > 100)
				return BadRequest(new { message = "Page size must be between 1 and 100" });

			var result = await _categoryService.GetAllCategoriesAsync(search, pagination);
			return new ResultActionResult<PaginatedResultDto<CategoryDto>>(result);
		}

		[HttpGet("hierarchy")]
		public async Task<IActionResult> GetCategoryHierarchy()
		{
			var result = await _categoryService.GetCategoryHierarchyAsync();
			return new ResultActionResult<List<CategoryHierarchyDto>>(result);
		}

		[Authorize(Policy = "AdminOnly")]
		[HttpPost]
		public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto dto)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var result = await _categoryService.CreateCategoryAsync(dto);
			return new ResultActionResult<CategoryDto>(result);
		}

		[Authorize(Policy = "AdminOnly")]
		[HttpDelete("{id:int}")]
		public async Task<IActionResult> DeleteCategory(int id)
		{
			// FIXED: Changed from "Id" to "id" for consistency
			if (id <= 0)
				return BadRequest(new { message = "Id must be a positive integer" });

			var result = await _categoryService.DeleteCategoryAsync(id);
			return new ResultActionResult(result);
		}

		[Authorize(Policy = "AdminOnly")]
		[HttpPut("{id:int}")]
		public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryDto dto)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			if (id <= 0)
				return BadRequest(new { message = "Id must be a positive integer" });

			var result = await _categoryService.UpdateCategoryAsync(id, dto);
			return new ResultActionResult<CategoryDto>(result);
		}
	}
}