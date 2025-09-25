using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ElectronicsStore.Data;
using ElectronicsStore.Models;

namespace ElectronicsStore.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? category, string? search, decimal? minPrice, decimal? maxPrice, string? sort)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive);

            // Filter by category
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.Category.Slug == category);
            }

            // Filter by search
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Name.Contains(search) || p.Description!.Contains(search));
            }

            // Filter by price range
            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            // Sort products
            query = sort switch
            {
                "price_asc" => query.OrderBy(p => p.Price),
                "price_desc" => query.OrderByDescending(p => p.Price),
                "newest" => query.OrderByDescending(p => p.CreatedAt),
                _ => query.OrderBy(p => p.Name)
            };

            var products = await query.ToListAsync();
            var categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();

            ViewBag.Categories = categories;
            ViewBag.CurrentCategory = category;
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentMinPrice = minPrice;
            ViewBag.CurrentMaxPrice = maxPrice;
            ViewBag.CurrentSort = sort;

            return View(products);
        }

        public async Task<IActionResult> Details(string slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Slug == slug && p.IsActive);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
    }
}