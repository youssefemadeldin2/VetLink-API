using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using VetLink.Data.Entities;
using VetLink.Data.Enums;
using VetLink.Repository.Interfaces;
using VetLink.Repository.Specifications.AddressSpecifications;
using VetLink.Repository.Specifications.Paginated;
using VetLink.Services.Helper;
using VetLink.Services.Services.AccountService.MailService;
using VetLink.Services.Services.AccountService.OTPService;
using VetLink.Services.Services.AccountService.TokenService;
using VetLink.Services.Services.CachedService;
using VetLink.Services.Services.OrderService.Dtos;

namespace VetLink.Services.Services.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderService> _logger;
        private readonly UserManager<User> _userManager;
        private readonly IOtpService _otpService;
        private readonly ITokenService _tokenService;
        private readonly ICachedService _cacheService;
        IValidator<OrderActivationRequest> _validator;

		public OrderService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
			IOtpService otpService,
			ITokenService tokenService,
			ICachedService cacheService,
			UserManager<User> userManager,
			IValidator<OrderActivationRequest> validator,
			ILogger<OrderService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _userManager = userManager;
			_validator = validator;
			_otpService = otpService;
			_tokenService = tokenService;
			_cacheService = cacheService;
		}

		public async Task<ServiceResult<OrderResponseDto>> ActivateBuyerCartAsync(OrderActivationRequest request)
		{
			var validationResult = await _validator.ValidateAsync(request);
			if (!validationResult.IsValid)
				return ServiceResult<OrderResponseDto>.Fail(
					"Validation Data Error",
					(int)HttpStatusCode.BadRequest);

			var userRepository = _unitOfWork.Repository<User, int>();
			if (!await userRepository.ExistsAsync(request.BuyerId))
				return ServiceResult<OrderResponseDto>.Fail("User not found", (int)HttpStatusCode.NotFound);

			var orderRepository = _unitOfWork.Repository<Order, int>();
			var existingOrder = await orderRepository.GetByIdWithSpecAsync(new ActiveOrderByBuyerSpec(request.BuyerId));

			if (existingOrder != null)
				return ServiceResult<OrderResponseDto>.Fail("User already has an active order", (int)HttpStatusCode.Conflict);

			// Create order transaction
			using var transaction = await _unitOfWork.BeginTransactionAsync();

			try
			{
				var order = await CreateOrderWithAddressAsync(request);

				await _unitOfWork.SaveChangesAsync();
				await transaction.CommitAsync();

				var orderDto = _mapper.Map<OrderResponseDto>(order);
				return ServiceResult<OrderResponseDto>.Ok(orderDto);
			}
			catch (Exception ex)
			{
				await transaction.RollbackAsync();
				_logger.LogError(ex, "Failed to activate buyer cart for buyer {BuyerId}", request.BuyerId);
				return ServiceResult<OrderResponseDto>.Fail("Failed to create order", (int)HttpStatusCode.InternalServerError);
			}
		}

		private async Task<Order> CreateOrderWithAddressAsync(OrderActivationRequest request)
		{
			var addressRepository = _unitOfWork.Repository<Address, int>();
			var orderRepository = _unitOfWork.Repository<Order, int>();

			// Create address
			var address = _mapper.Map<Address>(request);
			address = await addressRepository.AddAsync(address);

			// Create order
			var order = _mapper.Map<Order>(request);
			order.AddressId = address.Id;
			order.OrderNumber = await orderRepository.GenerateOrderNumberAsync();
			order.PaymentStatus = PaymentStatus.pending;
			order.TotalAmount = 0;
            order.PaymentMethod = request.PaymentMethod;
            order.Status = OredrStatus.Active;
            order.BuyerId = request.BuyerId;

            return await orderRepository.AddAsync(order);
		}








        public async Task<ServiceResult<OrderResponseDto>> AddItemToOrder(int buyerId, int productId, int qty)
        {
            //         var orderRepo = _unitOfWork.Repository<Order, int>();
            //         var AddRepo = _unitOfWork.Repository<Address, int>();
            //var productRepo = _unitOfWork.Repository<Product, int>();

            //var ActiveBuyerOrder = await orderRepo.GetByIdWithSpecAsync(
            //                new ActiveOrderByBuyerSpec(buyerId)
            //            );
            //         if (ActiveBuyerOrder == null)
            //         {
            //	var Add = await AddRepo.ListAllWithSpecAsync(new UserAddressWithSpecifications(buyerId));
            //	var NewOrder = new Order
            //             {
            //                 BuyerId = buyerId,
            //                 AddressId = Add.FirstOrDefault().Id,
            //                 PaymentMethod = PaymentMethod.CashOnDelivery,
            //                 PaymentStatus= PaymentStatus.pending,
            //                 Status= OredrStatus.Active
            //	};

            //             await orderRepo.AddAsync(NewOrder);
            //	await _unitOfWork.SaveChangesAsync();

            //	var product = await productRepo.GetByIdAsync(item.ProductId);
            //	if (product == null)
            //	{
            //		return ServiceResult<OrderResponseDto>.Fail("المنتج غير موجود");
            //	}

            //}


            throw new NotImplementedException();
        }


        public async Task<ServiceResult<OrderItemDto>> AddItemAsync(int buyerId, AddOrderItemDto dto)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                var orderRepo = _unitOfWork.Repository<Order, int>();
                var orderItemRepo = _unitOfWork.Repository<OrderItem, int>();
                var productRepo = _unitOfWork.Repository<Product, int>();
                var AddRepo = _unitOfWork.Repository<Address, int>();

                var order = await orderRepo.GetByIdWithSpecAsync(
                    new ActiveOrderByBuyerSpec(buyerId)
                );


                if (order == null)
                {
                    var Add = await AddRepo.GetByIdWithSpecAsync(new UserAddressWithSpecifications(buyerId));

                    order = new Order
                    {
                        BuyerId = buyerId,
                        Status = OredrStatus.Active,
                        OrderNumber = await orderRepo.GenerateOrderNumberAsync(),
                        CreatedAt = DateTime.UtcNow,
                        AddressId = Add.Id,

                    };

                    await orderRepo.AddAsync(order);
                    await _unitOfWork.SaveChangesAsync();
                }

                if (order.Status != OredrStatus.Active)
                {
                    return ServiceResult<OrderItemDto>.Fail(
                        "لا يمكن إضافة عناصر على الطلب في حالته الحالية");
                }

                var product = await productRepo.GetByIdAsync(dto.ProductId);
                if (product == null)
                {
                    return ServiceResult<OrderItemDto>.Fail("المنتج غير موجود");
                }

                if (dto.Quantity > product.StockQuantity)
                {
                    return ServiceResult<OrderItemDto>.Fail(
                        $"الكمية المتاحة من المنتج ({product.Name}) هي {product.StockQuantity} فقط");
                }

                // 5️⃣ Check if item already exists in order
                var existingItem = order.OrderItems
                    .FirstOrDefault(i => i.ProductId == dto.ProductId);

                if (existingItem != null)
                {
                    existingItem.Quantity += dto.Quantity;
                    existingItem.TotalPrice =
                        existingItem.Quantity * existingItem.Price;
                }
                else
                {
                    var orderItem = new OrderItem
                    {
                        OrderId = order.Id,
                        ProductId = product.Id,
                        Quantity = dto.Quantity,
                        Price = product.Price,
                        TotalPrice = product.Price * dto.Quantity
                    };

                    await orderItemRepo.AddAsync(orderItem);
                    order.OrderItems.Add(orderItem);
                }

                // 6️⃣ Recalculate order total
                order.TotalAmount = order.OrderItems.Sum(i => i.TotalPrice);
                order.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation(
                    "Item added to Order {OrderNumber} for Buyer {BuyerId}",
                    order.OrderNumber, buyerId);

                var addedItem = order.OrderItems
                    .First(i => i.ProductId == dto.ProductId);

                return ServiceResult<OrderItemDto>.Ok(
                    _mapper.Map<OrderItemDto>(addedItem));
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync();
                return ServiceResult<OrderItemDto>.Fail(
                    "تم تعديل الطلب من جهاز آخر، حاول مرة أخرى");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error while adding item to order");
                return ServiceResult<OrderItemDto>.Fail("حدث خطأ غير متوقع");
            }
        }



        public Task<ServiceResult<OrderResponseDto>> GetActiveOrderAsync(int buyerId)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResult<IReadOnlyList<OrderResponseDto>>> GetAllOrdersAsync(OredrStatus? Status, PaginationSpecification pagination)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResult<List<OrderResponseDto>>> GetBuyerOrdersAsync(int buyerId, string search)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResult<bool>> RemoveItemAsync(int buyerId, int orderItemId)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResult<OrderResponseDto>> SubmitOrderAsync(int buyerId, int orderId)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResult<OrderItemDto>> UpdateItemQuantityAsync(int buyerId, int orderItemId, int quantity)
        {
            throw new NotImplementedException();
        }
    }
}




//using System.Net;
//using AutoMapper;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Logging;
//using VetLink.Data.Entities;
//using VetLink.Data.Enums;
//using VetLink.Repository.Interfaces;
//using VetLink.Repository.Specifications.OrderSpecifications;
//using VetLink.Repository.Specifications.Paginated;
//using VetLink.Services.Helper;
//using VetLink.Services.Services.Extensions;
//using VetLink.Services.Services.OrderService.Dtos;
//using VetLink.Services.Services.OrderService.Helpers;

//namespace VetLink.Services.Services.OrderService
//{
//    public class OrderService : IOrderService
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//        private readonly ILogger<OrderService> _logger;

//        public OrderService(
//            IUnitOfWork unitOfWork,
//            IMapper mapper,
//            ILogger<OrderService> logger)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//            _logger = logger;
//        }

//        public async Task<ServiceResult<OrderResponseDto>> CreateOrderAsync(CreateOrderDto dto)
//        {
//            _logger.LogInformation("Creating new order for buyer {BuyerId}", dto.BuyerId);

//            try
//            {
//                var productRepo = _unitOfWork.Repository<Product, int>();
//                var orderRepo = _unitOfWork.Repository<Order, int>();
//                var couponRepo = _unitOfWork.Repository<Coupon, int>();

//                var order = new Order
//                {
//                    BuyerId = dto.BuyerId,
//                    AddressId = dto.AddressId,
//                    Status = OredrStatus.pending,
//                    PaymentMethod = dto.PaymentMethod,
//                    PaymentStatus = PaymentStatus.pending,
//                    CreatedAt = DateTime.UtcNow
//                };

//                decimal total = 0;

//                foreach (var item in dto.Items)
//                {
//                    var product = await productRepo.GetByIdAsync(item.ProductId);
//                    if (product == null)
//                    {
//                        _logger.LogWarning("Product {ProductId} not found while creating order", item.ProductId);
//                        return ServiceResult<OrderResponseDto>.Fail(
//                            $"Product {item.ProductId} not found",
//                            (int)HttpStatusCode.NotFound);
//                    }

//                    // Check product stock if applicable
//                    if (product.StockQuantity < item.Quantity)
//                    {
//                        _logger.LogWarning("Insufficient stock for product {ProductId}. Requested: {Quantity}, Available: {Stock}",
//                            item.ProductId, item.Quantity, product.StockQuantity);
//                        return ServiceResult<OrderResponseDto>.Fail(
//                            $"Insufficient stock for product {product.Name}",
//                            (int)HttpStatusCode.BadRequest);
//                    }

//                    var price = product.Price * item.Quantity;
//                    total += price;

//                    order.OrderItems.Add(new OrderItem
//                    {
//                        ProductId = product.Id,
//                        Quantity = item.Quantity,
//                        Price = product.Price
//                    });
//                }

//                if (dto.Coupons != null && dto.Coupons.Any())
//                {
//                    var appliedCoupons = new HashSet<int>();

//                    foreach (var couponDto in dto.Coupons)
//                    {
//                        if (!appliedCoupons.Add(couponDto.CouponId))
//                        {
//                            return ServiceResult<OrderResponseDto>.Fail(
//                                "Duplicate coupon is not allowed",
//                                (int)HttpStatusCode.BadRequest);
//                        }

//                        var coupon = await couponRepo.GetByIdAsync(couponDto.CouponId);
//                        if (coupon == null)
//                        {
//                            return ServiceResult<OrderResponseDto>.Fail(
//                                $"Coupon {couponDto.CouponId} not found",
//                                (int)HttpStatusCode.NotFound);
//                        }

//                        if (!coupon.IsValid())
//                        {
//                            return ServiceResult<OrderResponseDto>.Fail(
//                                $"Coupon {coupon.Code} is invalid or expired",
//                                (int)HttpStatusCode.BadRequest);
//                        }

//                        var discount = coupon.CalculateDiscount(total);
//                        total -= discount;

//                        coupon.UsageCount++;

//                        order.OrderCoupons.Add(new OrderCoupons
//                        {
//                            CouponId = coupon.Id,
//                            DiscountAmount = discount
//                        });
//                    }
//                }

//                order.TotalAmount = total;

//                await orderRepo.AddAsync(order);
//                await _unitOfWork.SaveChangesAsync();

//				var spec = new OrderWithSpecification(order.Id);
//				var createdOrder = await orderRepo.GetByIdWithSpecAsync(spec);


//				return ServiceResult<OrderResponseDto>.Ok(
//					_mapper.Map<OrderResponseDto>(createdOrder));

//			}
//			catch (DbUpdateException dbEx)
//            {
//                _logger.LogError(dbEx, "Database error while creating order for buyer {BuyerId}", dto.BuyerId);
//                return ServiceResult<OrderResponseDto>.Fail(
//                    "Database error occurred while creating order",
//                    (int)HttpStatusCode.InternalServerError);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Unexpected error while creating order for buyer {BuyerId}", dto.BuyerId);
//                return ServiceResult<OrderResponseDto>.Fail(
//                    "An unexpected error occurred",
//                    (int)HttpStatusCode.InternalServerError);
//            }
//        }

//        public async Task<ServiceResult<OrderResponseDto>> GetOrderByIdAsync(int orderId)
//        {
//            _logger.LogInformation("Fetching order with ID {OrderId}", orderId);

//            try
//            {
//                var repo = _unitOfWork.Repository<Order, int>();
//                var spec = new OrderWithSpecification(orderId);

//                var order = await repo.GetByIdWithSpecAsync(spec);

//                if (order == null)
//                {
//                    _logger.LogWarning("Order {OrderId} not found", orderId);
//                    return ServiceResult<OrderResponseDto>.Fail(
//                        "Order not found",
//                        (int)HttpStatusCode.NotFound);
//                }

//                _logger.LogDebug("Order {OrderId} retrieved successfully", orderId);
//                return ServiceResult<OrderResponseDto>.Ok(
//                    _mapper.Map<OrderResponseDto>(order));
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error retrieving order {OrderId}", orderId);
//                return ServiceResult<OrderResponseDto>.Fail(
//                    "An error occurred while retrieving the order",
//                    (int)HttpStatusCode.InternalServerError);
//            }
//        }

//        public async Task<ServiceResult<bool>> CancelOrderAsync(int orderId)
//        {
//            _logger.LogInformation("Cancelling order with ID {OrderId}", orderId);

//            try
//            {
//                var orderRepo = _unitOfWork.Repository<Order, int>();

//                var order = await orderRepo.GetByIdAsync(orderId);
//                if (order == null)
//                {
//                    _logger.LogWarning("Order {OrderId} not found for cancellation", orderId);
//                    return ServiceResult<bool>.Fail(
//                        "Order not found",
//                        (int)HttpStatusCode.NotFound);
//                }

//                if (order.Status != OredrStatus.pending)
//                {
//                    _logger.LogWarning("Cannot cancel order {OrderId} with status {Status}",
//                        orderId, order.Status);
//                    return ServiceResult<bool>.Fail(
//                        "Only pending orders can be canceled",
//                        (int)HttpStatusCode.BadRequest);
//                }

//                order.Status = OredrStatus.cancelled;
//                order.UpdatedAt = DateTime.UtcNow;

//                orderRepo.Update(order);
//                await _unitOfWork.SaveChangesAsync();

//                _logger.LogInformation("Order {OrderId} cancelled successfully", orderId);
//                return ServiceResult<bool>.Ok(true);
//            }
//            catch (DbUpdateException dbEx)
//            {
//                _logger.LogError(dbEx, "Database error while cancelling order {OrderId}", orderId);
//                return ServiceResult<bool>.Fail(
//                    "Database error occurred while cancelling order",
//                    (int)HttpStatusCode.InternalServerError);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Unexpected error while cancelling order {OrderId}", orderId);
//                return ServiceResult<bool>.Fail(
//                    "An unexpected error occurred",
//                    (int)HttpStatusCode.InternalServerError);
//            }
//        }

//        public async Task<ServiceResult<bool>> ChangeOrderStatusAsync(int orderId, OredrStatus newStatus)
//        {
//            _logger.LogInformation("Changing order {OrderId} status to {NewStatus}", orderId, newStatus);

//            try
//            {
//                var repo = _unitOfWork.Repository<Order, int>();
//                var order = await repo.GetByIdAsync(orderId);

//                if (order == null)
//                {
//                    _logger.LogWarning("Order {OrderId} not found for status change", orderId);
//                    return ServiceResult<bool>.Fail(
//                        "Order not found",
//                        (int)HttpStatusCode.NotFound);
//                }

//                if (!OrderStatusRules.CanTransition(order.Status, newStatus))
//                {
//                    _logger.LogWarning("Invalid status transition from {CurrentStatus} to {NewStatus} for order {OrderId}",
//                        order.Status, newStatus, orderId);
//                    return ServiceResult<bool>.Fail(
//                        $"Cannot change status from {order.Status} to {newStatus}",
//                        (int)HttpStatusCode.BadRequest);
//                }

//                order.Status = newStatus;
//                order.UpdatedAt = DateTime.UtcNow;

//                // Update payment status based on order status
//                if (newStatus == OredrStatus.completed)
//                {
//                    order.PaymentStatus = PaymentStatus.paid;
//                }

//                repo.Update(order);
//                await _unitOfWork.SaveChangesAsync();

//                _logger.LogInformation("Order {OrderId} status changed successfully to {NewStatus}", orderId, newStatus);
//                return ServiceResult<bool>.Ok(true, "Order Status Changed Successfully");
//            }
//            catch (DbUpdateException dbEx)
//            {
//                _logger.LogError(dbEx, "Database error while changing status for order {OrderId}", orderId);
//                return ServiceResult<bool>.Fail(
//                    "Database error occurred while changing order status",
//                    (int)HttpStatusCode.InternalServerError);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Unexpected error while changing status for order {OrderId}", orderId);
//                return ServiceResult<bool>.Fail(
//                    "An unexpected error occurred",
//                    (int)HttpStatusCode.InternalServerError);
//            }
//        }

//        public async Task<ServiceResult<bool>> RefundOrderAsync(int orderId, string reason)
//        {
//            _logger.LogInformation("Processing refund request for order {OrderId}", orderId);

//            try
//            {
//                var orderRepo = _unitOfWork.Repository<Order, int>();
//                var returnRepo = _unitOfWork.Repository<Return, int>();

//                var order = await orderRepo.GetByIdAsync(orderId);
//                if (order == null)
//                    return ServiceResult<bool>.Fail("Order not found", 404);

//                if (order.PaymentStatus != PaymentStatus.paid)
//                    return ServiceResult<bool>.Fail("Order is not paid", 400);

//                if (order.Status != OredrStatus.completed)
//                    return ServiceResult<bool>.Fail("Only completed orders can be returned", 400);

//                var orderReturn = new Return
//                {
//                    OrderId = order.Id,
//                    BuyerId = order.BuyerId!.Value,
//                    Reason = reason,
//                    Status = ReturnStatus.pending_approval
//                };

//                await returnRepo.AddAsync(orderReturn);

//                order.Status = OredrStatus.returned;
//                order.UpdatedAt = DateTime.UtcNow;

//                orderRepo.Update(order);
//                await _unitOfWork.SaveChangesAsync();

//                return ServiceResult<bool>.Ok(true, "Return request created");
//            }
//            catch (DbUpdateException dbEx)
//            {
//                _logger.LogError(dbEx, "Database error while processing refund for order {OrderId}", orderId);
//                return ServiceResult<bool>.Fail(
//                    "Database error occurred while processing refund",
//                    (int)HttpStatusCode.InternalServerError);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Unexpected error while processing refund for order {OrderId}", orderId);
//                return ServiceResult<bool>.Fail(
//                    "An unexpected error occurred",
//                    (int)HttpStatusCode.InternalServerError);
//            }
//        }

//        public async Task<ServiceResult<bool>> ApproveReturnAsync(int returnId)
//        {
//            _logger.LogInformation("Approving return {ReturnId}", returnId);

//            try
//            {
//                var returnRepo = _unitOfWork.Repository<Return, int>();
//                var orderRepo = _unitOfWork.Repository<Order, int>();

//                var entity = await returnRepo.GetByIdAsync(returnId);
//                if (entity == null)
//                    return ServiceResult<bool>.Fail("Return not found", 404);

//                if (entity.Status != ReturnStatus.pending_approval)
//                    return ServiceResult<bool>.Fail("Invalid return state", 400);

//                entity.Status = ReturnStatus.approved;

//                var order = await orderRepo.GetByIdAsync(entity.OrderId);
//                order!.PaymentStatus = PaymentStatus.failed; // refunded equivalent
//                order.UpdatedAt = DateTime.UtcNow;

//                await _unitOfWork.SaveChangesAsync();

//                return ServiceResult<bool>.Ok(true, "Return approved and refunded");
//            }
//            catch (DbUpdateException dbEx)
//            {
//                _logger.LogError(dbEx, "Database error while approving return {ReturnId}", returnId);
//                return ServiceResult<bool>.Fail(
//                    "Database error occurred while approving return",
//                    (int)HttpStatusCode.InternalServerError);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Unexpected error while approving return {ReturnId}", returnId);
//                return ServiceResult<bool>.Fail(
//                    "An unexpected error occurred",
//                    (int)HttpStatusCode.InternalServerError);
//            }
//        }

//        public async Task<ServiceResult<IReadOnlyList<OrderResponseDto>>> GetOrdersAsync(OrderQueryDto query, PaginationSpecification pagination)
//        {
//            _logger.LogInformation("Fetching orders with query {@Query}", query);

//            try
//            {
//                var repo = _unitOfWork.Repository<Order, int>();

//                var spec = new OrderWithSpecification(
//                    query.BuyerId,
//                    query.Status,
//                    pagination);

//                var orders = await repo.ListAllWithSpecAsync(spec);

//                _logger.LogDebug("Retrieved {Count} orders", orders.Count);
//                return ServiceResult<IReadOnlyList<OrderResponseDto>>.Ok(
//                    _mapper.Map<IReadOnlyList<OrderResponseDto>>(orders));
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error retrieving orders with query {@Query}", query);
//                return ServiceResult<IReadOnlyList<OrderResponseDto>>.Fail(
//                    "An error occurred while retrieving orders",
//                    (int)HttpStatusCode.InternalServerError);
//            }
//        }

//		private async Task<Order> GetOrCreateActiveOrderAsync(int buyerId, int addressId, PaymentMethod paymentMethod)
//		{
//			var orderRepo = _unitOfWork.Repository<Order, int>();
//            var specs = new OrderWithSpecification(buyerId, paymentMethod); 
//			var orders = await orderRepo.ListAllWithSpecAsync(specs);
//            var order = orders.FirstOrDefault();
//			if (order != null)
//				return order;

//			order = new Order
//			{
//				BuyerId = buyerId,
//				AddressId = addressId,
//				PaymentMethod = paymentMethod,
//				Status = OredrStatus.pending,
//				PaymentStatus = PaymentStatus.pending,
//				CreatedAt = DateTime.UtcNow
//			};

//			await orderRepo.AddAsync(order);
//			return order;
//		}

//		public async Task<ServiceResult<OrderResponseDto>> AddToOrderAsync(
//	int buyerId, int productId, int quantity)
//		{
//			if (quantity <= 0)
//				return ServiceResult<OrderResponseDto>.Fail("Invalid quantity", 400);

//			var productRepo = _unitOfWork.Repository<Product, int>();
//			var orderRepo = _unitOfWork.Repository<Order, int>();
//            var ADDrepo = _unitOfWork.Repository<Address, int>();

//			var product = await productRepo.GetByIdAsync(productId);
//			if (product == null)
//				return ServiceResult<OrderResponseDto>.Fail("Product not found", 404);

//			var order = await GetOrCreateActiveOrderAsync(
//				buyerId, , PaymentMethod.cash);

//			var existingItem = order.OrderItems
//				.FirstOrDefault(i => i.ProductId == productId);

//			var newQuantity = existingItem == null
//				? quantity
//				: existingItem.Quantity + quantity;

//			if (newQuantity > product.StockQuantity)
//				return ServiceResult<OrderResponseDto>.Fail(
//					$"Only {product.StockQuantity} items available", 400);

//			if (existingItem != null)
//			{
//				existingItem.Quantity = newQuantity;
//			}
//			else
//			{
//				order.OrderItems.Add(new OrderItem
//				{
//					ProductId = productId,
//					Quantity = quantity,
//					Price = product.Price
//				});
//			}

//			order.TotalAmount = order.OrderItems.Sum(i => i.Price * i.Quantity);
//			order.UpdatedAt = DateTime.UtcNow;

//			await _unitOfWork.SaveChangesAsync();

//			return ServiceResult<OrderResponseDto>.Ok(
//				_mapper.Map<OrderResponseDto>(order));
//		}

//	}
//}