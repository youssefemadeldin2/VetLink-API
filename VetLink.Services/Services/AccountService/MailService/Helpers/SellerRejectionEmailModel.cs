using VetLink.Services.Services.AccountService.MailService.Dtos;

namespace VetLink.Services.Services.Email
{
    public class SellerRejectionEmailModel : EmailModel
	{
		public string Reason { get; set; }
	}
}