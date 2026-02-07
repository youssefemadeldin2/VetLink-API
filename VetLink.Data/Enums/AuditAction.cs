using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VetLink.Data.Enums
{
	public enum AuditAction
	{
		BUYER_REGISTERED,
		SELLER_REGISTERED,
		OTP_SENT,
		EMAIL_VERIFIED,
		LOGIN_FAILED,
		LOGIN_BLOCKED,
		ACCOUNT_LOCKED,
		LOGIN_SUCCESS,
		SELLER_APPROVED,
		SELLER_REJECTED,
		REGISTRATION_STARTED,
		OTP_RESENT,
		EMAIL_SEND_FAILED
	}

}
