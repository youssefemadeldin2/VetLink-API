using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using VetLink.Data.Enums;

namespace VetLink.Repository.Specifications.UserSpecifications
{
    public class UserSpecification
    {
		public string FullName { get; set; }
		public string Email { get; set; }
		public string PasswordHash { get; set; }
		public string PhoneNumber { get; set; }
		public UserRole Role { get; set; } = UserRole.buyer;
        public string Passwoerd { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	}
}
