using VetLink.Data.Enums;

namespace VetLink.Services.Services.AccountService.AuditLogService
{
    public interface IAuditLogService
    {
		void Log(string UserEmail, AuditAction action, string details);
	}

}
