using AutoMapper;
using VetLink.Data.Entities;
using VetLink.Services.Services.ProductService.Dtos;

namespace VetLink.Services.Services.AccountService.Dtos.SellerDtos
{
    public class SellerProfile:Profile
    {
        public SellerProfile()
        {
            CreateMap<Seller, SellerProfileDto>()
				.ForMember(d => d.Id, o => o.MapFrom(s => s.UserId))
				.ForMember(d => d.Email, o => o.MapFrom(s => s.User.Email))
				.ForMember(d => d.FullName, o => o.MapFrom(s => s.User.FullName))
				.ForMember(d => d.Status, o => o.MapFrom(s => s.User.Status))
				.ForMember(d => d.CreatedAt, o => o.MapFrom(s => s.User.CreatedAt));
			CreateMap<Seller, SellerDetailDto>()
			.ForMember(d => d.Email, o => o.MapFrom(s => s.User.Email))
			.ForMember(d => d.FullName, o => o.MapFrom(s => s.User.FullName))
			.ForMember(d => d.Status, o => o.MapFrom(s => s.User.Status));
			CreateMap<Seller, ProductSeller>();
		}
    }
}
