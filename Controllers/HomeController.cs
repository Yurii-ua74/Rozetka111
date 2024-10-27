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
        //        // ��������� ������ ��������� � ��������� ������
        //        products = await _context.Products
        //            .Include(p => p.ProductType)
        //            .Include(p => p.Brand)
        //            .Include(p => p.Childcategory)
        //            .Include(p => p.ProductImages)
        //            .Include(p => p.ProductColor)
        //            .Include(p => p.Reviews)
        //                .ThenInclude(r => r.User)
        //            .ToListAsync();
        //        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // �������� ID �������� ������������

        //        if (userId != null)
        //        {
        //            // �������� ������ ��������� ������� ��� �������� ������������
        //            var favoriteProductIds = await _context.Favorites
        //                .Where(f => f.UserId == userId)
        //                .Select(f => f.ProductId)
        //                .ToListAsync();

        //            // ��������� ���������� � ���, ��������� �� ������ ������� � ���������
        //            foreach (var product in products)
        //            {
        //                product.IsInFavorites = favoriteProductIds.Contains(product.Id);
        //            }
        //        }
        //    }
        //    catch (Exception ex) // ����� ��������� ����������
        //    {
        //        // �������� ������ � ���������� �������� ������
        //        _logger.LogError(ex, "������ ��� ��������� ������ ���������.");
        //        return View("Error"); // ����������� �������� ������
        //    }
        //    return View(products);
        //    //return View();
        //}

        public async Task<IActionResult> Index()
        {
            // ������� ����� ������ ������ �������������
            var model = new ProductHomeIndexViewModel();

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // �������� ID �������� ������������

                // ������� ��������� � ���������� ����������� ������
                var products = await _context.Products
                    .Include(p => p.ProductType)
                    .Include(p => p.Brand)
                    .Include(p => p.Childcategory)
                    .Include(p => p.ProductImages)
                    .Include(p => p.ProductColor)
                    .Include(p => p.Reviews)
                    .ToListAsync();

                // ��������� ��� �������� �����
                var activeActions = await _context.Actions
                    .Where(a => a.StartDate <= DateTime.Now && a.EndDate >= DateTime.Now) // ������ �������� �����
                    .ToListAsync();

                // ����������� ���� �����, ���� ������� ����� �������� �����
                foreach (var product in products)
                {
                    var action = activeActions.FirstOrDefault(a => a.ProductId == product.Id);
                    if (action != null)
                    {
                        product.ActionPrice = action.NewPrice; // ������������� ���� �����
                    }
                }

                // 1. �������� 6 ������� � ���������� ���������
                model.GetProductsRating = products
                    .Where(p => p.Rating.HasValue)
                    .OrderByDescending(p => p.Rating)
                    .Take(6)
                    .ToList();

                // 2. �������� 6 ��������� �������
                model.GetProductsRandom = products
                    .OrderBy(p => Guid.NewGuid()) // ���������� ��������� �������
                    .Take(6)
                    .ToList();

                // 3. �������� 6 ��������� ������� ��� �����
                model.GetProductsActions = products
                    .Where(p => p.ActionPrice.HasValue) // ����� ������ �������� � �������
                    .OrderBy(p => Guid.NewGuid())
                    .Take(6)
                    .ToList();

                // 4. �������� 6 �������, ������� ���� ����� ����������� � ���������
                var favorProductIds = await _context.Favorites
                    .GroupBy(f => f.ProductId)
                    .OrderByDescending(g => g.Count()) // ��������� �� ���������� ���������� � ���������
                    .Take(6)
                    .Select(g => g.Key) // �������� Id ���������
                    .ToListAsync();

                // ������ ��������� ��������� ������ ��� ���� ���������
                model.GetProductsFavorites = await _context.Products
                    .Where(p => favorProductIds.Contains(p.Id))
                    .Include(p => p.ProductType)
                    .Include(p => p.Brand)
                    .Include(p => p.ProductImages)
                    .ToListAsync();

                //5.�������� 6 �������, ������� ���� ����� ��������
                var frequentlyBoughtProductIds = await _context.ShoppingList
                    .Include(sl => sl.Cart)
                        .ThenInclude(c => c.Items)
                    .SelectMany(sl => sl.Cart.Items) // �������� ��� ������ �� ������
                    .GroupBy(ci => ci.ProductId)
                    .OrderByDescending(g => g.Count()) // ��������� �� ���������� �������
                    .Take(6)
                    .Select(g => g.Key) // �������� ������ ProductId
                    .ToListAsync();

                // ������ ��������� ��������� ������ ��� ���� ���������
                model.GetProductsShopping = await _context.Products
                    .Where(p => frequentlyBoughtProductIds.Contains(p.Id))
                    .Include(p => p.ProductType)
                    .Include(p => p.Brand)
                    .Include(p => p.ProductImages)
                    .ToListAsync();

                //6. �������� ID ��������� "����, ������ �� ��������"
                var garmentsCategoryId = await _context.Categories
                    .Where(c => c.Name == "����, ������ �� ��������")
                    .Select(c => c.Id)
                    .FirstOrDefaultAsync();

                // ���� ��������� �������, �������� ��������
                if (garmentsCategoryId != 0)
                {
                    model.GetProductsGarments = await _context.Products
                        .Where(p => p.CategoryId == garmentsCategoryId || p.Childcategory.CategoryId == garmentsCategoryId)
                        .OrderBy(r => Guid.NewGuid()) // ��������� �������
                        .Take(6)
                        .Include(p => p.ProductType)
                        .Include(p => p.Brand)
                        .Include(p => p.ProductImages)
                        .ToListAsync();
                }

                // 7. ��������� ���������� �� ��������� ���������, ���� ������������ �����������
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
            catch (Exception ex) // ����� ��������� ����������
            {
                // �������� ������ � ���������� �������� ������
                _logger.LogError(ex, "������ ��� ��������� ������ ���������.");
                return View("Error"); // ����������� �������� ������
            }

            // ���������� ������ � �������������
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


        // ³������� ������� �����
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
