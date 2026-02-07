namespace VetLink.Services.Services.AccountService.Dtos.SellerDtos
{
    public class PendingSellerDto
    {
        public int UserId { get; set; }
        public string Email { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string StoreName { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
