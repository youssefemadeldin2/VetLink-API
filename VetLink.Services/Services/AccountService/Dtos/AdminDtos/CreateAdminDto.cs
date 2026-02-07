using System.ComponentModel.DataAnnotations;

namespace VetLink.Services.Services.AccountService.Dtos.AdminDtos
{
    public class CreateAdminDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [StringLength(15)]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        [MinLength(8)]
        public string Password { get; set; }
    }
}
