using Microsoft.AspNetCore.Http;

namespace VetLink.Services.Services.ProductService.Dtos
{
    public class AddProductImagesDto
	{
		public int ProductId { get; set; }
		public List<IFormFile> Images { get; set; } = new();
		public int PrimaryIndex { get; set; } = -1;
	}

}
