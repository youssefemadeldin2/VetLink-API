namespace VetLink.Services.Services.AccountService.Dtos.SellerDtos
{
    public class ReapplySellerDto
	{
		public string Email { get; set; } = string.Empty;
		public string UpdatedCommercialRegisterURL { get; set; } = string.Empty;
		public string UpdatedPracticeLicenseURL { get; set; } = string.Empty;
		public string? AdditionalNotes { get; set; }
	}
}
