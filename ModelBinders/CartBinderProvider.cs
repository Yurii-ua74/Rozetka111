using Rozetka.ModelBinders;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Rozetka.Data.Entity;


namespace Rozetka.ModelBinders
{
    //public class CartBinderProvider : IModelBinderProvider
    //{
    //    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    //    {
    //        return context.Metadata.ModelType == typeof(Cart) ? new CartModelBinder() : null;
    //    }
    //}
}



//Інтерфейс IModelBinderProvider:
//IModelBinderProvider є частиною механізму моделювання в ASP.NET Core, який дозволяє
//надавати спеціальні біндери для моделей, використовуючи інтерфейс.
//Біндери відповідають за зв'язування (binding) даних з запиту HTTP до моделей у контролерах.

//Модель Cart:
//Клас CartBinderProvider створений для надання спеціального біндера для моделі Cart.
//Коли ASP.NET Core обробляє запит і потрібно зв'язати дані до моделі Cart,
//система перевіряє, чи є для цього біндер, який повертає CartBinderProvider.

//Біндер CartModelBinder:
//CartModelBinder - це клас, який реалізує IModelBinder і виконує фактичну логіку зв'язування даних
//до моделі Cart.
//Це може включати перетворення даних запиту в об'єкт Cart, наприклад, з параметрів запиту або даних форми.

//Налаштування:
//Клас CartBinderProvider зазвичай реєструється в конфігурації
//сервісів ASP.NET Core (зазвичай в Startup.cs або Program.cs).
//Коли система потребує біндера для Cart, вона викликає GetBinder і отримує
//відповідний біндер, якщо він доступний.