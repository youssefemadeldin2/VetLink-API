using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using VetLink.Data.Entities;
using VetLink.Repository.Interfaces;
using VetLink.Services.Hubs;

namespace VetLink.Services.Services.Notifications
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hub;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        public NotificationService(IHubContext<NotificationHub> hub,
                                    IUnitOfWork unitOfWork,
                                    UserManager<User> userManager,
                                    RoleManager<Role> roleManager)
        {
            _hub = hub;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task NotifyAdminsNewSellerAsync(int sellerId, string storeName)
        {
            var message = $"New seller registered: {storeName} (ID: {sellerId})";
            var admins = await _userManager.GetUsersInRoleAsync("Admin");

            foreach (var admin in admins)
            {
                var notification = new Notification
                {
                    UserId = admin.Id,
                    Message = message,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Repository<Notification, int>().AddAsync(notification);

                await _hub.Clients.Group("ROLE_ADMIN").SendAsync("ReceiveNotification", new
                {
                    Id = notification.Id,
                    Message = notification.Message,
                    CreatedAt = notification.CreatedAt
                });
            }

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task NotifyUserAsync(int userId, string message, string? link = null)
        {
            var notification = new Notification
            {
                UserId = userId,
                Message = message,
                Link = link,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Notification, int>().AddAsync(notification);
            await _unitOfWork.SaveChangesAsync();

            await _hub.Clients.Group($"USER_{userId}").SendAsync("ReceiveNotification", new
            {
                notification.Id,
                notification.Message,
                notification.CreatedAt,
                notification.Link
            });
        }
    }
}