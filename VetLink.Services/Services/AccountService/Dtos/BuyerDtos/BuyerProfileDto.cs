using VetLink.Services.Helper;
using VetLink.Services.Services.OrderService.Dtos;

namespace VetLink.Services.Services.AccountService.Dtos.BuyerDtos
{
    // Buyer-specific DTOs
    public class BuyerProfileDto
	{
		public int Id { get; set; }
		public string Email { get; set; } = string.Empty;
		public string FullName { get; set; } = string.Empty;
		public string PhoneNumber { get; set; } = string.Empty;
		public string Role { get; } = "Buyer";
        public DateTime CreatedAt { get; set; }
		public PaginatedResultDto<BuyerOrderDto> Orders { get; set; }
	}
}
