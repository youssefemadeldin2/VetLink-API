using System.ComponentModel.DataAnnotations;
using VetLink.Services.Services.AccountService.Dtos.BuyerDtos;

namespace VetLink.Services.Services.AccountService.Dtos.SellerDtos
{
    public class RegisterAsSellerDto : RegisterAsBuyerDto
	{
		[Required]
		public string StoreName { get; set; } = string.Empty;

		public string StoreDescription { get; set; } = string.Empty;

		[Required]
		public string CommercialRegisterURL { get; set; } = string.Empty;

		[Required]
		public string PracticeLicenseURL { get; set; } = string.Empty;

		public string? StoreLogoURL { get; set; }
	}
}
