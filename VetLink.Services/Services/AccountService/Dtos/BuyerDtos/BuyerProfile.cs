using AutoMapper;
using VetLink.Data.Entities;
using VetLink.Services.Helper;
using VetLink.Services.Services.AccountService.Dtos.BuyerDtos;
using VetLink.Services.Services.AccountService.Dtos.SellerDtos;
using VetLink.Services.Services.OrderService.Dtos;

namespace VetLink.Services.Mapping
{
	public class BuyerProfile : Profile
	{
		public BuyerProfile()
		{
			CreateMap<User, BuyerProfileDto>()
				.ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
				.ForMember(d => d.Email, o => o.MapFrom(s => s.Email))
				.ForMember(d => d.FullName, o => o.MapFrom(s => s.FullName))
				.ForMember(d => d.PhoneNumber, o => o.MapFrom(s => s.PhoneNumber))
				.ForMember(d => d.CreatedAt, o => o.MapFrom(s => s.CreatedAt))
				.ForMember(d => d.Orders, o => o.Ignore());
			CreateMap<UpdateBuyerProfileDto, User>()
				.ForAllMembers(opt =>
					opt.Condition((src, dest, srcMember) => srcMember != null));
			CreateMap<Order, BuyerOrderDto>()
							.ForMember(d => d.OrderId, o => o.MapFrom(s => s.Id))
							.ForMember(d => d.ItemsCount, o => o.MapFrom(s => s.OrderItems.Count))
							.ForMember(d => d.Items, o => o.MapFrom(s => s.OrderItems))
							.ForMember(d => d.ShippingAddress, o => o.MapFrom(s => s.Address));

			CreateMap<OrderItem, OrderItemSummaryDto>()
				.ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product.Name));

			CreateMap<Address, AddressSummaryDto>();
			CreateMap<Shipment, ShipmentDto>();
			CreateMap<Return, ReturnDto>();
			CreateMap<ReturnItem, ReturnItemDto>()
				.ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product.Name));
		}
	}
}
