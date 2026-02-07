using AutoMapper;
using Microsoft.Extensions.Logging;
using VetLink.Data.Entities;
using VetLink.Repository.Interfaces;
using VetLink.Repository.Specifications.ReviewsSpecifications;
using VetLink.Services.Helper;
using VetLink.Services.Services.ReviewService.Dtos;

namespace VetLink.Services.Services.ReviewService
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ReviewService> _logger;

        public ReviewService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ReviewService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ServiceResult<ShowReviewDto>> AddReviewAsync(AddReviewDto review)
        {
            try
            {
                _logger.LogInformation("Adding review for product ID: {ProductId} by user ID: {UserId}",
                    review.ProductId, review.BuyerId);

                var reviewRepo = _unitOfWork.Repository<Review, int>();
                var userRepo = _unitOfWork.Repository<User, int>();
                var productRepo = _unitOfWork.Repository<Product, int>();

                var existUser = await userRepo.ExistsAsync(review.BuyerId);
                if (!existUser)
                {
                    _logger.LogWarning("User not found: {UserId}", review.BuyerId);
                    return ServiceResult<ShowReviewDto>.NotFound("User not found");
                }

                var existProduct = await productRepo.ExistsAsync(review.ProductId);
                if (!existProduct)
                {
                    _logger.LogWarning("Product not found: {ProductId}", review.ProductId);
                    return ServiceResult<ShowReviewDto>.NotFound("Product not found");
                }

                var specs = new ReviewWithSpecs(review.ProductId, review.BuyerId);
                var existingReviews = await reviewRepo.ListAllWithSpecAsync(specs);

                if (existingReviews.Count >= 1)
                {
                    _logger.LogWarning("User {UserId} already reviewed product {ProductId}",
                        review.BuyerId, review.ProductId);
                    return ServiceResult<ShowReviewDto>.Conflict("Cannot add more than one review per product");
                }

                if (review.Rating < 1 || review.Rating > 5)
                {
                    return ServiceResult<ShowReviewDto>.Fail("Rating must be between 1 and 5", 400);
                }

                var newReview = new Review
                {
                    BuyerId = review.BuyerId,
                    Comment = review.Comment?.Trim(),
                    CreatedAt = DateTime.UtcNow,
                    ProductId = review.ProductId,
                    Rating = (byte)review.Rating,
                    ShowName = review.ShowName
                };

                var addedReview = await reviewRepo.AddAsync(newReview);
                await _unitOfWork.SaveChangesAsync();

                var mapped = _mapper.Map<ShowReviewDto>(addedReview);

                _logger.LogInformation("Review added successfully: {ReviewId}", addedReview.Id);
                return ServiceResult<ShowReviewDto>.Created(mapped, "Review added successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding review for product {ProductId} by user {UserId}",
                    review.ProductId, review.BuyerId);
                return ServiceResult<ShowReviewDto>.ServerError("An error occurred while adding the review");
            }
        }

        public async Task<ServiceResult> DeleteReviewAsync(int reviewId)
        {
            try
            {
                _logger.LogInformation("Deleting review: {ReviewId}", reviewId);

                var reviewRepo = _unitOfWork.Repository<Review, int>();
                var review = await reviewRepo.GetByIdAsync(reviewId);

                if (review == null)
                {
                    _logger.LogWarning("Review not found: {ReviewId}", reviewId);
                    return ServiceResult.NotFound("Review not found");
                }

                reviewRepo.Delete(review);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Review deleted successfully: {ReviewId}", reviewId);
                return ServiceResult.Ok("Review deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting review: {ReviewId}", reviewId);
                return ServiceResult.ServerError("An error occurred while deleting the review");
            }
        }

        public async Task<ServiceResult<List<ShowReviewDto>>> GetAllReviewsAsync(int productId)
        {
            try
            {
                _logger.LogInformation("Getting reviews for product ID: {ProductId}", productId);

                var reviewRepo = _unitOfWork.Repository<Review, int>();
                var productRepo = _unitOfWork.Repository<Product, int>();

                var existProduct = await productRepo.ExistsAsync(productId);
                if (!existProduct)
                {
                    _logger.LogWarning("Product not found: {ProductId}", productId);
                    return ServiceResult<List<ShowReviewDto>>.NotFound("Product not found");
                }

                var specs = new ReviewWithSpecs(productId);
                var reviews = await reviewRepo.ListAllWithSpecAsync(specs);

                if (!reviews.Any())
                {
                    _logger.LogInformation("No reviews found for product: {ProductId}", productId);
                    return ServiceResult<List<ShowReviewDto>>.Ok(new List<ShowReviewDto>(), "No reviews found");
                }

                var mapped = _mapper.Map<List<ShowReviewDto>>(reviews);

                _logger.LogInformation("Retrieved {Count} reviews for product: {ProductId}",
                    mapped.Count, productId);
                return ServiceResult<List<ShowReviewDto>>.Ok(mapped, "Reviews retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting reviews for product: {ProductId}", productId);
                return ServiceResult<List<ShowReviewDto>>.ServerError(
                    "An error occurred while retrieving reviews");
            }
        }

        public async Task<ServiceResult<ShowReviewDto>> UpdateReviewAsync(UpdateReviewDto review)
        {
            try
            {
                _logger.LogInformation("Updating review: {ReviewId}", review.Id);

                var reviewRepo = _unitOfWork.Repository<Review, int>();
                var existingReview = await reviewRepo.GetByIdAsync(review.Id);

                if (existingReview == null)
                {
                    _logger.LogWarning("Review not found: {ReviewId}", review.Id);
                    return ServiceResult<ShowReviewDto>.NotFound("Review not found");
                }

                if (existingReview.BuyerId != review.BuyerId)
                {
                    _logger.LogWarning("User {UserId} does not own review {ReviewId}",
                        review.BuyerId, review.Id);
                    return ServiceResult<ShowReviewDto>.Forbidden("You can only update your own reviews");
                }

                if (review.Rating < 1 || review.Rating > 5)
                {
                    return ServiceResult<ShowReviewDto>.Fail("Rating must be between 1 and 5", 400);
                }

                existingReview.Comment = review.Comment?.Trim();
                existingReview.Rating = (byte)review.Rating;
                existingReview.ShowName = review.ShowName;

                reviewRepo.Update(existingReview);
                await _unitOfWork.SaveChangesAsync();

                var mapped = _mapper.Map<ShowReviewDto>(existingReview);

                _logger.LogInformation("Review updated successfully: {ReviewId}", review.Id);
                return ServiceResult<ShowReviewDto>.Ok(mapped, "Review updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating review: {ReviewId}", review.Id);
                return ServiceResult<ShowReviewDto>.ServerError("An error occurred while updating the review");
            }
        }
    }
}