using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace VetLink.Services.Hubs
{
	[Authorize]
	public class NotificationHub : Hub
	{
		public override async Task OnConnectedAsync()
		{
			var userId = Context.UserIdentifier;
			if (Context.User.IsInRole("Admin"))
			{
				await Groups.AddToGroupAsync(
					Context.ConnectionId,
					"ROLE_ADMIN"
				);
			}

			if (!string.IsNullOrEmpty(userId))
			{
				await Groups.AddToGroupAsync(
					Context.ConnectionId,
					$"USER_{userId}"
				);
			}

			await base.OnConnectedAsync();
		}

		public override async Task OnDisconnectedAsync(Exception? exception)
		{
			var userId = Context.UserIdentifier;

			if (!string.IsNullOrEmpty(userId))
			{
				await Groups.RemoveFromGroupAsync(
					Context.ConnectionId,
					$"USER_{userId}"
				);
			}

			await base.OnDisconnectedAsync(exception);
		}
	}
}
