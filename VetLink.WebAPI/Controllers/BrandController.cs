using Microsoft.AspNetCore.Mvc;
using VetLink.WebApi.Helpers;
using VetLink.Repository.Specifications.Paginated;
using VetLink.Services.Helper;
using VetLink.Services.Services.BrandService;
using VetLink.Services.Services.BrandService.Dtos;
using VetLink.Services.Services.ProductService.Dtos;

namespace VetLink.WebApi.Controllers
{
	[ApiController]
	[ApiVersion("1.0")]
	[Route("api/v{version:apiVersion}/brands")]
	[Produces("application/json")]
	[Consumes("application/json")]
	public class BrandController : ControllerBase
	{
		private readonly IBrandService _brandService;

		public BrandController(IBrandService service)
		{
			_brandService = service;
		}

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> GetAllBrands(
			[FromQuery] string? search,
			[FromQuery] PaginationSpecification pagination)
		{
			if (pagination.PageIndex < 1)
				return BadRequest(new { message = "Page must be greater than 0" });

			if (pagination.PageSize < 1 || pagination.PageSize > 100)
				return BadRequest(new { message = "Page size must be between 1 and 100" });

			var result = await _brandService.GetAllBrandsAsync(search, pagination);
			return new ResultActionResult<PaginatedResultDto<BrandDto>>(result);
		}

		[HttpGet("{id:int}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> GetBrandById(int id)
		{
			if (id <= 0)
				return BadRequest(new { message = "Id must be a positive integer" });

			var result = await _brandService.GetBrandByIdAsync(id);
			return new ResultActionResult<BrandWithProductsDto>(result);
		}

		[HttpPost]
		//[Authorize(Policy = "AdminOnly")]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status409Conflict)]
		public async Task<IActionResult> CreateBrand([FromBody] CreateBrandDto dto)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var result = await _brandService.CreateBrandAsync(dto.Name);
			return new ResultActionResult<BrandDto>(result);
		}

		[HttpPut("{id:int}")]
		//[Authorize(Policy = "AdminOnly")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status409Conflict)]
		public async Task<IActionResult> UpdateBrand(int id, [FromBody] UpdateBrandDto dto)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			if (id <= 0)
				return BadRequest(new { message = "Id must be a positive integer" });

			var result = await _brandService.UpdateBrandAsync(id, dto.Name);
			return new ResultActionResult<BrandDto>(result);
		}

		[HttpDelete("{id:int}")]
		//[Authorize(Policy = "AdminOnly")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> DeleteBrand(int id)
		{
			if (id <= 0)
				return BadRequest(new { message = "Id must be a positive integer" });

			var result = await _brandService.DeleteBrandAsync(id);
			return new ResultActionResult(result);
		}
	}
}