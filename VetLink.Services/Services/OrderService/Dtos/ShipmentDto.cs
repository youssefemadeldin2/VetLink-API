using VetLink.Data.Enums;

namespace VetLink.Services.Services.OrderService.Dtos
{
    public class ShipmentDto
	{
		public int ShipmentId { get; set; }

		public string ShipmentCompany { get; set; }
		public string TrackingNumber { get; set; }

		public ShipmentStatus Status { get; set; }

		public DateTime CreatedAt { get; set; }
	}
}
