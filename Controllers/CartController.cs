using Rozetka.Data;
//using Rozetka.Extensions;
using Rozetka.Models.ViewModels.Cart;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Rozetka.Data.Entity;
using System;
using System.Security.Policy;
using Microsoft.AspNetCore.Http;
using Rozetka.Extensions;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;

namespace Rozetka.Controllers
{
    public class CartController : Controller
    {

        //Контроллер CartController відповідає за управління кошиком покупок у програмі.
        //Він містить методи для перегляду, додавання, видалення товарів з кошика та інших операцій, пов'язаних з кошиком.
        private readonly UserManager<User> _userManager;
        private readonly DataContext _context;

        public CartController(UserManager<User> userManager, DataContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: CartController
        //public async Task<IActionResult> Index()
        //{
        //    Cart? cart;
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        //    if (userId != null)
        //    {
        //        var user = await _userManager.FindByIdAsync(userId); // Получаем текущего пользователя
        //        try
        //        {
        //            cart = await _context.Carts
        //                .Include(c => c.Items)
        //                    .ThenInclude(ci => ci.Product)
        //                        .ThenInclude(p => p.Brand)  // Подгружаем информацию из Brand
        //                .Include(c => c.Items)
        //                    .ThenInclude(ci => ci.Product)
        //                        .ThenInclude(p => p.ProductType)  // Подгружаем информацию из ProductType
        //                .Include(c => c.Items)
        //                    .ThenInclude(ci => ci.Product)
        //                        .ThenInclude(p => p.ProductImages)  // Подгружаем информацию из ProductImages
        //                .FirstOrDefaultAsync(c => c.UserId == userId && c.IsActive); // Только активная корзина

        //            //Получаем список избранных товаров для пользователя
        //            if (cart == null)
        //            {
        //                cart = new Cart { UserId = userId, Items = new List<CartItem>() };
        //                cart.IsActive = true;
        //                _context.Carts.Add(cart);
        //                await _context.SaveChangesAsync();
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine($"Ошибка при выполнении запроса: {ex.Message}");
        //            cart = new Cart { Items = new List<CartItem>() };
        //        }
        //    }
        //    else
        //    {
        //        // Логика для работы с сессионной корзиной
        //        var cartSession = HttpContext.Session.GetString("Cart");
        //        cart = string.IsNullOrEmpty(cartSession) ? new Cart { Items = new List<CartItem>() } : JsonConvert.DeserializeObject<Cart>(cartSession);

        //        // Загрузка полной информации для товаров в корзине
        //        foreach (var item in cart!.Items)
        //        {
        //            // Загрузим продукт из базы данных, чтобы получить информацию о Brand, ProductType и ProductImage
        //            var product = await _context.Products
        //                .Include(p => p.Brand)
        //                .Include(p => p.ProductType)
        //                .Include(p => p.ProductImages) // Подгружаем изображения продуктов
        //                .FirstOrDefaultAsync(p => p.Id == item.ProductId);

        //            if (product != null)
        //            {
        //                item.Product = product; // Обновляем продукт в корзине полной информацией
        //            }
        //        }
        //    }

        //    if (cart.Items == null)
        //    {
        //        cart.Items = new List<CartItem>();
        //    }

        //    double totalPrice = cart.Items.Sum(item => item.TotalPrice ?? 0);

        //    var viewModel = new CartViewModel
        //    {
        //        CartItems = cart.Items,
        //        TotalPrice = totalPrice                
        //    };

        //    if (userId == null)
        //    {
        //        var settings = new JsonSerializerSettings
        //        {
        //            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        //        };

        //        HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart, settings));
        //        //HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart));
        //    }
        //    else
        //    {
        //        var user = await _userManager.FindByIdAsync(userId); // Получаем текущего пользователя
        //        viewModel.FirstName = user!.FirstName; 
        //        viewModel.LastName = user!.LastName;
        //        viewModel.Email = user!.Email;
        //        viewModel.PhoneNumber = user!.PhoneNumber;
        //    }

        //    return View(viewModel);
        //}

        //Метод добавления товара в корзину
        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int count = 1)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                Cart? cart;

                if (userId != null)
                {
                    // Поиск корзины пользователя
                    cart = await _context.Carts
                        .Include(c => c.Items)
                        .ThenInclude(ci => ci.Product)
                        .FirstOrDefaultAsync(c => c.UserId == userId && c.IsActive);

                    if (cart == null)
                    {
                        // Создание корзины, если она не найдена
                        cart = new Cart { UserId = userId };
                        cart.IsActive = true; // делаем корзину активной до добавления в ShoppingList
                        _context.Carts.Add(cart);
                        await _context.SaveChangesAsync();
                    }
                }
                else
                {
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
                }

                // Проверяем, есть ли товар в корзине
                var existingCartItem = cart!.Items.FirstOrDefault(i => i.ProductId == productId);
                if (existingCartItem != null)
                {
                    // Обновляем количество, если товар уже в корзине
                    existingCartItem.Count += count;
                }
                else
                {
                    // Загружаем продукт с его связанными данными
                    var product = await _context.Products
                        .Include(p => p.Brand)
                        .Include(p => p.ProductType)
                        .Include(p => p.ProductImages)  // Подгружаем изображения
                        .FirstOrDefaultAsync(p => p.Id == productId);

                    if (product == null)
                    {
                        return NotFound("Товар не найден");
                    }

                    // Добавляем новый товар в корзину
                    cart.Items.Add(new CartItem
                    {
                        ProductId = productId,
                        Count = count,
                        Product = product // Сохраняем полный объект продукта с его связями
                    });
                }
                cart.TotalPrice = (decimal)cart.TotalCartPrice; //фиксируем общую цену
                if (userId != null)
                {
                    // Сохраняем изменения в базе данных
                    await _context.SaveChangesAsync();
                }
                else
                {
                    // Сохраняем изменения в сессии для неавторизованного пользователя
                    var settings = new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    };

                    HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart, settings));
                    
                    //HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart));
                }

                return Json(new { count = cart.Items.Sum(i => i.Count) });
            }
            catch (Exception ex)
            {
                // Логируем ошибку и возвращаем статус 500
                Console.WriteLine($"Ошибка при добавлении товара в корзину: {ex.Message}");
                return StatusCode(500, $"Произошла ошибка: {ex.Message}");
            }
        }

        //Метод удаления товара из корзины
        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Cart? cart;

            if (userId != null)
            {
                cart = await _context.Carts
                    .Include(c => c.Items)
                    .ThenInclude(ci => ci.Product)
                    .FirstOrDefaultAsync(c => c.UserId == userId);
            }
            else
            {
                var cartSession = HttpContext.Session.GetString("Cart");
                cart = string.IsNullOrEmpty(cartSession) ? new Cart() : JsonConvert.DeserializeObject<Cart>(cartSession);
            }

            if (cart == null) return NotFound("Корзина не найдена");

            // Удаляем товар из корзины
            var cartItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (cartItem != null)
            {
                cart.Items.Remove(cartItem);
                cart.TotalPrice = (decimal)cart.TotalCartPrice; //фиксируем общую цену
            }

            if (userId != null)
            {
                await _context.SaveChangesAsync();
            }
            else
            {
                // Сохраняем изменения в сессии для неавторизованного пользователя
                var settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };

                HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart, settings));
                //HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart));
            }

            // Подсчитываем обновлённую итоговую сумму
            double totalPrice = cart.Items.Sum(i => i.TotalPrice ?? 0);

            return Json(new { count = cart.Items.Sum(i => i.Count), totalPrice });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCartItem(int productId, int newCount)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                Cart? cart;

                if (userId != null)
                {
                    // Логика для авторизованных пользователей
                    cart = await _context.Carts
                        .Include(c => c.Items)
                        .ThenInclude(ci => ci.Product)
                        .FirstOrDefaultAsync(c => c.UserId == userId);

                    if (cart == null)
                        return BadRequest("Корзина не найдена");
                }
                else
                {
                    // Логика для неавторизованных пользователей
                    var cartSession = HttpContext.Session.GetString("Cart");
                    if (string.IsNullOrEmpty(cartSession))
                        return BadRequest("Корзина пуста");

                    cart = JsonConvert.DeserializeObject<Cart>(cartSession);
                }

                // Обновляем количество товара
                var cartItem = cart!.Items.FirstOrDefault(i => i.ProductId == productId);
                if (cartItem != null)
                {
                    cartItem.Count = newCount;
                    cart.TotalPrice = (decimal)cart.TotalCartPrice; //фиксируем общую цену
                }                

                // Для авторизованных пользователей сохраняем изменения в БД
                if (userId != null)
                {                    
                    await _context.SaveChangesAsync();
                }
                else
                {
                    // Для неавторизованных — обновляем сессию
                    var settings = new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    };

                    HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart, settings));
                }

                // Возвращаем обновленные данные
                var itemTotalPrice = cartItem!.TotalPrice ?? 0;
                var totalPrice = cart.Items.Sum(i => i.TotalPrice ?? 0);

                return Json(new { itemTotalPrice, totalPrice, count = cart.Items.Sum(i => i.Count) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> LoadCartModal()
        {
            Cart? cart;
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != null)
            {
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
                        .FirstOrDefaultAsync(c => c.UserId == userId && c.IsActive);

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
                foreach (var item in cart.Items)
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

            return PartialView("_CartModalPartial", viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetCartCount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Cart? cart;

            if (userId != null)
            {
                // Если пользователь авторизован, загружаем корзину из базы данных
                cart = await _context.Carts
                    .Include(c => c.Items)
                    .FirstOrDefaultAsync(c => c.UserId == userId && c.IsActive);

                if (cart == null)
                {
                    return Json(new { count = 0 });
                }
            }
            else
            {
                // Если пользователь не авторизован, используем сессионную корзину
                var cartSession = HttpContext.Session.GetString("Cart");

                cart = string.IsNullOrEmpty(cartSession) ? new Cart() : JsonConvert.DeserializeObject<Cart>(cartSession);
            }

            return Json(new { count = cart.Items.Sum(i => i.Count) });
        }

        // GET: Cart
        public async Task<IActionResult> Index()
        {
            return View(await _context.Carts
                .Include (c => c.Items)
                .Include(u => u.User)                
                .ToListAsync());
        }

        // GET: Cart/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Загружаем корзину вместе с её элементами
            var cart = await _context.Carts
                .Include(c => c.Items) // Включаем элементы корзины
                .FirstOrDefaultAsync(m => m.Id == id);

            if (cart == null)
            {
                return NotFound();
            }

            return View(cart); // Передаем корзину для подтверждения удаления
        }
        // POST: Cart/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Загружаем корзину вместе с её элементами
            var cart = await _context.Carts
                .Include(c => c.Items) // Включаем элементы корзины
                .FirstOrDefaultAsync(m => m.Id == id);

            if (cart == null)
            {
                return NotFound();
            }

            // Удаляем элементы корзины
            _context.CartItems.RemoveRange(cart.Items); // Удаляем все связанные CartItem

            // Удаляем саму корзину
            _context.Carts.Remove(cart);

            // Сохраняем изменения в базе данных
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index)); // Перенаправляем на страницу корзин
        }


    }   
}
