using VetLink.Data.Enums;

namespace VetLink.Services.Services.AccountService.Dtos.AdminDtos
{
    public class AdminProfileDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public AccountStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
