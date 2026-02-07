using System.ComponentModel.DataAnnotations;

namespace VetLink.Services.Services.BrandService.Dtos
{
    public class UpdateBrandDto
	{
		[Required(ErrorMessage = "Brand name is required")]
		[StringLength(100, MinimumLength = 2, ErrorMessage = "Brand name must be between 2 and 100 characters")]
		public string Name { get; set; }
	}
}
