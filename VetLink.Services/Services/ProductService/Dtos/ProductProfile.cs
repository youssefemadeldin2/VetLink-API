using AutoMapper;
using VetLink.Data.Entities;
using VetLink.Services.Services.CategoryService.Dtos;
using VetLink.Services.Services.ProductService.Dtos;
using VetLink.Services.Services.ReviewService.Dtos;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<Product, ProductProfileDto>()
            .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.Name, o => o.MapFrom(s => s.Name))
            .ForMember(d => d.Description, o => o.MapFrom(s => s.Description))
            .ForMember(d => d.Price, o => o.MapFrom(s => s.Price))
            .ForMember(d => d.IsActive, o => o.MapFrom(s => s.IsActive))
            .ForMember(d => d.IsFeatured, o => o.MapFrom(s => s.IsFeatured))
            .ForMember(d => d.UniteInStock, o => o.MapFrom(s => s.StockQuantity))
            .ForMember(d => d.ImgUrl, o =>
                o.MapFrom(s =>
                    s.Images != null
                        ? s.Images
                            .Where(i => i.IsPrimary)
                            .Select(i => i.ImageURL)
                            .FirstOrDefault() ?? string.Empty
                        : string.Empty))
            .ForMember(d => d.SellerName, o =>
                o.MapFrom(s => s.Seller != null ? s.Seller.StoreName : "VetLink"));

        CreateMap<Product, ProductDetailsDto>()
            //.ForMember(d => d.Stats, o => o.MapFrom(s => s.ProductStats))
            .ForMember(d => d.Images, o => o.MapFrom(s => s.Images ?? new List<Image>()));

        CreateMap<ProductState, ProductStatsDto>();
        CreateMap<Image, ImageDto>();
        CreateMap<Brand, BrandDto>();
        CreateMap<Review, ShowReviewDto>();
        CreateMap<Category, CategoryDto>();

        CreateMap<UpdateProductDto, Product>()
            .ForMember(d => d.Images, o => o.Ignore())
            .ForMember(d => d.Id, o => o.Ignore())
            .ForAllMembers(o =>
                o.Condition((src, dest, srcMember) => srcMember != null));
    }
}
