namespace VetLink.Services.Services.ProductService.Dtos
{
    public class ProductProfileDto
    {
        public int  Id { get; set; }
        public string  Name { get; set; }
        public string  Description { get; set; }
        public decimal  Price{ get; set; }
		public bool IsActive { get; set; } = true;
		public bool IsFeatured { get; set; } = false;
        public string  ImgUrl{ get; set; }
        public int UniteInStock { get; set; }
        public string SellerName { get; set; }
    }
}
