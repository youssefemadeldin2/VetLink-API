using Microsoft.AspNetCore.Mvc;
using VetLink.WebApi.Helpers;
using VetLink.Repository.Specifications.Paginated;
using VetLink.Services.Helper;
using VetLink.Services.Services.ProductService;
using VetLink.Services.Services.ProductService.Dtos;
using VetLink.Services.Services.ReviewService;
using VetLink.Services.Services.ReviewService.Dtos;

namespace VetLink.WebApi.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/products")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IReviewService _reviewService;

        public ProductController(IProductService productService, IReviewService reviewService)
        {
            _productService = productService;
            _reviewService = reviewService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllProducts(
            [FromQuery] string? search,
            [FromQuery] PaginationSpecification pagination)
        {
            var result = await _productService.GetAllProductsAsync(pagination, search);
            return new ResultActionResult<PaginatedResultDto<ProductProfileDto>>(result);
        }

        [HttpGet("{productId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProductDetails(int productId)
        {
            var result = await _productService.GetProductDetailsAsync(productId);
            return new ResultActionResult<ProductDetailsDto>(result);
        }

        [HttpPost]
        //[Authorize(Roles = "Seller")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto dto)
        {
            var result = await _productService.CreateProductAsync(dto);
            return new ResultActionResult<ProductDetailsDto>(result);
        }

        [HttpPut]
        //[Authorize(Roles = "Seller")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductDto dto)
        {
            var result = await _productService.UpdateProductAsync(dto);
            return new ResultActionResult<ProductDetailsDto>(result);
        }

        [HttpDelete("{productId:int}")]
        //[Authorize(Roles = "Seller")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            var result = await _productService.DeleteProductAsync(productId);
            return new ResultActionResult(result);
        }

        [HttpPut("images")]
        //[Authorize(Roles = "Seller")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateProductImages([FromBody] UpdateProductImagesDto dto)
        {
            var result = await _productService.UpdateProductImagesAsync(dto);
            return new ResultActionResult(result);
        }

        [HttpPost("images")]
        //[Authorize(Roles = "Seller")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddImages([FromForm] AddProductImagesDto dto)
        {
            var result = await _productService.AddProductImagesAsync(dto);
            return new ResultActionResult<List<ImageDto>>(result);
        }

        [HttpPut("images/primary")]
        //[Authorize(Roles = "Seller")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ChangePrimaryImage([FromBody] ChangePrimaryImageDto dto)
        {
            var result = await _productService.ChangePrimaryImageAsync(dto);
            return new ResultActionResult(result);
        }

        [HttpDelete("images")]
        //[Authorize(Roles = "Seller")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> DeleteImage([FromBody] DeleteProductImageDto dto)
        {
            var result = await _productService.DeleteProductImageAsync(dto);
            return new ResultActionResult(result);
        }

        [HttpPost("reviews")]
        //[Authorize(Roles = "Buyer")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> AddReview([FromBody] AddReviewDto dto)
        {
            var userIdClaim = User.FindFirst("uid")?.Value;
            if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return new ResultActionResult<ShowReviewDto>(
                    ServiceResult<ShowReviewDto>.Unauthorized("Invalid user ID"));
            }

            dto.BuyerId = userId;
            var result = await _reviewService.AddReviewAsync(dto);
            return new ResultActionResult<ShowReviewDto>(result);
        }

        [HttpGet("{productId:int}/reviews")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProductReviews(int productId)
        {
            var result = await _reviewService.GetAllReviewsAsync(productId);
            return new ResultActionResult<List<ShowReviewDto>>(result);
        }

        [HttpGet("{productId:int}/images")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProductImages(int productId)
        {
            var productResult = await _productService.GetProductDetailsAsync(productId);
            if (!productResult.Success || productResult.Data == null)
            {
                return new ResultActionResult<List<ImageDto>>(
                    ServiceResult<List<ImageDto>>.NotFound("Product not found"));
            }

            var images = productResult.Data.Images ?? new List<ImageDto>();

            return new ResultActionResult<List<ImageDto>>(
                ServiceResult<List<ImageDto>>.Ok(images,
                    images.Any()
                        ? "Product images retrieved successfully"
                        : "No images found for this product"));
        }

        [HttpPut("{productId:int}/activate")]
        //[Authorize(Roles = "Admin,Seller")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ActivateProduct(int productId)
        {
            var updateDto = new UpdateProductDto
            {
                Id = productId,
                IsActive = true
            };
            var result = await _productService.UpdateProductAsync(updateDto);
            return new ResultActionResult<ProductDetailsDto>(result);
        }

        [HttpPut("{productId:int}/deactivate")]
        //[Authorize(Roles = "Admin,Seller")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeactivateProduct(int productId)
        {
            var updateDto = new UpdateProductDto
            {
                Id = productId,
                IsActive = false
            };
            var result = await _productService.UpdateProductAsync(updateDto);
            return new ResultActionResult<ProductDetailsDto>(result);
        }
    }
}