using VetLink.Data.Enums;

namespace VetLink.Services.Services.AccountService.Dtos.SellerDtos
{
    // Seller-specific DTOs
    public class SellerProfileDto
	{
		public int Id { get; set; }
		public string Email { get; set; } = string.Empty;
		public string FullName { get; set; } = string.Empty;
		public string PhoneNumber { get; set; } = string.Empty;
		public string StoreName { get; set; } = string.Empty;
		public string StoreDescription { get; set; } = string.Empty;
		public string StoreLogoURL { get; set; } = string.Empty;
		public AccountStatus Status { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime? ApprovedAt { get; set; }
	}
	
}
