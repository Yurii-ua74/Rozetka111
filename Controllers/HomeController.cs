using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rozetka.Data;
using Rozetka.Models;
using Rozetka.Models.ViewModels.Products;
using System.Diagnostics;
using System.Linq;

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

        public IActionResult Index()
        {
            try
            {
                //HttpContext.Session.Clear();
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while loading the Index page.");
                return StatusCode(500, "Internal server error");
            }
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

        // GET: /Home/GetSearchResult
        public async Task<IActionResult> GetSearchResult(string inputSearching)
        {
            if (string.IsNullOrEmpty(inputSearching))  // Если поисковый запрос пуст или null, 
            {
                // Возвращаем пустую модель ProductSearchViewModel
                return View(new ProductSearchViewModel());
            }

            if (inputSearching.Length < 2)
            {
                return View(new ProductSearchViewModel()); // Возвращаем пустую модель при коротком запросе
            }

            // Разбиваем запрос на слова
            string[] words = inputSearching.Split(' ');

            // Инициализируем модель поиска
            ProductSearchViewModel productViewModel = new ProductSearchViewModel();

            // Загружаем продукты из базы данных
            var query = _context.Products
                    .Include(p => p.ProductType)
                    .Include(p => p.Brand)
                    .Include(p => p.Childcategory)
                    .Include(p => p.ProductImages)
                    .Include(p => p.ProductColor)
                    .Include(p => p.Reviews)
                    .AsQueryable(); 

            foreach (var word in words)
            {
                if (word.Length > 1)
                {
                         query = query.Where(product =>
                            (product.Title != null && product.Title.ToLower().Contains(word.ToLower())) ||
                            (product.ProductType != null && product.ProductType.Title.ToLower().Contains(word.ToLower())) ||
                            (product.Brand != null && product.Brand.Title.ToLower().Contains(word.ToLower())));
                }
            }

            // Выполняем запрос
            productViewModel.SearchingResults = await query.ToListAsync();

            return View(productViewModel);
        }

        // Відкриття сторінки адміна
        public IActionResult AdminPage()
        {
            return View();
        }
    }
}
