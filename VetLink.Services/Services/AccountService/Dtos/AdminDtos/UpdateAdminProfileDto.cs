using System.ComponentModel.DataAnnotations;

namespace VetLink.Services.Services.AccountService.Dtos.AdminDtos
{
    public class UpdateAdminProfileDto
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }
    }
}
