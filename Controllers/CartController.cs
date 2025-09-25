using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ElectronicsStore.Data;
using ElectronicsStore.Models;
using System.Security.Claims;

namespace ElectronicsStore.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            try
            {
                // Kiểm tra user đã đăng nhập chưa
                if (!User.Identity.IsAuthenticated)
                {
                    return Json(new { success = false, message = "Vui lòng đăng nhập để thêm sản phẩm vào giỏ hàng" });
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                
                // Kiểm tra sản phẩm có tồn tại không
                var product = await _context.Products.FindAsync(productId);
                if (product == null)
                {
                    return Json(new { success = false, message = "Sản phẩm không tồn tại" });
                }

                // Kiểm tra số lượng tồn kho
                if (product.StockQuantity < quantity)
                {
                    return Json(new { success = false, message = "Số lượng sản phẩm không đủ" });
                }

                // Tìm hoặc tạo giỏ hàng cho user
                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart == null)
                {
                    cart = new Cart
                    {
                        UserId = userId,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };
                    _context.Carts.Add(cart);
                    await _context.SaveChangesAsync();
                }

                // Kiểm tra sản phẩm đã có trong giỏ hàng chưa
                var existingCartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
                
                if (existingCartItem != null)
                {
                    // Cập nhật số lượng
                    existingCartItem.Quantity += quantity;
                    
                    // Kiểm tra lại số lượng tồn kho
                    if (existingCartItem.Quantity > product.StockQuantity)
                    {
                        return Json(new { success = false, message = "Số lượng vượt quá tồn kho" });
                    }
                }
                else
                {
                    // Thêm sản phẩm mới vào giỏ hàng
                    var cartItem = new CartItem
                    {
                        CartId = cart.Id,
                        ProductId = productId,
                        Quantity = quantity,
                        CreatedAt = DateTime.Now
                    };
                    _context.CartItems.Add(cartItem);
                }

                cart.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();

                // Đếm tổng số sản phẩm trong giỏ hàng
                var totalItems = await _context.CartItems
                    .Where(ci => ci.Cart.UserId == userId)
                    .SumAsync(ci => ci.Quantity);

                return Json(new { 
                    success = true, 
                    message = "Đã thêm sản phẩm vào giỏ hàng",
                    cartItemCount = totalItems
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        public async Task<IActionResult> Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .ThenInclude(p => p.Category)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { CartItems = new List<CartItem>() };
            }

            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int cartItemId, int quantity)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                
                var cartItem = await _context.CartItems
                    .Include(ci => ci.Cart)
                    .Include(ci => ci.Product)
                    .FirstOrDefaultAsync(ci => ci.Id == cartItemId && ci.Cart.UserId == userId);

                if (cartItem == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy sản phẩm trong giỏ hàng" });
                }

                if (quantity <= 0)
                {
                    return Json(new { success = false, message = "Số lượng phải lớn hơn 0" });
                }

                if (quantity > cartItem.Product.StockQuantity)
                {
                    return Json(new { success = false, message = "Số lượng vượt quá tồn kho" });
                }

                cartItem.Quantity = quantity;
                cartItem.Cart.UpdatedAt = DateTime.Now;
                
                await _context.SaveChangesAsync();

                var itemTotal = cartItem.Quantity * cartItem.Product.Price;
                
                // Tính tổng giỏ hàng
                var cartTotal = await _context.CartItems
                    .Where(ci => ci.Cart.UserId == userId)
                    .Include(ci => ci.Product)
                    .SumAsync(ci => ci.Quantity * ci.Product.Price);

                return Json(new { 
                    success = true, 
                    itemTotal = itemTotal.ToString("N0"),
                    cartTotal = cartTotal.ToString("N0")
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveItem(int cartItemId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                
                var cartItem = await _context.CartItems
                    .Include(ci => ci.Cart)
                    .FirstOrDefaultAsync(ci => ci.Id == cartItemId && ci.Cart.UserId == userId);

                if (cartItem == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy sản phẩm trong giỏ hàng" });
                }

                _context.CartItems.Remove(cartItem);
                cartItem.Cart.UpdatedAt = DateTime.Now;
                
                await _context.SaveChangesAsync();

                // Tính tổng giỏ hàng sau khi xóa
                var cartTotal = await _context.CartItems
                    .Where(ci => ci.Cart.UserId == userId)
                    .Include(ci => ci.Product)
                    .SumAsync(ci => ci.Quantity * ci.Product.Price);

                var totalItems = await _context.CartItems
                    .Where(ci => ci.Cart.UserId == userId)
                    .SumAsync(ci => ci.Quantity);

                return Json(new { 
                    success = true, 
                    message = "Đã xóa sản phẩm khỏi giỏ hàng",
                    cartTotal = cartTotal.ToString("N0"),
                    cartItemCount = totalItems
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCartItemCount()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Json(new { count = 0 });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var totalItems = await _context.CartItems
                .Where(ci => ci.Cart.UserId == userId)
                .SumAsync(ci => ci.Quantity);

            return Json(new { count = totalItems });
        }
    }
}