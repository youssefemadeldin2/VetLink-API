using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using VetLink.Data.Entities;
using VetLink.Services.Services.OrderService.Dtos;

namespace VetLink.Services.Services.OrderService.Mapping
{
    public class OrderProfile:Profile
    {
        public OrderProfile()
        {
			CreateMap<Order, BuyerOrderDto>()
				.ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Id))
				.ForMember(dest => dest.ItemsCount, opt => opt.MapFrom(src => src.OrderItems.Count));

			CreateMap<OrderItem, OrderItemSummaryDto>()
				.ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name));

			CreateMap<Address, AddressSummaryDto>();
			CreateMap<Shipment, ShipmentDto>()
				.ForMember(d => d.ShipmentId, o => o.MapFrom(s => s.Id))
				.ForMember(d => d.ShipmentCompany, o => o.MapFrom(s => s.ShipMentCompany))
				.ForMember(d => d.CreatedAt, o => o.MapFrom(s => s.CraetedAt));

			CreateMap<Return, ReturnDto>()
				.ForMember(d => d.ReturnId, o => o.MapFrom(r => r.Id));

			CreateMap<ReturnItem, ReturnItemDto>()
				.ForMember(d => d.ProductName, o => o.MapFrom(ri => ri.Product.Name));
			CreateMap<Order, OrderResponseDto>();
			CreateMap<OrderItem, OrderItemDto>();
			CreateMap<OrderCoupons, OrderCouponDto>();

		}
	}
}
