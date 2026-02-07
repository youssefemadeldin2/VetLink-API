using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetLink.WebApi.Helpers;
using VetLink.Services.Services.OrderService;
using VetLink.Services.Services.OrderService.Dtos;

namespace VetLink.WebApi.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/orders")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService service)
        {
            _orderService = service;
        }

        [HttpPost("CreateOrder")]
        [Authorize(Roles = "Buyer")]
        public async Task<IActionResult> CreateOrderAsync([FromBody] AddOrderItemDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Validation Error",
                    Detail = "The request contains invalid data",
                    Status = (int)HttpStatusCode.BadRequest,
                    Instance = HttpContext.Request.Path
                });
            }
            if (dto.ProductId < 0 || dto.Quantity < 0)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Bad Request",
                    Detail = "Order must contain at least one item",
                    Status = (int)HttpStatusCode.BadRequest
                });
            }
            var currentUserId = GetUserId();
            var result = await _orderService.AddItemAsync(currentUserId, dto);
            return new ResultActionResult<OrderItemDto>(result);
        }

        //[Authorize(Roles = "Buyer")]
        [HttpPost("ActivateOrder")]
        public async Task<IActionResult> ActivateCart(OrderActivationRequest dto)
        {
            var result = await _orderService.ActivateBuyerCartAsync(dto);
            return new ResultActionResult<OrderResponseDto>(result);
        }


        // in last time of refactoring i will ISOLATE IT IN helper method or ctor
        private int GetUserId()
        {
            var userIdClaim =
                User.FindFirst("uid")?.Value ??
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                throw new UnauthorizedAccessException("User ID not found in token");

            return userId;
        }
    }
}