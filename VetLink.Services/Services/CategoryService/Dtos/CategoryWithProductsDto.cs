namespace VetLink.Services.Services.CategoryService.Dtos
{
    public class CategoryWithProductsDto : CategoryDto
	{
		public int ProductsCount { get; set; }
		public IReadOnlyList<CategoryProductDto> Products { get; set; } = new List<CategoryProductDto>();
	}
}