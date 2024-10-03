using System.Text.Json.Serialization;
using System.Text.Json;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using JsonSerializer = System.Text.Json.JsonSerializer;


namespace Rozetka.Extensions
{
    // Клас SessionExtensions є статичним класом, що містить розширення для
    // інтерфейсу ISession в ASP.NET Core.
    // Розширення надають зручний спосіб зберігати і отримувати об'єкти в/з сесії
    // за допомогою серіалізації і десеріалізації в JSON.

    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                WriteIndented = true,
                ReferenceHandler = ReferenceHandler.Preserve
            };
            string str = JsonSerializer.Serialize(value, options);
            session.SetString(key, str);
        }
        // Метод: Розширює інтерфейс ISession, додаючи можливість зберігати об'єкти в сесії.
        //Параметри:
        //ISession session: Об'єкт сесії, в якому зберігатимуться дані.
        //string key: Ключ, під яким зберігатиметься об'єкт у сесії.
        //T value: Об'єкт, який потрібно зберегти.
        //Процес:
        //Серіалізація: Створюється об'єкт JsonSerializerOptions для налаштування серіалізації.
        //В даному випадку, WriteIndented забезпечує форматування JSON з відступами,
        //а ReferenceHandler.Preserve управляє посиланнями між об'єктами.
        //Зберігання в сесії: Об'єкт value серіалізується в рядок JSON за
        //допомогою JsonSerializer.Serialize і зберігається в сесії за допомогою методу SetString.



        public static T? Get<T>(this ISession session, string key)
        {
            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                WriteIndented = true,
                ReferenceHandler = ReferenceHandler.Preserve
            };
            string? str = session.GetString(key);
            T? res = default;
            if (str is not null)
                res = JsonSerializer.Deserialize<T>(str, options);
            return res;
        }
        // Метод: Розширює інтерфейс ISession, надаючи можливість отримувати об'єкти з сесії.
        //Параметри:
        //ISession session:
        //Об'єкт сесії, з якого потрібно отримати дані.
        //string key:
        //Ключ, за яким зберігається об'єкт у сесії.
        //Процес:
        //Отримання з сесії:
        //Метод GetString використовується для отримання рядка JSON з сесії.
        //Десеріалізація:
        //Якщо рядок не є null, він десеріалізується назад в об'єкт типу T за допомогою JsonSerializer.Deserialize.


        public static void SetObject(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }

    }
}


//Серіалізація / Десеріалізація:
//Клас використовує JSON для серіалізації об'єктів перед зберіганням у сесії і для
//десеріалізації об'єктів після отримання з сесії. Це дозволяє зберігати складні об'єкти в сесії як рядки.
//Гнучкість:
//З використанням узагальнень (generic types) методи Set і Get можуть працювати
//з будь-яким типом даних, що забезпечує гнучкість при зберіганні різних об'єктів у сесії.
//Упрощення коду:
//Розширення спрощує код, дозволяючи зберігати і отримувати об'єкти з сесії за допомогою
//простих методів без необхідності вручну конвертувати дані в і з JSON.