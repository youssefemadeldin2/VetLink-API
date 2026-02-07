using System.Text.Json.Serialization;

namespace VetLink.Services.Services.AccountService.Dtos.BuyerDtos
{
	using System.Text.Json.Serialization;

	public class PendingRegistration
	{
		[JsonPropertyName("email")]
		public string Email { get; set; }

		[JsonPropertyName("fullName")]
		public string FullName { get; set; }

		[JsonPropertyName("phoneNumber")]
		public string PhoneNumber { get; set; }

		[JsonPropertyName("password")]
		public string Password { get; set; }

		[JsonPropertyName("role")]
		public string Role { get; set; }

		[JsonPropertyName("extraData")]
		public string ExtraData { get; set; }

		[JsonPropertyName("createdAt")]
		public DateTime CreatedAt { get; set; }
	}

}
