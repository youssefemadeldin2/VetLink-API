using System.ComponentModel.DataAnnotations;
using VetLink.Data.Enums;

namespace VetLink.Data.Entities
{
    public class Shipment
	{
		public int Id { get; set; }
		public int OrderId { get; set; }
		public int SellerId { get; set; }
        [MaxLength(100)]
        public string ShipMentCompany { get; set; }
        [MaxLength(100)]
        public string TrackingNumber { get; set; }
        public ShipmentStatus Status { get; set; }
		public DateTime CraetedAt { get; set; } = DateTime.UtcNow;
    }
}