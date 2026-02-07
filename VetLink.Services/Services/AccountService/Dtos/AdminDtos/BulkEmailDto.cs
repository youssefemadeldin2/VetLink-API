namespace VetLink.Services.Services.AccountService.Dtos.AdminDtos
{
    public class BulkEmailDto
	{
		public string Subject { get; set; } = string.Empty;
		public string Body { get; set; } = string.Empty;
		public List<string> RecipientTypes { get; set; } = new(); // "All", "Buyers", "Sellers", "PendingSellers"
		public List<int>? SpecificUserIds { get; set; }
	}
}
