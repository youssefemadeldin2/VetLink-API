namespace VetLink.Services.Services.Notifications
{
    public interface INotificationService
    {
		Task NotifyAdminsNewSellerAsync(int sellerUserId, string storeName);
		Task NotifyUserAsync(int userId, string message, string? link = null);
	}

}
