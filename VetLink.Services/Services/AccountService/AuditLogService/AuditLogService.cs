using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VetLink.Data.Entities;
using VetLink.Data.Enums;
using VetLink.Repository.Interfaces;

namespace VetLink.Services.Services.AccountService.AuditLogService
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IServiceScopeFactory _scopeFactory;
		private readonly ILogger<AuditLogService> _logger;
		public AuditLogService(IServiceScopeFactory scopeFactory, ILogger<AuditLogService> logger)
        {
            _scopeFactory = scopeFactory;
			_logger = logger;
		}

        public void Log(string UserEmail, AuditAction action, string details)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                    var log = new AuditLog
                    {
                        UserEmail = UserEmail,
                        Action = action,
                        Details = details,
                        CreatedAt = DateTime.UtcNow
                    };

                    await unitOfWork.Repository<AuditLog, int>().AddAsync(log);
                    await unitOfWork.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    // Log audit logging failure to prevent silent failures
                    // In production, consider using a separate logging mechanism
                    // or a message queue for critical audit logs
                    _logger.LogError(ex, "Failed to write audit log for {UserEmail}, Action: {Action}", UserEmail, action);
                }
            });
        }
    }


}
