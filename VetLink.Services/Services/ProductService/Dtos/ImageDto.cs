namespace VetLink.Services.Services.ProductService.Dtos
{
    public class ImageDto
	{
		public int Id { get; set; }
		public string ImageUrl { get; set; } = string.Empty;
		//public string ThumbnailUrl { get; set; } = string.Empty;
		public bool IsPrimary { get; set; }
    }

}
