using System.ComponentModel.DataAnnotations;

namespace VetLink.Services.Services.AccountService.Dtos.AdminDtos
{
    public class RejectSellerDto
    {
		[Required]
		public int SellerUserId { get; set; }

		[Required]
		[StringLength(500)]
		public string Reason { get; set; }
	}

}
