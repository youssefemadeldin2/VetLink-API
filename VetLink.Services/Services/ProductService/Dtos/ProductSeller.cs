namespace VetLink.Services.Services.ProductService.Dtos
{
    public class ProductSeller
	{
		public int UserId { get; set; }
		public string StoreLogoURL { get; set; } = string.Empty;
		public string StoreName { get; set; } = string.Empty;
	}
}
