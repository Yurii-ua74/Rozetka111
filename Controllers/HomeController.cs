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
                // ��������� ������ ��������� � ��������� ������
                products = await _context.Products
                    .Include(p => p.ProductType)
                    .Include(p => p.Brand)
                    .Include(p => p.Childcategory)
                    .Include(p => p.ProductImages)
                    .Include(p => p.ProductColor)
                    .Include(p => p.Reviews)
                        .ThenInclude(r => r.User)
                    .ToListAsync();
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // �������� ID �������� ������������

                if (userId != null)
                {
                    // �������� ������ ��������� ������� ��� �������� ������������
                    var favoriteProductIds = await _context.Favorites
                        .Where(f => f.UserId == userId)
                        .Select(f => f.ProductId)
                        .ToListAsync();

                    // ��������� ���������� � ���, ��������� �� ������ ������� � ���������
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
            return View(products);
            //return View();
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
