using VetLink.Services.Services.ProductService.Dtos;

namespace VetLink.Services.Services.BrandService.Dtos
{
    public class BrandWithProductsDto : BrandDto
    {
        public IReadOnlyList<BrandProductDto> Products { get; set; } = new List<BrandProductDto>();
    }
}
