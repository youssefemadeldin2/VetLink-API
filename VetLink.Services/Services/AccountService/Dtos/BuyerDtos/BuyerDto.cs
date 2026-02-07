namespace VetLink.Services.Services.AccountService.Dtos.BuyerDtos
{
    public class BuyerDto
	{
		public int Id { get; set; }
		public string Email { get; set; } = string.Empty;
		public string FullName { get; set; } = string.Empty;
		public string PhoneNumber { get; set; } = string.Empty;
		public DateTime CreatedAt { get; set; }
		public int TotalOrders { get; set; }
		public decimal TotalSpent { get; set; }
	}
}
