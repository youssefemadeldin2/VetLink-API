using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Identity;
using VetLink.Data.Enums;

namespace VetLink.Data.Entities
{
    public class User:IdentityUser<int>
    {
		public string FullName { get; set; } = null!;
		public AccountStatus Status { get; set; }

		public DateTime? EmailVerifiedAt { get; set; }
		public string? RejectionReason { get; set; }

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
		public DateTime? DeletedAt { get; set; }

		public string? RefreshToken { get; set; }
		public DateTime? RefreshTokenExpiresAt { get; set; }

		// Navigation properties
		public Seller? Seller { get; set; }

		public ICollection<Order> Orders { get; set; } = new List<Order>();
		public ICollection<Address> Addresses { get; set; } = new List<Address>();
		public ICollection<Review> Reviews { get; set; } = new List<Review>();
		public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
		public ICollection<Coupon> Coupons { get; set; } = new List<Coupon>();
		public ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();
		public ICollection<Return> Returns { get; set; } = new List<Return>();
		public ICollection<TicketReplay> Replays { get; set; } = new List<TicketReplay>();

		public ICollection<Conversation> ConversationsAsParticipant1 { get; set; } = new List<Conversation>();
		public ICollection<Conversation> ConversationsAsParticipant2 { get; set; } = new List<Conversation>();
		public ICollection<Message> Messages { get; set; } = new List<Message>();
		public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

	}
}
