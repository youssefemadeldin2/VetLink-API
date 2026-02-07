namespace VetLink.Services.Services.CategoryService.Dtos
{
    public class CategoryProductDto
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string ProductCode { get; set; }
		public decimal Price { get; set; }
		public string ImageUrl { get; set; }
	}
}