using System.ComponentModel.DataAnnotations;

namespace VetLink.Data.Entities
{
    public class Address
    {
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        public string AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        [MaxLength(100)]
		public string City { get; set; }
        [MaxLength(20)]
		public string PostalCode {get; set; }
        [MaxLength(100)]
        public string Country { get; set; } = "Egypt";
        public bool IsDefault { get; set; } = false;
        public User User { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}