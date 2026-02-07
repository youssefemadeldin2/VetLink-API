namespace VetLink.Services.Services.AccountService.Dtos.AdminDtos
{
    public class AdminDashboardDto
    {
        public int TotalUsers { get; set; }
        public int TotalSellers { get; set; }
        public int TotalBuyers { get; set; }
        public int PendingSellers { get; set; }
        public int ActiveSellers { get; set; }
        public int RejectedSellers { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public int TotalProducts { get; set; }
    }
}
