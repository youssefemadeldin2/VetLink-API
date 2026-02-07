using VetLink.Services.Helper;
using VetLink.Services.Services.ReviewService.Dtos;

namespace VetLink.Services.Services.ReviewService
{
    public interface IReviewService
    {
        Task<ServiceResult<ShowReviewDto>> AddReviewAsync(AddReviewDto review);
        Task<ServiceResult<ShowReviewDto>> UpdateReviewAsync(UpdateReviewDto review);
        Task<ServiceResult> DeleteReviewAsync(int reviewId);
        Task<ServiceResult<List<ShowReviewDto>>> GetAllReviewsAsync(int productId);
    }
}