using VetLink.Data.Enums;

namespace VetLink.Data.Entities
{
    public class Return
    {
		public int Id { get; set; }
		public int OrderId { get; set; }
		public int BuyerId { get; set; }
        public string Reason { get; set; }
        public ReturnStatus Status { get; set; }
		public DateTime CreatedAt{ get; set; } = DateTime.UtcNow;
        public Order Order { get; set; }
		public User Buyer { get; set; }
		public ICollection<ReturnItem> ReturnItems { get; set; }
	}
}