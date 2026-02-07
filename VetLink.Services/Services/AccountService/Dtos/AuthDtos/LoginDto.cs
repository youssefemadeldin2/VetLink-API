using System.ComponentModel.DataAnnotations;

namespace VetLink.Services.Services.AccountService.Dtos.AuthDtos
{
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "admin@vetlink.com";

        [Required]
        public string Password { get; set; } = "|VetLinkAdmin@VetLink.Com2026-";

	}
}
