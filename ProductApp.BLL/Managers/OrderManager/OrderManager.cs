using Microsoft.EntityFrameworkCore.Storage;
using ProductApp.Common.Utilities;
using ProductApp.DAL;

namespace ProductApp.BLL
{
    public class OrderManager : IOrderManager
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<OrderDetailsDto> CreateOrderAsync(string userId)
        {
            var cart = await _unitOfWork.CartRepositories.GetByUserIdWithItemsAsync(userId);
            if (cart == null || !cart.CartItems.Any())
                throw new InvalidOperationException("Cart is empty");

            using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                var order = new Order
                {
                    UserId = userId,
                    OrderDate = DateTime.UtcNow,
                    Status = OrderStatus.Pending.ToString(),
                    TotalAmount = cart.CartItems.Sum(ci => ci.UnitPrice * ci.Quantity)
                };

                foreach (var cartItem in cart.CartItems)
                {
                    var product = await _unitOfWork.ProductRepositories.GetByIdAsync(cartItem.ProductId);
                    if (product == null)
                        throw new KeyNotFoundException($"Product with ID {cartItem.ProductId} not found");

                    if (product.StockQuantity < cartItem.Quantity)
                        throw new InvalidOperationException($"Insufficient stock for product '{product.Name}'. Available: {product.StockQuantity}");

                    product.StockQuantity -= cartItem.Quantity;

                    order.OrderItems.Add(new OrderItem
                    {
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                        UnitPrice = cartItem.UnitPrice
                    });
                }

                _unitOfWork.OrderRepositories.Insert(order);
                _unitOfWork.CartRepositories.Delete(cart);
                await _unitOfWork.SaveChangesAsync();

                await transaction.CommitAsync();

                return MapToOrderDetails(order);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<OrderResponseDto>> GetUserOrdersAsync(string userId)
        {
            var orders = await _unitOfWork.OrderRepositories.GetByUserIdAsync(userId);
            return orders.Select(o => new OrderResponseDto
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                ItemCount = o.OrderItems.Sum(oi => oi.Quantity)
            }).ToList();
        }

        public async Task<OrderDetailsDto?> GetOrderByIdAsync(int orderId, string userId)
        {
            var order = await _unitOfWork.OrderRepositories.GetByIdWithItemsAsync(orderId);
            if (order == null || order.UserId != userId) return null;

            return MapToOrderDetails(order);
        }

        private static OrderDetailsDto MapToOrderDetails(Order order)
        {
            return new OrderDetailsDto
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                Items = order.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product?.Name ?? "Unknown",
                    ImageUrl = oi.Product?.ImageUrl,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList()
            };
        }
    }
}
