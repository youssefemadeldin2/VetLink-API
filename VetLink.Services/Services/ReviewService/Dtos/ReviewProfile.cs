using AutoMapper;
using VetLink.Data.Entities;

namespace VetLink.Services.Services.ReviewService.Dtos
{
    public class ReviewProfile : Profile
    {
        public ReviewProfile()
        {
            CreateMap<AddReviewDto, Review>();
			//CreateMap< Review, AddReview>();
			CreateMap<Review, ShowReviewDto>()
				.ForMember(dest => dest.BuyerName, opt => opt.MapFrom(src =>
					src.ShowName && src.Buyer != null
					? src.Buyer.FullName
					: "Anonymous"));
		}
    }
}
