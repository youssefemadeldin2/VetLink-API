namespace VetLink.Services.Services.AccountService.Dtos.SellerDtos
{
    // Admin-specific DTOs
    public class SellerDto : SellerProfileDto
	{
		public int TotalProducts { get; set; }
		public int TotalOrders { get; set; }
		public decimal TotalRevenue { get; set; }
	}
}
