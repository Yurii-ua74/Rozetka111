using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rozetka.Data;
using Rozetka.Data.Entity;
using Rozetka.Models;
using Rozetka.Models.ViewModels.Products;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;

namespace Rozetka.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataContext _context;    

        public HomeController(ILogger<HomeController> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<Product> products = new List<Product>();

            try
            {
                // Получение списка продуктов с проверкой связей
                products = await _context.Products
                    .Include(p => p.ProductType)
                    .Include(p => p.Brand)
                    .Include(p => p.Childcategory)
                    .Include(p => p.ProductImages)
                    .Include(p => p.ProductColor)
                    .Include(p => p.Reviews)
                        .ThenInclude(r => r.User)
                    .ToListAsync();
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Получаем ID текущего пользователя

                if (userId != null)
                {
                    // Получаем список избранных товаров для текущего пользователя
                    var favoriteProductIds = await _context.Favorites
                        .Where(f => f.UserId == userId)
                        .Select(f => f.ProductId)
                        .ToListAsync();

                    // Добавляем информацию о том, находится ли каждый продукт в избранном
                    foreach (var product in products)
                    {
                        product.IsInFavorites = favoriteProductIds.Contains(product.Id);
                    }
                }
            }
            catch (Exception ex) // Ловим возможные исключения
            {
                // Логируем ошибку и возвращаем страницу ошибки
                _logger.LogError(ex, "Ошибка при получении списка продуктов.");
                return View("Error"); // Отображение страницы ошибки
            }           
            return View(products);
        }

        public IActionResult Test()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        
        // Відкриття сторінки адміна
        public IActionResult AdminPage()
        {
            return View();
        }
    }
}
