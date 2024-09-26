using Microsoft.EntityFrameworkCore;
using Rozetka.Data.Entity;

namespace Rozetka.Data
{
    public static class DbInitializer
    {
        public static void Initialize(DataContext context)
        {
            //context.Database.EnsureCreated();
            context.Database.Migrate();

            // Перевіряємо, чи є вже дані в базі, щоб уникнути дублювання
            if (context.Categories.Any())
            {
                return;   // DB вже заповнена
            }

            var categories = new Category[]
            {
            new Category { Name = "Ноутбуки та комп'ютери" },
            new Category { Name = "Смартфони, ТВ і електроніка" },
            new Category { Name = "Товари для геймерів" },
            new Category { Name = "Побутова техніка" },
            new Category { Name = "Товари для дому" },
            new Category { Name = "Інструменти та автотовари" },
            new Category { Name = "Сантехніка та ремонт" },
            new Category { Name = "Дача, сад і город" },
            new Category { Name = "Спорт і захоплення" },
            new Category { Name = "Одяг, взуття та прикраси" },
            new Category { Name = "Краса та здоров'я" },
            new Category { Name = "Дитячі товари" },
            new Category { Name = "Зоотовари" },
            new Category { Name = "Офіс, школа, книги" },
            new Category { Name = "Алкогольні напої та продукти" },
            new Category { Name = "Побутова хімія" }
            };            

            foreach (var category in categories)
            {
                context.Categories.Add(category);
            }
            context.SaveChanges();

            // Створюємо підкатегорії (ChildCategories), що прив'язані до категорій
            var childCategories = new Childcategory[]
            {
            //Ноутбуки та комп'ютери
            new Childcategory { Name = "Ноутбуки", CategoryId = categories[0].Id }, // Для "Ноутбуки та комп'ютери"
            new Childcategory { Name = "Комп'ютери, неттопи, моноблоки", CategoryId = categories[0].Id },
            new Childcategory { Name = "Gaming", CategoryId = categories[0].Id },
            new Childcategory { Name = "Планшети", CategoryId = categories[0].Id },
            new Childcategory { Name = "Кабелі та перехідники", CategoryId = categories[0].Id },
            new Childcategory { Name = "Комплектуючі", CategoryId = categories[0].Id },
            new Childcategory { Name = "Аксесуари для ноутбуків та ПК", CategoryId = categories[0].Id },
            new Childcategory { Name = "Офісна техніка", CategoryId = categories[0].Id },
            new Childcategory { Name = "Мережеве обладнання", CategoryId = categories[0].Id },
            //Смартфони, ТВ і електроніка
            new Childcategory { Name = "Мобільні телефони", CategoryId = categories[1].Id },
            new Childcategory { Name = "Павербанки та зарядні станції", CategoryId = categories[1].Id },
            new Childcategory { Name = "Аксесуари для телефонів", CategoryId = categories[1].Id },
            new Childcategory { Name = "Телевізори", CategoryId = categories[1].Id },
            new Childcategory { Name = "Фото і відео", CategoryId = categories[1].Id },
            new Childcategory { Name = "Портативна електроніка", CategoryId = categories[1].Id },
            new Childcategory { Name = "Аудіотехніка", CategoryId = categories[1].Id },
            //Товари для геймерів
            new Childcategory { Name = "Повербанки та зарядні станції", CategoryId = categories[2].Id },
            new Childcategory { Name = "Ігрові приставки", CategoryId = categories[2].Id },
            new Childcategory { Name = "Ігрові ноутбуки", CategoryId = categories[2].Id },
            new Childcategory { Name = "PlayStation", CategoryId = categories[2].Id },
            new Childcategory { Name = "Ігрові комп'ютери", CategoryId = categories[2].Id },
            new Childcategory { Name = "Комплектуючі для геймерів", CategoryId = categories[2].Id },
            new Childcategory { Name = "Атрибутика та сувеніри", CategoryId = categories[2].Id },
            new Childcategory { Name = "Ігрова переферія", CategoryId = categories[2].Id },
            new Childcategory { Name = "Ігрові монітори", CategoryId = categories[2].Id },
            //Побутова техніка
            new Childcategory { Name = "Велика побутова техніка", CategoryId = categories[3].Id },
            new Childcategory { Name = "Догляд та прибирання", CategoryId = categories[3].Id },
            new Childcategory { Name = "Кліматична техніка", CategoryId = categories[3].Id },
            new Childcategory { Name = "Краса і здоров’я", CategoryId = categories[3].Id },
            new Childcategory { Name = "Кухня", CategoryId = categories[3].Id },
            //Товари для дому 
            new Childcategory { Name = "Домашній текстиль", CategoryId = categories[4].Id },
            new Childcategory { Name = "Посуд", CategoryId = categories[4].Id },
            new Childcategory { Name = "Побутова хімія", CategoryId = categories[4].Id },
            new Childcategory { Name = "Меблі", CategoryId = categories[4].Id },
            new Childcategory { Name = "Дитяча кімната", CategoryId = categories[4].Id },
            new Childcategory { Name = "Каміни, печі, сауни", CategoryId = categories[4].Id },
            //Інструменти та автотовари
            new Childcategory { Name = "Інструменти", CategoryId = categories[5].Id },
            new Childcategory { Name = "Витратні матеріали та приладдя", CategoryId = categories[5].Id },
            new Childcategory { Name = "Устаткування", CategoryId = categories[5].Id },
            new Childcategory { Name = "Ручний інструмент", CategoryId = categories[5].Id },
            new Childcategory { Name = "Автотовари", CategoryId = categories[5].Id },
            //Сантехніка та ремонт
            new Childcategory { Name = "Сантехніка та меблі для ванної", CategoryId = categories[6].Id },
            new Childcategory { Name = "Будівельні матеріали", CategoryId = categories[6].Id },
            new Childcategory { Name = "Інтер'єр та оздоблення", CategoryId = categories[6].Id },
            new Childcategory { Name = "Освітлення", CategoryId = categories[6].Id },
            //Дача, сад і город
            new Childcategory { Name = "Садова техніка", CategoryId = categories[7].Id },
            new Childcategory { Name = "Садовий інвертар", CategoryId = categories[7].Id },
            new Childcategory { Name = "Системи поливу", CategoryId = categories[7].Id },
            new Childcategory { Name = "Рослини і догляд за ними", CategoryId = categories[7].Id },
            //Спорт і захоплення
            new Childcategory { Name = "Велосипеди та акксесуари", CategoryId = categories[8].Id },
            new Childcategory { Name = "Електротранспорт", CategoryId = categories[8].Id },
            new Childcategory { Name = "Тренажери", CategoryId = categories[8].Id },
            new Childcategory { Name = "Фітнес та йога", CategoryId = categories[8].Id },
            new Childcategory { Name = "Бокс та єдиноборства", CategoryId = categories[8].Id },
            new Childcategory { Name = "БАДи та спортивне", CategoryId = categories[8].Id },
            //Одяг, взуття та прикраси
            new Childcategory { Name = "Жінкам", CategoryId = categories[9].Id },
            new Childcategory { Name = "Чоловікам", CategoryId = categories[9].Id },
            new Childcategory { Name = "Дітям", CategoryId = categories[9].Id },
            new Childcategory { Name = "Premiumbrands", CategoryId = categories[9].Id },
            new Childcategory { Name = "Спорт", CategoryId = categories[9].Id },
            new Childcategory { Name = "Нові колекції", CategoryId = categories[9].Id },
            //Краса та здоров'я
            new Childcategory { Name = "Дерматокосметика", CategoryId = categories[10].Id },
            new Childcategory { Name = "Парфумерія", CategoryId = categories[10].Id },
            new Childcategory { Name = "Декоративна косметика", CategoryId = categories[10].Id },
            new Childcategory { Name = "Подарункові набори", CategoryId = categories[10].Id },
            new Childcategory { Name = "Догляд за обличчям", CategoryId = categories[10].Id },
            new Childcategory { Name = "Догляд за волоссям", CategoryId = categories[10].Id },
            //Дитячі товари
            new Childcategory { Name = "Іграшки", CategoryId = categories[11].Id },
            new Childcategory { Name = "Конструктори LEGO", CategoryId = categories[11].Id },
            new Childcategory { Name = "Розвиток і творчість", CategoryId = categories[11].Id },
            new Childcategory { Name = "Підгузки та сповивання", CategoryId = categories[11].Id },
            new Childcategory { Name = "Книжки", CategoryId = categories[11].Id },
            new Childcategory { Name = "Харчування та годування", CategoryId = categories[11].Id },
            //Зоотовари
            new Childcategory { Name = "Коти", CategoryId = categories[12].Id },
            new Childcategory { Name = "Собаки", CategoryId = categories[12].Id },
            new Childcategory { Name = "Птахи", CategoryId = categories[12].Id },
            new Childcategory { Name = "Гризуни", CategoryId = categories[12].Id },
            //Офіс, школа, книги
            new Childcategory { Name = "Шкільні рюкзаки, ранці, сумки та пенали", CategoryId = categories[13].Id },
            new Childcategory { Name = "Шкільне приладдя та творчість", CategoryId = categories[13].Id },
            new Childcategory { Name = "Все для навчання", CategoryId = categories[13].Id },
            new Childcategory { Name = "Товари для дитячої творчості", CategoryId = categories[13].Id },
            new Childcategory { Name = "Паперова продукція", CategoryId = categories[13].Id },
            new Childcategory { Name = "Офісна техніка та витратні матеріали", CategoryId = categories[13].Id },
            //Алкогольні напої та продукти
            new Childcategory { Name = "Міцні напої", CategoryId = categories[14].Id },
            new Childcategory { Name = "Вино", CategoryId = categories[14].Id },
            new Childcategory { Name = "Тютюнові вироби", CategoryId = categories[14].Id },
            new Childcategory { Name = "Безалкогольні напої", CategoryId = categories[14].Id },
            new Childcategory { Name = "Кава", CategoryId = categories[14].Id },
            new Childcategory { Name = "Спеції та приправи", CategoryId = categories[14].Id },
            new Childcategory { Name = "Рослинна олія", CategoryId = categories[14].Id },
            new Childcategory { Name = "Шоколад та цукерки", CategoryId = categories[14].Id },
            //Побутова хімія
            new Childcategory { Name = "Засоби для прання", CategoryId = categories[15].Id },
            new Childcategory { Name = "Засоби для миття посуду", CategoryId = categories[15].Id },
            new Childcategory { Name = "Засоби для догляду за будинком", CategoryId = categories[15].Id },
            new Childcategory { Name = "Засоби для догляду за ванною та туалетом", CategoryId = categories[15].Id },
            };
        }
    }
}
