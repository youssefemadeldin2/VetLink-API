namespace VetLink.Services.Services.OrderService.Dtos
{
    public class AddressSummaryDto
	{
		public int AddressId { get; set; }

		public string City { get; set; }
		public string Street { get; set; }
		public string Building { get; set; }

		public string FullAddress =>
			$"{City}, {Street}, {Building}";
	}
}
