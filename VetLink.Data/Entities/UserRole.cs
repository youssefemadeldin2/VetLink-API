using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace VetLink.Data.Entities
{
	public class UserRole : IdentityUserRole<int>
	{
		public virtual User User { get; set; } = null!;
		public virtual Role Role { get; set; } = null!;
	}
}
