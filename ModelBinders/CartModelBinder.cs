using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Rozetka.Extensions;
using Rozetka.Data.Entity;

namespace Rozetka.ModelBinders
{
    public class CartModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
                throw new ArgumentNullException(nameof(bindingContext));

            string sessionKey = "cart";
            IEnumerable<CartItem>? cartItems = null;
            cartItems = bindingContext.
                HttpContext.Session.Get<IEnumerable<CartItem>>(sessionKey);

            if (cartItems == null)
            {
                cartItems = new List<CartItem>();
                bindingContext.HttpContext.Session.Set(sessionKey, cartItems);
            }
            Cart cart = new Cart(cartItems);
            bindingContext.Result = ModelBindingResult.Success(cart);
            return Task.CompletedTask;
        }
    }
}


//Метод: Реалізує метод BindModelAsync інтерфейсу IModelBinder.
//Цей метод відповідає за асинхронне зв'язування даних до моделі Cart.

//Параметри:

//ModelBindingContext bindingContext:
//Контекст, що містить інформацію про модель, яку потрібно зв'язати, та дані запиту.

//Процес:

//Перевірка bindingContext: Якщо bindingContext є null, викидається виключення ArgumentNullException.

//Отримання даних з сесії:

//Визначається ключ сесії sessionKey зі значенням "cart".
//Отримання списку елементів кошика (cartItems) з сесії за допомогою ключа.
//Якщо дані не знайдено (cartItems == null), створюється новий порожній список.

//Зберігання даних у сесії:

//Якщо дані кошика (cartItems) не були знайдені, новий список елементів кошика зберігається
//в сесії за допомогою ключа sessionKey.

//Створення об'єкта Cart:

//Створюється новий об'єкт Cart на основі отриманих (або нових) елементів кошика.

//Результат моделі:

//Встановлюється результат біндінгу (bindingContext.Result) як успішний (ModelBindingResult.Success)
//з об'єктом Cart.

//Результат:

//Повертає Task.CompletedTask, що сигналізує про завершення асинхронної операції.

//Як це працює
//Інтерфейс IModelBinder:
//Інтерфейс IModelBinder є частиною механізму моделювання в ASP.NET Core.
//Він дозволяє налаштувати, як дані з запиту перетворюються в об'єкти моделей.

//Біндинг моделі Cart: CartModelBinder забезпечує специфічний спосіб зв'язування даних до моделі Cart.
//Це корисно, коли модель Cart зберігається в сесії або іншому сховищі даних.

//Реалізація: Клас використовує сесію для отримання або створення кошика.
//Якщо кошик не знайдено в сесії, створюється новий, який потім зберігається в сесії.

//Контекст: ModelBindingContext містить інформацію про модель, що зв'язується, та результати.
//Він надає доступ до HTTP-контексту і дозволяє зберігати результат моделі.
