using VetLink.Data.Enums;

namespace VetLink.Services.Services.AccountService.Dtos
{
    public class UserDto
    {
		public int Id { get; set; }
		public string Email { get; set; }
		public string FullName { get; set; }
		public string PhoneNumber { get; set; }
		public string Status { get; set; }
		public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
		public DateTime? EmailVerifiedAt { get; set; }

	}
}