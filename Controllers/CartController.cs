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

namespace Rozetka.Controllers
{
    public class CartController : Controller
    {

        //Контроллер CartController відповідає за управління кошиком покупок у програмі.
        //Він містить методи для перегляду, додавання, видалення товарів з кошика та інших операцій, пов'язаних з кошиком.

        private readonly DataContext _context;

        public CartController(DataContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            Cart cart;
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != null)
            {
                try
                {
                    cart = await _context.Carts
                        .Include(c => c.Items)
                        .ThenInclude(ci => ci.Product)
                        .FirstOrDefaultAsync(c => c.UserId == userId);

                    if (cart == null)
                    {
                        Console.WriteLine($"Корзина для пользователя {userId} не найдена.");
                        cart = new Cart { UserId = userId, Items = new List<CartItem>() }; // Инициализация пустого списка Items
                        _context.Carts.Add(cart);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        Console.WriteLine($"Корзина найдена. Количество товаров: {cart.Items.Count}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при выполнении запроса: {ex.Message}");
                    cart = new Cart { Items = new List<CartItem>() }; // В случае ошибки возвращаем пустую корзину
                }
            }
            else
            {
                var cartSession = HttpContext.Session.GetString("Cart");
                cart = string.IsNullOrEmpty(cartSession) ? new Cart { Items = new List<CartItem>() } : JsonConvert.DeserializeObject<Cart>(cartSession);
            }

            // Убедитесь, что Items не равен null, чтобы избежать ошибок
            if (cart.Items == null)
            {
                cart.Items = new List<CartItem>();
            }

            // Подсчет общей суммы товаров в корзине
            double totalPrice = cart.Items.Sum(item => item.TotalPrice ?? 0);

            // Подготовка данных для передачи в представление
            var viewModel = new CartViewModel
            {
                CartItems = cart.Items,
                TotalPrice = totalPrice
            };

            // Сохраняем корзину в сессии, если пользователь не авторизован
            if (userId == null)
            {
                HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart));
            }

            // Возвращаем частичное представление с данными корзины
            return PartialView("_CartModalPartial", viewModel);
        }

        //Метод добавления товара в корзину
        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int count = 1)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                Cart cart;

                if (userId != null)
                {
                    // Поиск корзины пользователя
                    cart = await _context.Carts
                        .Include(c => c.Items)
                        .ThenInclude(ci => ci.Product)
                        .FirstOrDefaultAsync(c => c.UserId == userId);

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
                var existingCartItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);
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
            Cart cart;

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
            // Получаем ID пользователя
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Cart cart;

            if (userId != null)
            {
                // Если пользователь авторизован, ищем его корзину в базе данных
                cart = await _context.Carts
                    .Include(c => c.Items)
                    .ThenInclude(ci => ci.Product)                    
                    .FirstOrDefaultAsync(c => c.UserId == userId);               
            }
            else
            {
                // Если пользователь не авторизован, работаем с сессионной корзиной
                var cartSession = HttpContext.Session.GetString("Cart");
                cart = string.IsNullOrEmpty(cartSession) ? new Cart() : JsonConvert.DeserializeObject<Cart>(cartSession);
            }

            if (cart == null) return NotFound("Корзина не найдена");

            // Ищем товар в корзине
            var cartItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (cartItem != null)
            {
                cartItem.Count = newCount; // Обновляем количество товара
                //cartItem.TotalPrice = (double)(cartItem.Product.Price * newCount); ;
                if (cartItem.Count <= 0)
                {
                    cart.Items.Remove(cartItem); // Удаляем товар, если количество 0 или меньше
                }
            }

            // Сохраняем изменения
            if (userId != null)
            {
                await _context.SaveChangesAsync();
            }
            else
            {
                var settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };

                HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart, settings));
                //HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart));
            }

            //// Подсчитываем обновлённую итоговую сумму
            double totalPrice = cart.Items.Sum(i => i.TotalPrice ?? 0);
            //return Json(new { count = cart.Items.Sum(i => i.Count), totalPrice });

            //Возвращаем новую цену товара и общую сумму корзины
            return Json(new { itemTotalPrice = cartItem.TotalPrice, totalPrice, count = cart.Items.Sum(i => i.Count) });
        }

        [HttpGet]
        public async Task<IActionResult> LoadCartModal()
        {
            Cart cart;
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != null)
            {
                try
                {
                    // Загрузка корзины для авторизованного пользователя
                    //cart = await _context.Carts
                    //    .Include(c => c.Items)
                    //    .ThenInclude(ci => ci.Product)
                    //    .ThenInclude(p => p.Brand)  // Только связь с брендом
                    //    .FirstOrDefaultAsync(c => c.UserId == userId);

                    cart = await _context.Carts
                        .Include(c => c.Items)
                        .ThenInclude(ci => ci.Product)
                        .ThenInclude(p => p.Brand)  // Подгружаем информацию из Brand
                        .Include(c => c.Items)
                        .ThenInclude(ci => ci.Product)
                        .ThenInclude(p => p.ProductType)  // Подгружаем информацию из ProductType
                        .Include(c => c.Items)
                        .ThenInclude(ci => ci.Product)
                        .ThenInclude(p => p.ProductColor)  // Подгружаем информацию из ProductColor
                        .FirstOrDefaultAsync(c => c.UserId == userId);

                    //Получаем список избранных товаров для пользователя
                    if (cart == null)
                    {
                        cart = new Cart { UserId = userId, Items = new List<CartItem>() };
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
            Cart cart;

            if (userId != null)
            {
                // Если пользователь авторизован, загружаем корзину из базы данных
                cart = await _context.Carts
                    .Include(c => c.Items)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

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
    }

    //public class CartController : Controller
    //{

    //    //Контроллер CartController відповідає за управління кошиком покупок у програмі.
    //    //Він містить методи для перегляду, додавання, видалення товарів з кошика та інших операцій, пов'язаних з кошиком.

    //    private readonly DataContext _context;

    //    public CartController(DataContext context)
    //    {
    //        _context = context;
    //    }
    //    //Ініціалізує контроллер з контекстом бази даних DataContext, що дозволяє взаємодіяти з базою даних.
    //    //Викликається фреймворком при створенні екземпляра контролера.
    //    //Контекст бази даних ін'єктується через конструктор.

    //    // GET: CartController
    //    //public async Task<IActionResult> Index()
    //    //{
    //    //    // Получаем ID текущего пользователя
    //    //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

    //    //    // Проверяем, что пользователь авторизован
    //    //    if (userId == null)
    //    //    {
    //    //        return RedirectToAction("Login", "Account");
    //    //    }

    //    //    // Получаем список избранных товаров для пользователя
    //    //    var carts = await _context.Carts
    //    //        .Where(c => c.UserId == userId)
    //    //        .Include(c => c.Product) // Подгружаем информацию о продукте
    //    //        .ThenInclude(p => p.ProductImages)
    //    //        .ToListAsync();

    //    //    // Возвращаем представление с данными
    //    //    return View(carts);
    //    //}

    //    // GET: CartController
    //    public IActionResult Index(string? returnUrl)
    //    {
    //        Cart cart = GetCart();  // Отримання кошика з сесії
    //        CartIndexVM cartIndexVM = new CartIndexVM
    //        {
    //            CartItems = cart.CartItems,
    //            TotalPrice = cart.GetTotalPrice(),
    //            ReturnUrl = returnUrl ?? Url.Action("Index", "Home")
    //        };
    //        return View(cartIndexVM);
    //    }
    //    //Відображає вміст кошика покупок.
    //    //Викликається при переході до сторінки за URL Cart/Index.
    //    //Створює модель CartIndexVM, яка містить елементи кошика, загальну ціну та URL для повернення,
    //    //і передає її до представлення.


    //    public async Task<IActionResult> AddToCart(int productId)
    //    {
    //        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Получаем ID текущего пользователя

    //        if (userId == null)
    //        {
    //            //var cart = GetCart();
    //            Product? product = await _context.Products.FindAsync(productId);

    //            if (product == null) return NotFound();

    //            // Завантаження зображень товару
    //            await _context.Entry(product).Collection(t => t.ProductImages!).LoadAsync();

    //            Cart cart = GetCart(); //отримання кошика з сесії
    //            cart.AddToCart(new CartItem { Product = product, Count = 1 });

    //            SetCart(cart);  // додавання кошика в сесію
    //                            //return Redirect(returnUrl);                              
    //                            //return Redirect(returnUrl ?? Url.Action("Index", "Cart"));
    //            var cartCount = cart.GetTotalCount();
    //            HttpContext.Session.SetInt32("CartItemCount", cartCount);

    //            return Json(new { success = true, cartCount });
    //        }
    //        else
    //        {
    //            // Проверяем, есть ли уже этот товар в корзине
    //            var existingCart = await _context.Carts
    //                .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);
    //            if (existingCart == null)
    //            {
    //                var cartItem = new Cart
    //                {
    //                    UserId = userId,
    //                    ProductId = productId
    //                };

    //                _context.Carts.Add(cartItem);
    //                await _context.SaveChangesAsync();
    //                return Json(new { success = true, cartItem });
    //            }
    //            else
    //            {
    //                existingCart.Quantity++;
    //                _context.Carts.Add(existingCart);
    //                await _context.SaveChangesAsync();
    //                return Json(new { success = true, existingCart });
    //            }
    //        }
    //    }
    //    //Додає товар до кошика.
    //    //Викликається при додаванні товару до кошика за URL Cart/AddToCart/{ id}.
    //    //Отримує товар за його id, завантажує його зображення, додає товар до кошика та зберігає кошик у сесії.


    //    [HttpPost]
    //    public IActionResult RemoveFromCart(int id)
    //    {
    //        // Логика удаления товара
    //        var cart = GetCart();
    //        cart.RemoveFromCart(id);
    //        SetCart(cart);
    //        var cartItemCount = cart.GetTotalCount(); // Получите актуальное количество товаров в корзине
    //        var totalPrice = cart.GetTotalPrice(); // Рассчитайте общую стоимость

    //        // Возвращаем актуальные данные
    //        return Json(new
    //        {
    //            success = true,
    //            cartItemCount = cart.GetTotalCount(),
    //            totalPrice = cart.GetTotalPrice()
    //        });
    //    }
    //    //Видаляє товар з кошика.
    //    //Викликається при видаленні товару з кошика (метод POST) за URL Cart/RemoveFromCart.
    //    //Видаляє товар за його id з кошика та зберігає кошик у сесії.


    //    public IActionResult SetCountry()
    //    {
    //        HttpContext.Session.SetString("country", "Ukraine");
    //        return View();
    //    }
    //    //Встановлює значення країни у сесію.
    //    //Викликається при встановленні країни за URL Cart/SetCountry.


    //    public IActionResult GetCountry()
    //    {
    //        string country = HttpContext.Session.GetString("country") ?? "Session key country was not set!";
    //        return View(model: country);
    //    }
    //    //Отримує значення країни з сесії.
    //    //Викликається при отриманні країни за URL Cart/GetCountry.
    //    //Отримує значення країни з сесії та передає його до представлення.


    //    public Cart GetCart()
    //    {
    //        string sessionKey = "cart";
    //        IEnumerable<CartItem>? cartItems = HttpContext.Session.Get<IEnumerable<CartItem>>(sessionKey);
    //        if (cartItems == null)
    //        {
    //            cartItems = new List<CartItem>();
    //            //HttpContext.Session.Set(sessionKey, cartItems);
    //        }
    //        Cart cart = new Cart(cartItems);

    //        // Збереження кількості товарів у сесії
    //        int totalItemsCount = cartItems.Sum(item => item.Count);
    //        HttpContext.Session.SetInt32("CartItemCount", totalItemsCount);

    //        return cart;

    //        //return new Cart(cartItems);
    //    }
    //    //Отримує кошик з сесії.
    //    //Викликається всередині контролера.
    //    //Отримує елементи кошика з сесії.Якщо кошик порожній, створює новий і зберігає його у сесії.




    //    // Эагрузка корзины в модальном окне
    //    public IActionResult LoadCartModal()
    //    {
    //        Cart cart = GetCart(); // Получаем корзину из сессии
    //        CartIndexVM cartIndexVM = new CartIndexVM
    //        {
    //            CartItems = cart.CartItems,
    //            TotalPrice = cart.GetTotalPrice()
    //        };
    //        return PartialView("_CartModalPartial", cartIndexVM);  // Возвращаем частичное представление
    //    }




    //    public void SetCart(Cart cart)
    //    {
    //        HttpContext.Session.Set("cart", cart.CartItems);
    //    }
    //    //Зберігає кошик у сесії.
    //    //Викликається всередині контролера після зміни кошика.


    //    [HttpPost]
    //    public IActionResult IncCount(int id, Cart cart)
    //    {
    //        CartItem? cartItem = cart.CartItems.FirstOrDefault(t => t.Product.Id == id);
    //        if (cartItem == null)
    //            return NotFound();
    //        cart.IncCount(cartItem.Product.Id);
    //        SetCart(cart);
    //        return Ok(new { cartItem.Count, cartItem.TotalPrice });
    //    }
    //    //Збільшує кількість товару в кошику.
    //    //Викликається при збільшенні кількості товару в кошику(метод POST) за URL Cart/IncCount.
    //    //Збільшує кількість товару за його id в кошику, зберігає кошик у сесії та повертає оновлені дані.

    //    [HttpPost]
    //    public IActionResult DecCount(int id, Cart cart)
    //    {
    //        CartItem? cartItem = cart.CartItems.FirstOrDefault(t => t.Product.Id == id);
    //        if (cartItem == null)
    //            return NotFound();
    //        cart.DecCount(cartItem.Product.Id);
    //        SetCart(cart);
    //        return Ok(new { cartItem.Count, cartItem.TotalPrice });
    //    }
    //    //Зменшує кількість товару в кошику.
    //    //Викликається при зменшенні кількості товару в кошику(метод POST) за URL Cart/DecCount.
    //    //Зменшує кількість товару за його id в кошику, зберігає кошик у сесії та повертає оновлені дані.


    //    [HttpPost]
    //    public IActionResult getTotalPrice(Cart cart)
    //    {
    //        return Ok(new { TotalPrice = cart.GetTotalPrice() });
    //    }
    //    //Отримує загальну ціну товарів у кошику.
    //    //Викликається при запиті загальної ціни товарів у кошику (метод POST) за URL Cart/getTotalPrice.
    //    //Отримує загальну ціну товарів у кошику та повертає її.

    //    // отримувати кількість товарів у кошику з сесії
    //    //[HttpGet]
    //    //public IActionResult GetCartCount()
    //    //{
    //    //    int? cartCount = HttpContext.Session.GetInt32("CartItemCount");
    //    //    return Json(new { cartCount = cartCount ?? 0 });
    //    //}


    //    // Метод для получения количества избранных товаров
    //    public async Task<int> GetCartCount()
    //    {
    //        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Получаем ID текущего пользователя

    //        if (userId == null)
    //        {
    //            return 0;
    //        }

    //        // Получаем количество товаров в избранном
    //        var count = await _context.Carts
    //            .Where(f => f.UserId == userId)
    //            .CountAsync();

    //        return count;
    //    }
    //}
}
