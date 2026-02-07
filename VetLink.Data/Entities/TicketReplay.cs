namespace VetLink.Data.Entities
{
    public class TicketReplay
    {
		public int Id { get; set; }
        public int? TicketId { get; set; }
        public int UserId { get; set; }
        public string Message { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.Now;

        public SupportTicket? Ticket { get; set; }
        public User User { get; set; }
    }
}