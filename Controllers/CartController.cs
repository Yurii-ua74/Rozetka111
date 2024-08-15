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

namespace Rozetka.Controllers
{
    public class CartController : Controller
    {

        //Контроллер CartController відповідає за управління кошиком покупок у програмі.
        //Він містить методи для перегляду, додавання, видалення товарів з кошика та інших операцій, пов'язаних з кошиком.

        private readonly DataContext context;

        public CartController(DataContext context)
        {
            this.context = context;
        }
        //Ініціалізує контроллер з контекстом бази даних DataContext, що дозволяє взаємодіяти з базою даних.
        //Викликається фреймворком при створенні екземпляра контролера.
        //Контекст бази даних ін'єктується через конструктор.


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


        public async Task<IActionResult> AddToCart(int id, string? returnUrl)
        {
            //var cart = GetCart();
            Product? product = await context.Products.FindAsync(id);

            if (product == null)   return NotFound();

            // Завантаження зображень товару
            await context.Entry(product).Collection(t => t.ProductImages!).LoadAsync();

            Cart cart = GetCart(); //отримання кошика з сесії
            cart.AddToCart(new CartItem { Product = product, Count = 1 });

            SetCart(cart);  // додавання кошика в сесію
            //return Redirect(returnUrl);                              
            //return Redirect(returnUrl ?? Url.Action("Index", "Cart"));
            var cartCount = cart.GetTotalCount();
            HttpContext.Session.SetInt32("CartItemCount", cartCount);

            return Json(new { success = true, cartCount });
        }
        //Додає товар до кошика.
        //Викликається при додаванні товару до кошика за URL Cart/AddToCart/{ id}.
        //Отримує товар за його id, завантажує його зображення, додає товар до кошика та зберігає кошик у сесії.
        

        [HttpPost]
        public IActionResult RemoveFromCart(Cart cart, int? id, string? returnUrl)
        {
            if (id == null) return NotFound();
            //Cart cart = GetCart();
            cart.RemoveFromCart(id.Value);
            SetCart(cart);
            return RedirectToAction("Index", new { returnUrl });
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
        [HttpGet]
        public IActionResult GetCartCount()
        {
            int? cartCount = HttpContext.Session.GetInt32("CartItemCount");
            return Json(new { cartCount = cartCount ?? 0 });
        }
    }
}
