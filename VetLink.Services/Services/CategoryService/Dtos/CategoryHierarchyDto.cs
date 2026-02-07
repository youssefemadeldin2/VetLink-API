namespace VetLink.Services.Services.CategoryService.Dtos
{
    public class CategoryHierarchyDto : CategoryDto
	{
		public List<CategoryHierarchyDto> Children { get; set; } = new List<CategoryHierarchyDto>();
	}
}
