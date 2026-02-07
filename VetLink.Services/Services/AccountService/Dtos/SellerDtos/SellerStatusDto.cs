using VetLink.Data.Enums;

namespace VetLink.Services.Services.AccountService.Dtos.SellerDtos
{
    public class SellerStatusDto
	{
		public AccountStatus Status { get; set; }
		public string? RejectionReason { get; set; }
		public DateTime? RejectedAt { get; set; }
	}
}
