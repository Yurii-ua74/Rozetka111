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
        //Ініціалізує контроллер з контекстом бази даних DataContext, що дозволяє взаємодіяти з базою даних.
        //Викликається фреймворком при створенні екземпляра контролера.
        //Контекст бази даних ін'єктується через конструктор.

        // GET: CartController
        public async Task<IActionResult> Index()
        {
            // Получаем ID текущего пользователя
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Проверяем, что пользователь авторизован
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Получаем список избранных товаров для пользователя
            var carts = await _context.Carts
                .Where(c => c.UserId == userId)
                .Include(c => c.Product) // Подгружаем информацию о продукте
                .ThenInclude(p => p.ProductImages)
                .ToListAsync();

            // Возвращаем представление с данными
            return View(carts);
        }

        // GET: CartController
        public IActionResult Index(string? returnUrl)
        {
            Cart cart = GetCart();  // Отримання кошика з сесії
            CartIndexVM cartIndexVM = new CartIndexVM
            {
                CartItems = cart.CartItems,
                TotalPrice = cart.GetTotalPrice(),
                ReturnUrl = returnUrl ?? Url.Action("Index", "Home")
            };
            return View(cartIndexVM);
        }
        //Відображає вміст кошика покупок.
        //Викликається при переході до сторінки за URL Cart/Index.
        //Створює модель CartIndexVM, яка містить елементи кошика, загальну ціну та URL для повернення,
        //і передає її до представлення.


        public async Task<IActionResult> AddToCart(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Получаем ID текущего пользователя

            if (userId == null)
            {
                //var cart = GetCart();
                Product? product = await _context.Products.FindAsync(productId);

                if (product == null) return NotFound();

                // Завантаження зображень товару
                await _context.Entry(product).Collection(t => t.ProductImages!).LoadAsync();

                Cart cart = GetCart(); //отримання кошика з сесії
                cart.AddToCart(new CartItem { Product = product, Count = 1 });

                SetCart(cart);  // додавання кошика в сесію
                                //return Redirect(returnUrl);                              
                                //return Redirect(returnUrl ?? Url.Action("Index", "Cart"));
                var cartCount = cart.GetTotalCount();
                HttpContext.Session.SetInt32("CartItemCount", cartCount);

                return Json(new { success = true, cartCount });
            }
            else
            {
                // Проверяем, есть ли уже этот товар в корзине
                var existingCart = await _context.Carts
                    .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);
                if (existingCart == null) 
                {
                    var cartItem = new Cart
                    {
                        UserId = userId,
                        ProductId = productId
                    };

                    _context.Carts.Add(cartItem);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, cartItem });
                }
                else
                {
                    existingCart.Quantity++;
                    _context.Carts.Add(existingCart);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, existingCart });
                }
            }            
        }
        //Додає товар до кошика.
        //Викликається при додаванні товару до кошика за URL Cart/AddToCart/{ id}.
        //Отримує товар за його id, завантажує його зображення, додає товар до кошика та зберігає кошик у сесії.


        [HttpPost]
        public IActionResult RemoveFromCart(int id)
        {
            // Логика удаления товара
            var cart = GetCart();
            cart.RemoveFromCart(id);
            SetCart(cart);
            var cartItemCount = cart.GetTotalCount(); // Получите актуальное количество товаров в корзине
            var totalPrice = cart.GetTotalPrice(); // Рассчитайте общую стоимость

            // Возвращаем актуальные данные
            return Json(new
            {
                success = true,
                cartItemCount = cart.GetTotalCount(),
                totalPrice = cart.GetTotalPrice() 
            });
        }
        //Видаляє товар з кошика.
        //Викликається при видаленні товару з кошика (метод POST) за URL Cart/RemoveFromCart.
        //Видаляє товар за його id з кошика та зберігає кошик у сесії.


        public IActionResult SetCountry()
        {
            HttpContext.Session.SetString("country", "Ukraine");
            return View();
        }
        //Встановлює значення країни у сесію.
        //Викликається при встановленні країни за URL Cart/SetCountry.


        public IActionResult GetCountry()
        {
            string country = HttpContext.Session.GetString("country") ?? "Session key country was not set!";
            return View(model: country);
        }
        //Отримує значення країни з сесії.
        //Викликається при отриманні країни за URL Cart/GetCountry.
        //Отримує значення країни з сесії та передає його до представлення.


        public Cart GetCart()
        {
            string sessionKey = "cart";
            IEnumerable<CartItem>? cartItems = HttpContext.Session.Get<IEnumerable<CartItem>>(sessionKey);
            if (cartItems == null)
            {
                cartItems = new List<CartItem>();
                //HttpContext.Session.Set(sessionKey, cartItems);
            }
            Cart cart = new Cart(cartItems);

            // Збереження кількості товарів у сесії
            int totalItemsCount = cartItems.Sum(item => item.Count);
            HttpContext.Session.SetInt32("CartItemCount", totalItemsCount);

            return cart;

            //return new Cart(cartItems);
        }
        //Отримує кошик з сесії.
        //Викликається всередині контролера.
        //Отримує елементи кошика з сесії.Якщо кошик порожній, створює новий і зберігає його у сесії.




        // Эагрузка корзины в модальном окне
        public IActionResult LoadCartModal()
        {
            Cart cart = GetCart(); // Получаем корзину из сессии
            CartIndexVM cartIndexVM = new CartIndexVM
            {
                CartItems = cart.CartItems,
                TotalPrice = cart.GetTotalPrice()
            };
            return PartialView("_CartModalPartial", cartIndexVM);  // Возвращаем частичное представление
        }




        public void SetCart(Cart cart)
        {
            HttpContext.Session.Set("cart", cart.CartItems);
        }
        //Зберігає кошик у сесії.
        //Викликається всередині контролера після зміни кошика.


        [HttpPost]
        public IActionResult IncCount(int id, Cart cart)
        {
            CartItem? cartItem = cart.CartItems.FirstOrDefault(t => t.Product.Id == id);
            if (cartItem == null)
                return NotFound();
            cart.IncCount(cartItem.Product.Id);
            SetCart(cart);
            return Ok(new { cartItem.Count, cartItem.TotalPrice });
        }
        //Збільшує кількість товару в кошику.
        //Викликається при збільшенні кількості товару в кошику(метод POST) за URL Cart/IncCount.
        //Збільшує кількість товару за його id в кошику, зберігає кошик у сесії та повертає оновлені дані.

        [HttpPost]
        public IActionResult DecCount(int id, Cart cart)
        {
            CartItem? cartItem = cart.CartItems.FirstOrDefault(t => t.Product.Id == id);
            if (cartItem == null)
                return NotFound();
            cart.DecCount(cartItem.Product.Id);
            SetCart(cart);
            return Ok(new { cartItem.Count, cartItem.TotalPrice });
        }
        //Зменшує кількість товару в кошику.
        //Викликається при зменшенні кількості товару в кошику(метод POST) за URL Cart/DecCount.
        //Зменшує кількість товару за його id в кошику, зберігає кошик у сесії та повертає оновлені дані.


        [HttpPost]
        public IActionResult getTotalPrice(Cart cart)
        {
            return Ok(new { TotalPrice = cart.GetTotalPrice() });
        }
        //Отримує загальну ціну товарів у кошику.
        //Викликається при запиті загальної ціни товарів у кошику (метод POST) за URL Cart/getTotalPrice.
        //Отримує загальну ціну товарів у кошику та повертає її.

        // отримувати кількість товарів у кошику з сесії
        //[HttpGet]
        //public IActionResult GetCartCount()
        //{
        //    int? cartCount = HttpContext.Session.GetInt32("CartItemCount");
        //    return Json(new { cartCount = cartCount ?? 0 });
        //}


        // Метод для получения количества избранных товаров
        public async Task<int> GetCartCount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Получаем ID текущего пользователя

            if (userId == null)
            {
                return 0;
            }

            // Получаем количество товаров в избранном
            var count = await _context.Carts
                .Where(f => f.UserId == userId)
                .CountAsync();

            return count;
        }
    }
}
