using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Rozetka.Data;
using Rozetka.Data.Entity;
using Rozetka.Models.ViewModels.Cart;
using System.Security.Claims;

namespace Rozetka.Controllers
{
    public class OrderController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly DataContext _context;
        public OrderController(UserManager<User> userManager, DataContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        public IActionResult SuccessOrder()
        {
            //return View("~/Views/Order/SuccessOrder.cshtml"); // Укажите полный путь к представлению
            return View();
        }
        public async Task<IActionResult> CreateOrder()
        {
            Cart? cart;
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != null)
            {
                var user = await _userManager.FindByIdAsync(userId); // Получаем текущего пользователя
                try
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

                    //Получаем список избранных товаров для пользователя
                    if (cart == null)
                    {
                        cart = new Cart { UserId = userId, Items = new List<CartItem>() };
                        cart.IsActive = true;
                        _context.Carts.Add(cart);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при выполнении запроса: {ex.Message}");
                    cart = new Cart { Items = new List<CartItem>() };
                }
            }
            else
            {
                // Логика для работы с сессионной корзиной
                var cartSession = HttpContext.Session.GetString("Cart");
                cart = string.IsNullOrEmpty(cartSession) ? new Cart { Items = new List<CartItem>() } : JsonConvert.DeserializeObject<Cart>(cartSession);

                // Загрузка полной информации для товаров в корзине
                foreach (var item in cart!.Items)
                {
                    // Загрузим продукт из базы данных, чтобы получить информацию о Brand, ProductType и ProductImage
                    var product = await _context.Products
                        .Include(p => p.Brand)
                        .Include(p => p.ProductType)
                        .Include(p => p.ProductImages) // Подгружаем изображения продуктов
                        .FirstOrDefaultAsync(p => p.Id == item.ProductId);

                    if (product != null)
                    {
                        item.Product = product; // Обновляем продукт в корзине полной информацией
                    }
                }
            }

            if (cart.Items == null)
            {
                cart.Items = new List<CartItem>();
            }

            double totalPrice = cart.Items.Sum(item => item.TotalPrice ?? 0);

            var viewModel = new CartViewModel
            {
                CartItems = cart.Items,
                TotalPrice = totalPrice
            };

            if (userId == null)
            {
                var settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };

                HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart, settings));
                //HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart));
            }
            else
            {
                var user = await _userManager.FindByIdAsync(userId); // Получаем текущего пользователя
                viewModel.FirstName = user!.FirstName;
                viewModel.LastName = user!.LastName;
                viewModel.Email = user!.Email;
                viewModel.PhoneNumber = user!.PhoneNumber;
            }

            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> ConfirmOrder()
        {
            // Получаем ID пользователя
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                Cart? cart;
                // Работа с сессионной корзиной для неавторизованного пользователя
                var cartSession = HttpContext.Session.GetString("Cart");
                if (string.IsNullOrEmpty(cartSession))
                {
                    cart = new Cart();
                }
                else
                {
                    cart = JsonConvert.DeserializeObject<Cart>(cartSession);
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
                    TotalPrice = (decimal)cart.TotalCartPrice                   
                };
                
                // Сохраняем изменения в сессии для неавторизованного пользователя
                var settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };
                                
                HttpContext.Session.SetString("ShoppingList", JsonConvert.SerializeObject(shoppingList, settings));
                HttpContext.Session.Remove("Cart");
                
                return RedirectToAction("SuccessOrder");
                //return BadRequest("Пользователь не авторизован.");
            }
            else
            {
                // Находим корзину пользователя
                var cart = await _context.Carts
                    .Include(c => c.Items)
                    .ThenInclude(ci => ci.Product)
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
        }
    }
}
