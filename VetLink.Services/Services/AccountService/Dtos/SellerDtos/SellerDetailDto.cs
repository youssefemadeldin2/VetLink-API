namespace VetLink.Services.Services.AccountService.Dtos.SellerDtos
{
    public class SellerDetailDto : SellerDto
	{
		public string CommercialRegisterURL { get; set; } = string.Empty;
		public string PracticeLicenseURL { get; set; } = string.Empty;
		public string RejectionReason { get; set; } = string.Empty;
		public List<AuditLogDto> AuditLogs { get; set; } = new();
	}
}
