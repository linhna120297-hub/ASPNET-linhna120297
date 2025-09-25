using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ElectronicsStore.Data;
using ElectronicsStore.Models;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;

namespace ElectronicsStore.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CheckoutController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.CartItems.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            var user = await _userManager.GetUserAsync(User);
            
            var model = new CheckoutViewModel
            {
                FirstName = user?.FirstName ?? "",
                LastName = user?.LastName ?? "",
                Email = user?.Email ?? "",
                Address = user?.Address ?? "",
                CartItems = cart.CartItems.ToList(),
                TotalAmount = cart.CartItems.Sum(ci => ci.Quantity * ci.Product.Price)
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder(CheckoutViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                    .FirstOrDefaultAsync(c => c.UserId == userId);
                
                model.CartItems = cart?.CartItems.ToList() ?? new List<CartItem>();
                model.TotalAmount = model.CartItems.Sum(ci => ci.Quantity * ci.Product.Price);
                
                return View("Index", model);
            }

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                
                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart == null || !cart.CartItems.Any())
                {
                    return RedirectToAction("Index", "Cart");
                }

                // Tạo đơn hàng
                var order = new Order
                {
                    UserId = userId,
                    OrderNumber = GenerateOrderNumber(),
                    TotalAmount = cart.CartItems.Sum(ci => ci.Quantity * ci.Product.Price),
                    Status = OrderStatus.Pending,
                    ShippingAddress = $"{model.Address}, {model.City}",
                    PhoneNumber = model.PhoneNumber,
                    Notes = model.Notes,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Tạo chi tiết đơn hàng
                foreach (var cartItem in cart.CartItems)
                {
                    var orderItem = new OrderItem
                    {
                        OrderId = order.Id,
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                        UnitPrice = cartItem.Product.Price,
                        TotalPrice = cartItem.Quantity * cartItem.Product.Price
                    };
                    _context.OrderItems.Add(orderItem);

                    // Cập nhật số lượng tồn kho
                    cartItem.Product.StockQuantity -= cartItem.Quantity;
                }

                // Xóa giỏ hàng
                _context.CartItems.RemoveRange(cart.CartItems);
                
                await _context.SaveChangesAsync();

                return RedirectToAction("OrderSuccess", new { orderId = order.Id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi đặt hàng: " + ex.Message);
                return View("Index", model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> OrderSuccess(int orderId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        private string GenerateOrderNumber()
        {
            return "ORD" + DateTime.Now.ToString("yyyyMMddHHmmss");
        }
    }

    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "Họ là bắt buộc")]
        public string FirstName { get; set; } = "";

        [Required(ErrorMessage = "Tên là bắt buộc")]
        public string LastName { get; set; } = "";

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        public string PhoneNumber { get; set; } = "";

        [Required(ErrorMessage = "Địa chỉ là bắt buộc")]
        public string Address { get; set; } = "";

        [Required(ErrorMessage = "Thành phố là bắt buộc")]
        public string City { get; set; } = "";

        public string? Notes { get; set; }

        [Required(ErrorMessage = "Phương thức thanh toán là bắt buộc")]
        public string PaymentMethod { get; set; } = "";

        public List<CartItem> CartItems { get; set; } = new();
        public decimal TotalAmount { get; set; }
    }
}