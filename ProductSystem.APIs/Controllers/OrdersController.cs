using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductApp.BLL;
using ProductApp.Common.Utilities;
using System.Security.Claims;

namespace ProductSystem.APIs.Controllers
{
    [Route("api/orders")]
    [ApiController]
    [Authorize(Policy = "CustomerOnly")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderManager _orderManager;

        public OrdersController(IOrderManager orderManager)
        {
            _orderManager = orderManager;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<OrderDetailsDto>>> CreateOrder()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            try
            {
                var order = await _orderManager.CreateOrderAsync(userId);
                return Ok(ApiResponse<OrderDetailsDto>.Ok(order, "Order placed successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<OrderDetailsDto>.BadRequest(ex.Message));
            }
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<OrderResponseDto>>>> GetOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var orders = await _orderManager.GetUserOrdersAsync(userId);
            return Ok(ApiResponse<List<OrderResponseDto>>.Ok(orders));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<OrderDetailsDto>>> GetOrderById(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var order = await _orderManager.GetOrderByIdAsync(id, userId);

            if (order == null)
                return NotFound(ApiResponse<OrderDetailsDto>.NotFound("Order not found"));

            return Ok(ApiResponse<OrderDetailsDto>.Ok(order));
        }
    }
}
