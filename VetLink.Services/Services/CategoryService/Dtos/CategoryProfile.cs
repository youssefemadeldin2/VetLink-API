using AutoMapper;
using VetLink.Data.Entities;
using VetLink.Services.Services.CategoryService.Dtos;

namespace VetLink.Services.Services.CategoryService.Profiles
{
	public class CategoryProfile : Profile
	{
		public CategoryProfile()
		{
			CreateMap<CategoryDto, Category>();

			CreateMap<Category, AllCategoryDto>()
				.ForMember(dest => dest.IsParentCategory,
					opt => opt.MapFrom(src => src.SubCategories != null && src.SubCategories.Any()));

			CreateMap<Category, CategoryDto>()
				.ForMember(dest => dest.ParentId,
					opt => opt.MapFrom(src => src.ParentId));

			CreateMap<Category, CategoryByIdDto>()
				.ForMember(dest => dest.SubCategories,
					opt => opt.MapFrom(src => src.SubCategories));

			CreateMap<Category, SubCategoryDto>();
		}
	}

}