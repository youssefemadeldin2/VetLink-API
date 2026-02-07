using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VetLink.Data.Entities
{
    public class Seller 
    {
		[Key, ForeignKey(nameof(User))]
		public int UserId { get; set; }

		public string StoreName { get; set; }
		public string StoreLogoURL { get; set; } = string.Empty;
		public string StoreDescription { get; set; }
		public string CommercialRegisterURL { get; set; }
		public string PracticeLicenseURL { get; set; }

		public DateTime? ApprovedAt { get; set; }
		//navigation property
		public User User { get; set; } = null!;
		public ICollection<Product> Products { get; set; } = new List<Product>();
	}
}
