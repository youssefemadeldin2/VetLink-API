using System.ComponentModel.DataAnnotations;

namespace VetLink.Services.Services.AccountService.Dtos.BuyerDtos
{
    public class RegisterAsBuyerDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string FullName { get; set; } = string.Empty;

        [Phone]
        public string PhoneNumber { get; set; } = null!;
    }
}
