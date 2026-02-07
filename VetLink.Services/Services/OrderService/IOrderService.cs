using VetLink.Data.Enums;
using VetLink.Repository.Specifications.Paginated;
using VetLink.Services.Helper;
using VetLink.Services.Services.OrderService.Dtos;

namespace VetLink.Services.Services.OrderService
{
    public interface IOrderService
    {
        Task<ServiceResult<OrderResponseDto>> ActivateBuyerCartAsync(OrderActivationRequest request);

        Task<ServiceResult<OrderResponseDto>> GetActiveOrderAsync(int buyerId);
        Task<ServiceResult<OrderItemDto>> AddItemAsync(int buyerId, AddOrderItemDto dto);
        Task<ServiceResult<OrderItemDto>> UpdateItemQuantityAsync(int buyerId, int orderItemId, int quantity);
        Task<ServiceResult<bool>> RemoveItemAsync(int buyerId, int orderItemId);

        Task<ServiceResult<OrderResponseDto>> SubmitOrderAsync(int buyerId, int orderId);

        Task<ServiceResult<List<OrderResponseDto>>> GetBuyerOrdersAsync(int buyerId, string search);
        Task<ServiceResult<IReadOnlyList<OrderResponseDto>>> GetAllOrdersAsync(OredrStatus? Status, PaginationSpecification pagination);


        //Task<ServiceResult<OrderResponseDto>> CreateOrderAsync(CreateOrderDto dto);
        //Task<ServiceResult<OrderResponseDto>> GetOrderByIdAsync(int orderId);
        //Task<ServiceResult<bool>> CancelOrderAsync(int orderId);
        //Task<ServiceResult<bool>> ChangeOrderStatusAsync(int orderId, OredrStatus newStatus);
        //Task<ServiceResult<bool>> RefundOrderAsync(int orderId, string reason);
        //Task<ServiceResult<bool>> ApproveReturnAsync(int returnId);
        //Task<ServiceResult<OrderResponseDto>> AddToOrderAsync(int buyerId, int productId, int quantity);
    }


}
