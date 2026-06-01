using ProductApp.DAL;

namespace ProductApp.BLL
{
    public class CartManager : ICartManager
    {
        private readonly IUnitOfWork _unitOfWork;

        public CartManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CartResponseDto?> GetCartAsync(string userId)
        {
            var cart = await _unitOfWork.CartRepositories.GetByUserIdWithItemsAsync(userId);
            if (cart == null) return null;

            return MapToCartResponse(cart);
        }

        public async Task<CartResponseDto> AddToCartAsync(string userId, AddToCartDto dto)
        {
            var product = await _unitOfWork.ProductRepositories.GetByIdAsync(dto.ProductId);
            if (product == null)
                throw new KeyNotFoundException($"Product with ID {dto.ProductId} not found");

            if (product.StockQuantity < dto.Quantity)
                throw new InvalidOperationException($"Insufficient stock. Available: {product.StockQuantity}");

            var cart = await _unitOfWork.CartRepositories.GetByUserIdWithItemsAsync(userId);
            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                _unitOfWork.CartRepositories.Insert(cart);
                await _unitOfWork.SaveChangesAsync();
            }

            var existingItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == dto.ProductId);
            if (existingItem != null)
            {
                existingItem.Quantity += dto.Quantity;
                existingItem.UnitPrice = product.Price;
            }
            else
            {
                cart.CartItems.Add(new CartItem
                {
                    CartId = cart.Id,
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity,
                    UnitPrice = product.Price
                });
            }

            await _unitOfWork.SaveChangesAsync();

            cart = await _unitOfWork.CartRepositories.GetByUserIdWithItemsAsync(userId);
            return MapToCartResponse(cart!);
        }

        public async Task<CartResponseDto> UpdateCartAsync(string userId, UpdateCartDto dto)
        {
            var cart = await _unitOfWork.CartRepositories.GetByUserIdWithItemsAsync(userId);
            if (cart == null)
                throw new KeyNotFoundException("Cart not found");

            var item = cart.CartItems.FirstOrDefault(ci => ci.ProductId == dto.ProductId);
            if (item == null)
                throw new KeyNotFoundException($"Product with ID {dto.ProductId} not found in cart");

            if (dto.Quantity == 0)
            {
                _unitOfWork.CartRepositories.DeleteCartItem(cart.CartItems.First(ci => ci.ProductId == dto.ProductId));
            }
            else
            {
                var product = await _unitOfWork.ProductRepositories.GetByIdAsync(dto.ProductId);
                if (product == null)
                    throw new KeyNotFoundException($"Product with ID {dto.ProductId} not found");

                if (product.StockQuantity < dto.Quantity)
                    throw new InvalidOperationException($"Insufficient stock. Available: {product.StockQuantity}");

                item.Quantity = dto.Quantity;
            }

            await _unitOfWork.SaveChangesAsync();

            cart = await _unitOfWork.CartRepositories.GetByUserIdWithItemsAsync(userId);
            return MapToCartResponse(cart!);
        }

        public async Task RemoveFromCartAsync(string userId, int productId)
        {
            var cart = await _unitOfWork.CartRepositories.GetByUserIdWithItemsAsync(userId);
            if (cart == null) return;

            var item = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
            if (item != null)
            {
                _unitOfWork.CartRepositories.DeleteCartItem(item);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        private static CartResponseDto MapToCartResponse(Cart cart)
        {
            return new CartResponseDto
            {
                CartId = cart.Id,
                Items = cart.CartItems.Select(ci => new CartItemDto
                {
                    ProductId = ci.ProductId,
                    ProductName = ci.Product?.Name ?? "Unknown",
                    ImageUrl = ci.Product?.ImageUrl,
                    Quantity = ci.Quantity,
                    UnitPrice = ci.UnitPrice,
                }).ToList()
            };
        }
    }
}
