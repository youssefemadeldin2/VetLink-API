using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VetLink.Data.Enums;

namespace VetLink.Data.Entities
{
    public class Order
    {
        public int Id { get; set; }
		[MaxLength(20)]
		public string OrderNumber { get; set; }
        public int? BuyerId { get; set; }
        public int AddressId { get; set; }
        public OredrStatus Status { get; set; } = OredrStatus.Active;
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; }
        public string Phone { get; set; }

        public PaymentMethod PaymentMethod { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DeletedAt { get; set; }
		[Timestamp]
		[ConcurrencyCheck]
		public byte[] RowVersion { get; set; } = null!;
		public User? Buyer { get; set; }
        public Address Address { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public List<OrderCoupons> OrderCoupons { get; set; } = new List<OrderCoupons>();
        public List<Shipment> Shipments { get; set; } = new List<Shipment>();
        public List<Return> Returns { get; set; } = new List<Return>();
    }
}