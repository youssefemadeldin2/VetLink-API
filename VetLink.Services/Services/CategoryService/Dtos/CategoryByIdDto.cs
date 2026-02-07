namespace VetLink.Services.Services.CategoryService.Dtos
{
    public class CategoryByIdDto
    {
        public int Id { get; set; }
        public String Name { get; set; }
		public List<SubCategoryDto> SubCategories { get; set; } = new List<SubCategoryDto>();

	}
}
