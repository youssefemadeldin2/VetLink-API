using AutoMapper;
using Microsoft.Extensions.Logging;
using VetLink.Data.Entities;
using VetLink.Repository.Interfaces;
using VetLink.Repository.Specifications.ImageSpecifications;
using VetLink.Repository.Specifications.Paginated;
using VetLink.Repository.Specifications.ProductSpecifications;
using VetLink.Services.Helper;
using VetLink.Services.Services.ImageService;
using VetLink.Services.Services.ProductService.Dtos;
using VetLink.Services.Services.ReviewService;
using VetLink.Services.Services.ReviewService.Dtos;

namespace VetLink.Services.Services.ProductService
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IReviewService _reviewService;
        private readonly IImageStorageService _imageStorageService;
        private readonly ILogger<ProductService> _logger;
        private const int MaxImages = 6;

        public ProductService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IImageStorageService imageStorageService,
            IReviewService reviewService,
            ILogger<ProductService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _imageStorageService = imageStorageService;
            _reviewService = reviewService;
            _logger = logger;
        }

        public async Task<ServiceResult<ProductDetailsDto>> GetProductDetailsAsync(int productId)
        {
            try
            {
                _logger.LogInformation("Getting product with ID: {ProductId}", productId);

                var repo = _unitOfWork.Repository<Product, int>();
                var specs = new ProductWithSpecs(productId);
                var product = await repo.GetByIdWithSpecAsync(specs);

                if (product == null)
                {
                    _logger.LogWarning("Product with ID {ProductId} not found", productId);
                    return ServiceResult<ProductDetailsDto>.NotFound($"Product with ID {productId} not found");
                }

                var mapped = _mapper.Map<ProductDetailsDto>(product);
                var reviewsResult = await _reviewService.GetAllReviewsAsync(productId);

                if (reviewsResult.Success && reviewsResult.Data != null)
                {
                    mapped.Reviews = _mapper.Map<List<ShowReviewDto>>(reviewsResult.Data);
                }

                _logger.LogInformation("Successfully retrieved product with ID: {ProductId}", productId);
                return ServiceResult<ProductDetailsDto>.Ok(mapped, "Product fetched successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product with ID: {ProductId}", productId);
                return ServiceResult<ProductDetailsDto>.ServerError("An error occurred while retrieving the product");
            }
        }

        public async Task<ServiceResult<PaginatedResultDto<ProductProfileDto>>> GetAllProductsAsync(
            PaginationSpecification pagination,
            string? search)
        {
            try
            {
                _logger.LogInformation("Getting products with search: {Search}, page: {Page}", search, pagination.PageIndex);

                var repo = _unitOfWork.Repository<Product, int>();
                var specs = new ProductWithSpecs(search, pagination);
                var products = await repo.ListAllWithSpecAsync(specs);

                var totalItems = await repo.CountWithSpecAsync(specs);
                var totalPages = (int)Math.Ceiling(totalItems / (double)pagination.PageSize);

                if (!products.Any())
                {
                    _logger.LogInformation("No products found with search: {Search}", search);
                    var emptyPaging = new PaginatedResultDto<ProductProfileDto>(
                        new List<ProductProfileDto>(),
                        pagination.PageIndex,
                        pagination.PageSize,
                        0,
                        0
                    );
                    return ServiceResult<PaginatedResultDto<ProductProfileDto>>.Ok(
                        emptyPaging,
                        "No products found"
                    );
                }

                var mapping = _mapper.Map<IReadOnlyList<ProductProfileDto>>(products);

                var paging = new PaginatedResultDto<ProductProfileDto>(
                    mapping,
                    pagination.PageIndex,
                    pagination.PageSize,
                    totalItems,
                    totalPages);

                _logger.LogInformation("Retrieved {Count} products", mapping.Count);
                return ServiceResult<PaginatedResultDto<ProductProfileDto>>.Ok(paging, "Products fetched successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting products with search: {Search}", search);
                return ServiceResult<PaginatedResultDto<ProductProfileDto>>.ServerError(
                    "An error occurred while retrieving products");
            }
        }

        public async Task<ServiceResult<ProductDetailsDto>> CreateProductAsync(CreateProductDto dto)
        {
            try
            {
                _logger.LogInformation("Creating new product: {ProductName}", dto.Name);

                if (dto == null)
                    return ServiceResult<ProductDetailsDto>.Fail("Invalid product data", 400);

                if (dto.Images != null && dto.Images.Count > MaxImages)
                    return ServiceResult<ProductDetailsDto>.Fail($"Max images allowed is {MaxImages}", 400);

                var repo = _unitOfWork.Repository<Product, int>();
                var imageRepo = _unitOfWork.Repository<Image, int>();

                var product = _mapper.Map<Product>(dto);
                product.CreatedAt = DateTime.UtcNow;
                product.UpdatedAt = DateTime.UtcNow;
                product.IsActive = false;
                product.SKU = $"PRD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..4].ToUpper()}";

                await repo.AddAsync(product);
                await _unitOfWork.SaveChangesAsync();

                if (dto.Images != null && dto.Images.Any())
                {
                    var uploads = await _imageStorageService.UploadImagesAsync(
                        dto.Images,
                        $"products/{product.SKU}");

                    var primaryIndex = dto.PrimaryImageIndex >= 0 && dto.PrimaryImageIndex < uploads.Count
                        ? dto.PrimaryImageIndex
                        : 0;

                    var images = uploads.Select((u, i) => new Image
                    {
                        ProductId = product.Id,
                        ImageURL = u.Url,
                        PublicId = u.PublicId,
                        IsPrimary = i == primaryIndex
                    }).ToList();

                    await imageRepo.AddRangeAsync(images);
                    await _unitOfWork.SaveChangesAsync();
                }

                var specs = new ProductWithSpecs(product.Id);
                var created = await repo.GetByIdWithSpecAsync(specs);
                var resultDto = _mapper.Map<ProductDetailsDto>(created);

                _logger.LogInformation("Product created successfully with ID: {ProductId}", product.Id);
                return ServiceResult<ProductDetailsDto>.Created(resultDto, "Product created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product: {ProductName}", dto?.Name);
                return ServiceResult<ProductDetailsDto>.ServerError("An error occurred while creating the product");
            }
        }

        public async Task<ServiceResult> DeleteProductAsync(int productId)
        {
            try
            {
                _logger.LogInformation("Deleting product with ID: {ProductId}", productId);

                var repo = _unitOfWork.Repository<Product, int>();
                var product = await repo.GetByIdAsync(productId);

                if (product == null)
                {
                    _logger.LogWarning("Product with ID {ProductId} not found", productId);
                    return ServiceResult.NotFound($"Product with ID {productId} not found");
                }

                repo.Delete(product);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Product deleted successfully with ID: {ProductId}", productId);
                return ServiceResult.Ok("Product deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product with ID: {ProductId}", productId);
                return ServiceResult.ServerError("An error occurred while deleting the product");
            }
        }

        public async Task<ServiceResult<ProductDetailsDto>> UpdateProductAsync(UpdateProductDto dto)
        {
            try
            {
                _logger.LogInformation("Updating product with ID: {ProductId}", dto.Id);

                var repo = _unitOfWork.Repository<Product, int>();
                var specs = new ProductWithSpecs(dto.Id);
                var product = await repo.GetByIdWithSpecAsync(specs);

                if (product == null)
                {
                    _logger.LogWarning("Product with ID {ProductId} not found for update", dto.Id);
                    return ServiceResult<ProductDetailsDto>.NotFound("Product not found");
                }

                _mapper.Map(dto, product);
                product.UpdatedAt = DateTime.UtcNow;

                repo.Update(product);
                await _unitOfWork.SaveChangesAsync();

                var updated = await repo.GetByIdWithSpecAsync(specs);
                var resultDto = _mapper.Map<ProductDetailsDto>(updated);

                _logger.LogInformation("Product updated successfully with ID: {ProductId}", dto.Id);
                return ServiceResult<ProductDetailsDto>.Ok(resultDto, "Product updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product with ID: {ProductId}", dto.Id);
                return ServiceResult<ProductDetailsDto>.ServerError("An error occurred while updating the product");
            }
        }

        public async Task<ServiceResult> UpdateProductImagesAsync(UpdateProductImagesDto dto)
        {
            try
            {
                _logger.LogInformation("Updating images for product ID: {ProductId}", dto.ProductId);

                var repo = _unitOfWork.Repository<Product, int>();
                var imageRepo = _unitOfWork.Repository<Image, int>();

                var specs = new ProductWithSpecs(dto.ProductId);
                var product = await repo.GetByIdWithSpecAsync(specs);

                if (product == null)
                    return ServiceResult.NotFound("Product not found");

                if (dto.Images == null || !dto.Images.Any())
                    return ServiceResult.Fail("No images provided", 400);

                if (dto.Images.Count > MaxImages)
                    return ServiceResult.Fail($"Max images allowed is {MaxImages}", 400);

                await using var transaction = await _unitOfWork.BeginTransactionAsync();

                try
                {
                    if (product.Images?.Any() == true)
                    {
                        var publicIds = product.Images
                            .Where(i => !string.IsNullOrEmpty(i.PublicId))
                            .Select(i => i.PublicId!)
                            .ToList();

                        if (publicIds.Any())
                            await _imageStorageService.DeleteImageAsync(publicIds);

                        foreach (var img in product.Images)
                            imageRepo.Delete(img);

                        await _unitOfWork.SaveChangesAsync();
                    }

                    var uploads = await _imageStorageService.UploadImagesAsync(
                        dto.Images,
                        $"products/{product.SKU}");

                    var primaryIndex = dto.IsPrimary >= 0 && dto.IsPrimary < uploads.Count
                        ? dto.IsPrimary
                        : 0;

                    var images = uploads.Select((u, i) => new Image
                    {
                        ProductId = product.Id,
                        ImageURL = u.Url,
                        PublicId = u.PublicId,
                        IsPrimary = i == primaryIndex
                    }).ToList();

                    await imageRepo.AddRangeAsync(images);
                    await _unitOfWork.SaveChangesAsync();
                    await transaction.CommitAsync();

                    _logger.LogInformation("Product images updated successfully for product ID: {ProductId}", dto.ProductId);
                    return ServiceResult.Ok("Product images updated successfully");
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating images for product ID: {ProductId}", dto.ProductId);
                return ServiceResult.ServerError("An error occurred while updating product images");
            }
        }

        public async Task<ServiceResult<List<ImageDto>>> AddProductImagesAsync(AddProductImagesDto dto)
        {
            try
            {
                _logger.LogInformation("Adding images to product ID: {ProductId}", dto.ProductId);

                var repo = _unitOfWork.Repository<Product, int>();
                var imageRepo = _unitOfWork.Repository<Image, int>();

                var specs = new ProductWithSpecs(dto.ProductId);
                var product = await repo.GetByIdWithSpecAsync(specs);

                if (product == null)
                    return ServiceResult<List<ImageDto>>.NotFound("Product not found");

                if (dto.Images == null || !dto.Images.Any())
                    return ServiceResult<List<ImageDto>>.Fail("No images provided", 400);

                var currentImagesCount = product.Images?.Count ?? 0;

                if (currentImagesCount + dto.Images.Count > MaxImages)
                    return ServiceResult<List<ImageDto>>.Fail(
                        $"Product cannot have more than {MaxImages} images", 400);

                if (dto.PrimaryIndex >= 0 && product.Images?.Any() == true)
                {
                    foreach (var img in product.Images)
                        img.IsPrimary = false;

                    foreach (var item in product.Images)
                        imageRepo.Update(item);

                    await _unitOfWork.SaveChangesAsync();
                }

                var uploads = await _imageStorageService.UploadImagesAsync(
                    dto.Images,
                    $"products/{product.SKU}");

                var primaryIndex = dto.PrimaryIndex >= 0 && dto.PrimaryIndex < uploads.Count
                    ? dto.PrimaryIndex
                    : -1;

                var newImages = uploads.Select((u, i) => new Image
                {
                    ProductId = product.Id,
                    ImageURL = u.Url,
                    PublicId = u.PublicId,
                    IsPrimary = i == primaryIndex
                }).ToList();

                if (currentImagesCount == 0 && primaryIndex == -1)
                    newImages.First().IsPrimary = true;

                var imgs = await imageRepo.AddRangeAsync(newImages);
                await _unitOfWork.SaveChangesAsync();
                var mapped = _mapper.Map<List<ImageDto>>(imgs);

                _logger.LogInformation("Added {Count} images to product ID: {ProductId}", dto.Images.Count, dto.ProductId);
                return ServiceResult<List<ImageDto>>.Ok(mapped, "Images added successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding images to product ID: {ProductId}", dto.ProductId);
                return ServiceResult<List<ImageDto>>.ServerError("An error occurred while adding images");
            }
        }

        public async Task<ServiceResult> ChangePrimaryImageAsync(ChangePrimaryImageDto dto)
        {
            try
            {
                _logger.LogInformation("Changing primary image for product ID: {ProductId}", dto.ProductId);

                var repo = _unitOfWork.Repository<Product, int>();
                var imageRepo = _unitOfWork.Repository<Image, int>();

                var specs = new ProductWithSpecs(dto.ProductId);
                var product = await repo.GetByIdWithSpecAsync(specs);

                if (product == null)
                    return ServiceResult.NotFound("Product not found");

                var targetImage = product.Images?.FirstOrDefault(i => i.Id == dto.ImageId);
                if (targetImage == null)
                    return ServiceResult.NotFound("Image not found");

                foreach (var img in product.Images!)
                    img.IsPrimary = false;

                targetImage.IsPrimary = true;

                foreach (var item in product.Images)
                    imageRepo.Update(item);

                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Primary image changed for product ID: {ProductId}", dto.ProductId);
                return ServiceResult.Ok("Primary image updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing primary image for product ID: {ProductId}", dto.ProductId);
                return ServiceResult.ServerError("An error occurred while changing primary image");
            }
        }

        public async Task<ServiceResult> DeleteProductImageAsync(DeleteProductImageDto dto)
        {
            try
            {
                _logger.LogInformation("Deleting image {ImageId} from product ID: {ProductId}",
                    dto.ImageId, dto.ProductId);

                var imageRepo = _unitOfWork.Repository<Image, int>();
                var specs = new ImageWithSpecs(dto.ProductId);
                var productImages = await imageRepo.ListAllWithSpecAsync(specs);

                var imagesList = productImages.ToList();
                var image = imagesList.FirstOrDefault(i => i.Id == dto.ImageId);

                if (image == null)
                    return ServiceResult.NotFound("Image not found");

                if (imagesList.Count == 1 && image.IsPrimary)
                    return ServiceResult.Conflict("Cannot delete the only primary image for this product");

                await using var transaction = await _unitOfWork.BeginTransactionAsync();

                try
                {
                    // FIXED: Already has imageRepo.Update(newPrimary) - Critical Issue #3
                    if (image.IsPrimary)
                    {
                        var newPrimary = imagesList.FirstOrDefault(i => i.Id != image.Id);
                        if (newPrimary != null)
                        {
                            newPrimary.IsPrimary = true;
                            imageRepo.Update(newPrimary); // Already present
                        }
                    }

                    imageRepo.Delete(image);
                    await _unitOfWork.SaveChangesAsync(); // Already present - Critical Issue #3
                    await transaction.CommitAsync();

                    _logger.LogInformation("Image {ImageId} deleted from product ID: {ProductId}",
                        dto.ImageId, dto.ProductId);
                    return ServiceResult.Ok("Image deleted successfully");
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting image {ImageId} from product ID: {ProductId}",
                    dto.ImageId, dto.ProductId);
                return ServiceResult.ServerError("An error occurred while deleting the image");
            }
        }
    }
}