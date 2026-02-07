using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VetLink.Data.Entities;

namespace VetLink.Data.Contexts
{
    public class VetLinkDbContext : IdentityDbContext<User,Role,int>
    {
        #region Tables
        public DbSet<Address> Addresses { get; set; }//1
        public DbSet<AuditLog> AuditLogs { get; set; }//2
        public DbSet<Brand> Brands { get; set; }//3
        public DbSet<Category> Categories { get; set; }//4
        public DbSet<Conversation> Conversations { get; set; }//5
        public DbSet<Coupon> Coupons { get; set; }//6
        public DbSet<Image> Images { get; set; }//7
        public DbSet<Message> Messages { get; set; }//8
        public DbSet<Notification> Notifications{ get; set; }//9
        public DbSet<Order> Orders{ get; set; }//10
        public DbSet<OrderCoupons> OrderCoupons{ get; set; }//11
        public DbSet<OrderItem> OrderItems{ get; set; }//12
        public DbSet<PasswordResetToken> PasswordResetTokens{ get; set; }//13
        public DbSet<Product> Products{ get; set; }//14
        public DbSet<ProductState> productStates{ get; set; }//15
        public DbSet<Return> Returns{ get; set; }//16
        public DbSet<ReturnItem> ReturnItems{ get; set; }//17
        public DbSet<Review> Reviews{ get; set; }//18
        public DbSet<Seller> Sellers{ get; set; }//19
        public DbSet<Shipment> Shipments{ get; set; }//20
        public DbSet<SupportTicket> SupportTickets{ get; set; }//21
        public DbSet<TicketReplay> TicketReplays{ get; set; }//22
        public DbSet<Wishlist> Wishlists{ get; set; }//24
        #endregion
        public VetLinkDbContext(DbContextOptions options) : base(options){}
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
			

			modelBuilder.Entity<ReturnItem>(entity =>
			{
				entity.HasKey(e => new { e.ReturnId, e.ProductId });

				entity.HasOne(e => e.Return)
					  .WithMany(r => r.ReturnItems)
					  .HasForeignKey(e => e.ReturnId)
					  .OnDelete(DeleteBehavior.Restrict);

				entity.HasOne(e => e.Product)
					  .WithMany(p => p.ReturnItems)
					  .HasForeignKey(e => e.ProductId)
					  .OnDelete(DeleteBehavior.Restrict);
			});
		}
    }
}
