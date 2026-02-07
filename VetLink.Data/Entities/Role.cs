using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using VetLink.Data.Enums;

namespace VetLink.Data.Entities
{
    public class Role:IdentityRole<int>
    {
		public DateTime CreatedAt { get; set; } = DateTime.Now;
		public string? Description { get; set; }
	}
}
