using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rozetka.Data;
using Rozetka.Data.Entity;
using System.Security.Claims;

namespace Rozetka.Controllers
{
    public class PurchaseController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly DataContext _context;

        public PurchaseController(UserManager<User> userManager, DataContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        // GET: PurchaseController/Index
        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Получаем ID текущего пользователя

                if (userId == null)
                {
                    return RedirectToAction("Login", "Account"); // Перенаправляем на страницу авторизации, если пользователь не авторизован
                }

                // Загружаем список покупок для текущего пользователя
                var shoppingLists = await _context.ShoppingList
                    .Include(s => s.Cart)
                        .ThenInclude(c => c.Items)
                            .ThenInclude(i => i.Product)
                                .ThenInclude(p => p.Brand)  // Подгружаем информацию о бренде
                    .Include(s => s.Cart)
                        .ThenInclude(c => c.Items)
                             .ThenInclude(i => i.Product)
                                .ThenInclude(p => p.ProductType)  // Подгружаем информацию о типе продукта
                    .Include(s => s.Cart)
                        .ThenInclude(c => c.Items)
                            .ThenInclude(i => i.Product)
                                .ThenInclude(p => p.ProductImages)  // Подгружаем изображения продуктов
                    .Where(s => s.UserId == userId)
                    .ToListAsync();

                return View(shoppingLists); // Возвращаем представление с данными
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        public async Task<ActionResult> AddPurchase() //добавление в список покупок
        {
            try
            {
                Cart? cart;
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (userId != null)
                {
                    cart = await _context.Carts
                       .Include(c => c.Items)
                           .ThenInclude(ci => ci.Product)
                               .ThenInclude(p => p.Brand)  // Подгружаем информацию из Brand
                       .Include(c => c.Items)
                           .ThenInclude(ci => ci.Product)
                               .ThenInclude(p => p.ProductType)  // Подгружаем информацию из ProductType
                       .Include(c => c.Items)
                           .ThenInclude(ci => ci.Product)
                               .ThenInclude(p => p.ProductImages)  // Подгружаем информацию из ProductImages
                       .FirstOrDefaultAsync(c => c.UserId == userId && c.IsActive); // Только активная корзина
                    if (cart == null)
                    {
                        return NotFound("Корзина не найдена или она уже была закрыта.");
                    }

                    // Проверяем наличие товаров на складе
                    foreach (var cartItem in cart.Items)
                    {
                        if (cartItem.Product!.QuantityInStock < cartItem.Count)
                        {
                            return BadRequest($"Недостаточно товара на складе: {cartItem.Product.Title}");
                        }
                    }
                    // Обновляем количество товаров на складе
                    foreach (var cartItem in cart.Items)
                    {
                        cartItem.Product!.QuantityInStock -= cartItem.Count;
                    }
                    // Создаем новый список покупок
                    var shoppingList = new ShoppingList
                    {
                        CartId = cart.Id,
                        Cart = cart,
                        DatePurchase = DateTime.Now,
                        TotalPrice = (decimal)cart.TotalCartPrice,
                        UserId = userId
                    };
                    // Добавляем список покупок в базу данных
                    _context.ShoppingList.Add(shoppingList);
                    // Делаем корзину неактивной
                    cart.IsActive = false;
                    // Сохраняем изменения в базе данных
                    await _context.SaveChangesAsync();

                    return RedirectToAction("SuccessOrder"); // Перенаправляем на страницу успешного оформления заказа
                }
                else
                {
                    
                }
                // Возвращаем сообщение об успешном добавлении товара в список покупок
                return Json(new { status = "Товары добавлены в список покупок" });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }

        }


        // GET: PurchaseController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: PurchaseController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PurchaseController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PurchaseController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PurchaseController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PurchaseController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PurchaseController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
