using System.ComponentModel.DataAnnotations;
using VetLink.Data.Enums;

namespace VetLink.Services.Services.OrderService.Dtos
{
    public class OrderActivationRequest
	{
		public int BuyerId { get; set; }

		[Required, Phone]
		public string PhoneNumber { get; set; }

		[Required]
		public string Country { get; set; }

		public PaymentMethod PaymentMethod { get; set; }

		[Required]
		public string City { get; set; }

		[Required]
		public string PostalCode { get; set; }

		[Required]
		public string Street { get; set; }
	}
}
