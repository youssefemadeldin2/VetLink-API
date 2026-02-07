using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetLink.WebApi.Helpers;
using VetLink.Services.Helper;
using VetLink.Services.Services.ReviewService;
using VetLink.Services.Services.ReviewService.Dtos;

namespace VetLink.WebApi.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/reviews")]
    [Authorize]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet("product/{productId:int}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProductReviews(int productId)
        {
            var result = await _reviewService.GetAllReviewsAsync(productId);
            return new ResultActionResult<List<ShowReviewDto>>(result);
        }

        [HttpPost]
        [Authorize(Roles = "Buyer")]
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

        [HttpPut]
        [Authorize(Roles = "Buyer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateReview([FromBody] UpdateReviewDto dto)
        {
            var userIdClaim = User.FindFirst("uid")?.Value;
            if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return new ResultActionResult<ShowReviewDto>(
                    ServiceResult<ShowReviewDto>.Unauthorized("Invalid user ID"));
            }

            dto.BuyerId = userId;

            var result = await _reviewService.UpdateReviewAsync(dto);
            return new ResultActionResult<ShowReviewDto>(result);
        }

        [HttpDelete("{reviewId:int}")]
        [Authorize(Roles = "Buyer,Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            var result = await _reviewService.DeleteReviewAsync(reviewId);
            return new ResultActionResult(result);
        }

        // [HttpGet("user/{userId:int}")]
        // [Authorize]
        // public async Task<IActionResult> GetUserReviews(int userId)
        // {
        //     // You might want to add this method to IReviewService
        //     // var result = await _reviewService.GetUserReviewsAsync(userId);
        //     // return result.ToActionResult();

        //     return this.ToOkResult("Method not implemented yet");
        // }
    }
}