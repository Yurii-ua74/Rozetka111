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

        //public async Task<IActionResult> Index()
        //{
        //    List<Product> products = new List<Product>();

        //    try
        //    {
        //        // Получение списка продуктов с проверкой связей
        //        products = await _context.Products
        //            .Include(p => p.ProductType)
        //            .Include(p => p.Brand)
        //            .Include(p => p.Childcategory)
        //            .Include(p => p.ProductImages)
        //            .Include(p => p.ProductColor)
        //            .Include(p => p.Reviews)
        //                .ThenInclude(r => r.User)
        //            .ToListAsync();
        //        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Получаем ID текущего пользователя

        //        if (userId != null)
        //        {
        //            // Получаем список избранных товаров для текущего пользователя
        //            var favoriteProductIds = await _context.Favorites
        //                .Where(f => f.UserId == userId)
        //                .Select(f => f.ProductId)
        //                .ToListAsync();

        //            // Добавляем информацию о том, находится ли каждый продукт в избранном
        //            foreach (var product in products)
        //            {
        //                product.IsInFavorites = favoriteProductIds.Contains(product.Id);
        //            }
        //        }
        //    }
        //    catch (Exception ex) // Ловим возможные исключения
        //    {
        //        // Логируем ошибку и возвращаем страницу ошибки
        //        _logger.LogError(ex, "Ошибка при получении списка продуктов.");
        //        return View("Error"); // Отображение страницы ошибки
        //    }
        //    return View(products);
        //    //return View();
        //}

        public async Task<IActionResult> Index()
        {
            // Создаем новый объект модели представления
            var model = new ProductHomeIndexViewModel();

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Получаем ID текущего пользователя

                // Выборка продуктов с подгрузкой необходимых данных
                var products = await _context.Products
                    .Include(p => p.ProductType)
                    .Include(p => p.Brand)
                    .Include(p => p.Childcategory)
                    .Include(p => p.ProductImages)
                    .Include(p => p.ProductColor)
                    .Include(p => p.Reviews)
                    .ToListAsync();

                // Загружаем все активные акции
                var activeActions = await _context.Actions
                    .Where(a => a.StartDate <= DateTime.Now && a.EndDate >= DateTime.Now) // Только активные акции
                    .ToListAsync();

                // Присваиваем цену акции, если продукт имеет активную акцию
                foreach (var product in products)
                {
                    var action = activeActions.FirstOrDefault(a => a.ProductId == product.Id);
                    if (action != null)
                    {
                        product.ActionPrice = action.NewPrice; // Устанавливаем цену акции
                    }
                }

                // 1. Получаем 6 товаров с наибольшим рейтингом
                model.GetProductsRating = products
                    .Where(p => p.Rating.HasValue)
                    .OrderByDescending(p => p.Rating)
                    .Take(6)
                    .ToList();

                // 2. Получаем 6 случайных товаров
                model.GetProductsRandom = products
                    .OrderBy(p => Guid.NewGuid()) // Сортировка случайным образом
                    .Take(6)
                    .ToList();

                // 3. Выбираем только акционные товары, у которых акции активны                
                model.GetProductsActions = products
                    .Where(p => p.ActionPrice.HasValue) // Берем только продукты с акциями
                    .OrderBy(p => Guid.NewGuid())
                    .Take(6)
                    .ToList();

                // 4. Получаем 6 товаров, которые чаще всего добавляются в избранное
                var favorProductIds = await _context.Favorites
                    .GroupBy(f => f.ProductId)
                    .OrderByDescending(g => g.Count()) // Сортируем по количеству добавлений в избранное
                    .Take(6)
                    .Select(g => g.Key) // Получаем Id продуктов
                    .ToListAsync();

                // Теперь загружаем связанные данные для этих продуктов
                model.GetProductsFavorites = await _context.Products
                    .Where(p => favorProductIds.Contains(p.Id))
                    .Include(p => p.ProductType)
                    .Include(p => p.Brand)
                    .Include(p => p.ProductImages)
                    .ToListAsync();

                //5.Получаем 6 товаров, которые чаще всего покупают
                var frequentlyBoughtProductIds = await _context.ShoppingList
                    .Include(sl => sl.Cart)
                        .ThenInclude(c => c.Items)
                    .SelectMany(sl => sl.Cart.Items) // Получаем все товары из корзин
                    .GroupBy(ci => ci.ProductId)
                    .OrderByDescending(g => g.Count()) // Сортируем по количеству покупок
                    .Take(6)
                    .Select(g => g.Key) // Получаем только ProductId
                    .ToListAsync();

                // Теперь загружаем связанные данные для этих продуктов
                model.GetProductsShopping = await _context.Products
                    .Where(p => frequentlyBoughtProductIds.Contains(p.Id))
                    .Include(p => p.ProductType)
                    .Include(p => p.Brand)
                    .Include(p => p.ProductImages)
                    .ToListAsync();

                //6. Получаем ID категории "Одяг, взуття та прикраси"
                var garmentsCategoryId = await _context.Categories
                    .Where(c => c.Name == "Одяг, взуття та прикраси")
                    .Select(c => c.Id)
                    .FirstOrDefaultAsync();

                // Если категория найдена, выбираем продукты
                if (garmentsCategoryId != 0)                         
                {
                    model.GetProductsGarments = await _context.Products
                        .Where(p => p.CategoryId == garmentsCategoryId)      // || p.Childcategory.CategoryId == garmentsCategoryId
                        .OrderBy(r => Guid.NewGuid()) // Случайный порядок
                        .Take(6)
                        .Include(p => p.ProductType)
                        .Include(p => p.Brand)
                        .Include(p => p.ProductImages)
                        .ToListAsync();
                }

                // 7. Добавляем информацию об избранных продуктах, если пользователь авторизован
                if (userId != null)
                {
                    var favoriteProductIds = await _context.Favorites
                        .Where(f => f.UserId == userId)
                        .Select(f => f.ProductId)
                        .ToListAsync();

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

            // Возвращаем модель в представление
            return View(model);
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
        //public IActionResult Error()
        //{
        //    var errorViewModel = new ErrorViewModel
        //    {
        //        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        //    };

        //    return View(errorViewModel);
        //}


        // Відкриття сторінки адміна
        public IActionResult AdminPage()
        {
            return View();
        }

        //public IActionResult TestError()
        //{
        //    throw new Exception("Test error");
        //}
    }
}
