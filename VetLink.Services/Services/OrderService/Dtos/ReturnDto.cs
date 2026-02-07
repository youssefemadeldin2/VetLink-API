using VetLink.Data.Enums;

namespace VetLink.Services.Services.OrderService.Dtos
{
    public class ReturnDto
	{
		public int ReturnId { get; set; }

		public ReturnStatus Status { get; set; }
		public string Reason { get; set; }

		public DateTime CreatedAt { get; set; }

		public List<ReturnItemDto> Items { get; set; } = new();
	}
}
