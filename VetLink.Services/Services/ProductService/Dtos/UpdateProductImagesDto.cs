using Microsoft.AspNetCore.Http;

namespace VetLink.Services.Services.ProductService.Dtos
{
    public class UpdateProductImagesDto
    {
		public int ProductId { get; set; }
		public List<IFormFile> Images { get; set; } = new();
        public int IsPrimary { get; set; } = 0;
		public int ImagesCountToUpdate { get; set; }
	}

}
