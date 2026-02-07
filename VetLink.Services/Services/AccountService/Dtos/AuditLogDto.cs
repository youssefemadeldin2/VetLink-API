namespace VetLink.Services.Services.AccountService.Dtos
{
    public class AuditLogDto
	{
		public int Id { get; set; }
		public int UserId { get; set; }
		public string UserEmail { get; set; } = string.Empty;
		public string Action { get; set; } = string.Empty;
		public string Details { get; set; } = string.Empty;
		public string IpAddress { get; set; } = string.Empty;
		public string UserAgent { get; set; } = string.Empty;
		public DateTime CreatedAt { get; set; }
	}
}
