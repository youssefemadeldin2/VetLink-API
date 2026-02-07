using VetLink.Data.Enums;

namespace VetLink.Services.Services.OrderService.Dtos
{
    public class BuyerOrderDto
    {
        public int OrderId { get; set; }

        public OredrStatus Status { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public PaymentStatus PaymentStatus { get; set; }

        public decimal TotalAmount { get; set; }
        public int ItemsCount { get; set; }

        public DateTime CreatedAt { get; set; }

        public AddressSummaryDto ShippingAddress { get; set; }

        public List<OrderItemSummaryDto> Items { get; set; } = new();
    }
}
