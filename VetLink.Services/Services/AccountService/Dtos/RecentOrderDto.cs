namespace VetLink.Services.Services.AccountService.Dtos
{
    // Common DTOs for lists
    public class RecentOrderDto
	{
		public int Id { get; set; }
		public string OrderNumber { get; set; } = string.Empty;
		public decimal Amount { get; set; }
		public string Status { get; set; } = string.Empty;
		public DateTime CreatedAt { get; set; }
	}
}
