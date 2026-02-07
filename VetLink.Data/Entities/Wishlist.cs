namespace VetLink.Data.Entities
{
    public class Wishlist
	{
		public int UserId { get; set; }
		public int ProductId { get; set; }
		public virtual ICollection<Product> Products { get; set; } = new List<Product>();
		public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
}