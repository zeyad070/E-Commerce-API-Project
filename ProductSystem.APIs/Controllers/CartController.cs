using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductApp.BLL;
using ProductApp.Common.Utilities;
using System.Security.Claims;

namespace ProductSystem.APIs.Controllers
{
    [Route("api/cart")]
    [ApiController]
    [Authorize(Policy = "CustomerOnly")]
    public class CartController : ControllerBase
    {
        private readonly ICartManager _cartManager;
        private readonly IValidator<AddToCartDto> _addToCartValidator;
        private readonly IValidator<UpdateCartDto> _updateCartValidator;

        public CartController(
            ICartManager cartManager,
            IValidator<AddToCartDto> addToCartValidator,
            IValidator<UpdateCartDto> updateCartValidator)
        {
            _cartManager = cartManager;
            _addToCartValidator = addToCartValidator;
            _updateCartValidator = updateCartValidator;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<CartResponseDto>>> GetCart()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var cart = await _cartManager.GetCartAsync(userId);

            if (cart == null)
                return Ok(ApiResponse<CartResponseDto>.Ok(new CartResponseDto(), "Cart is empty"));

            return Ok(ApiResponse<CartResponseDto>.Ok(cart));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<CartResponseDto>>> AddToCart([FromBody] AddToCartDto dto)
        {
            var validation = await _addToCartValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                return BadRequest(ApiResponse<CartResponseDto>.BadRequest("Validation failed",
                    validation.Errors.Select(e => e.ErrorMessage).ToList()));

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            try
            {
                var cart = await _cartManager.AddToCartAsync(userId, dto);
                return Ok(ApiResponse<CartResponseDto>.Ok(cart, "Product added to cart"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<CartResponseDto>.BadRequest(ex.Message));
            }
        }

        [HttpPut]
        public async Task<ActionResult<ApiResponse<CartResponseDto>>> UpdateCart([FromBody] UpdateCartDto dto)
        {
            var validation = await _updateCartValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                return BadRequest(ApiResponse<CartResponseDto>.BadRequest("Validation failed",
                    validation.Errors.Select(e => e.ErrorMessage).ToList()));

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            try
            {
                var cart = await _cartManager.UpdateCartAsync(userId, dto);
                return Ok(ApiResponse<CartResponseDto>.Ok(cart, "Cart updated"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<CartResponseDto>.BadRequest(ex.Message));
            }
        }

        [HttpDelete("{productId:int}")]
        public async Task<ActionResult<ApiResponse<string>>> RemoveFromCart(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _cartManager.RemoveFromCartAsync(userId, productId);
            return Ok(ApiResponse<string>.Ok("Product removed from cart"));
        }
    }
}
