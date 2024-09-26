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
            new Childcategory { Name = "Засоби для догляду за ванною та туалетом", CategoryId = categories[15].Id }
            };
            foreach (var childCategory in childCategories)
            {
                context.Childcategories.Add(childCategory);
            }
            context.SaveChanges();

            // Створюємо субкатегорії (SubChildCategories), що прив'язані до ChildCategories
            var subChildCategories = new SubChildCategory[]
            {
                // ////////////////  Ноутбуки та комп'ютери  ///////////////// //
            //Ноутбуки
            new SubChildCategory { Name = "Asus", ChildCategoryId = childCategories[0].Id }, 
            new SubChildCategory { Name = "Lenovo", ChildCategoryId = childCategories[0].Id },
            new SubChildCategory { Name = "Acer", ChildCategoryId = childCategories[0].Id }, 
            new SubChildCategory { Name = "HP", ChildCategoryId = childCategories[0].Id },
            new SubChildCategory { Name = "Dell", ChildCategoryId = childCategories[0].Id },
            //Комп'ютери, неттопи, моноблоки
            new SubChildCategory { Name = "Клавіатури та миші", ChildCategoryId = childCategories[1].Id },
            new SubChildCategory { Name = "Комп’ютерні колонки", ChildCategoryId = childCategories[1].Id },
            new SubChildCategory { Name = "Акумулятори для ДБЖ", ChildCategoryId = childCategories[1].Id },
            new SubChildCategory { Name = "Джерела безперебійного живлення", ChildCategoryId = childCategories[1].Id },
            new SubChildCategory { Name = "Програмне хабезпечення", ChildCategoryId = childCategories[1].Id },
            //Gaming
            new SubChildCategory { Name = "PlayStation", ChildCategoryId = childCategories[2].Id },
            new SubChildCategory { Name = "Sony PlayStation 5", ChildCategoryId = childCategories[2].Id },
            new SubChildCategory { Name = "Ігрові поверхні", ChildCategoryId = childCategories[2].Id },
            //Планшети
            new SubChildCategory { Name = "Apple iPad", ChildCategoryId = childCategories[3].Id },
            new SubChildCategory { Name = "Samsung", ChildCategoryId = childCategories[3].Id },
            new SubChildCategory { Name = "Lenovo", ChildCategoryId = childCategories[3].Id },
            new SubChildCategory { Name = "Xiaomi", ChildCategoryId = childCategories[3].Id },
            new SubChildCategory { Name = "Чохли для планшетів", ChildCategoryId = childCategories[3].Id },
            //Кабелі та перехідники 
            new SubChildCategory { Name = "Кабелі", ChildCategoryId = childCategories[4].Id },
            new SubChildCategory { Name = "Перехідники", ChildCategoryId = childCategories[4].Id },
            //Комплектуючі
            new SubChildCategory { Name = "Відеокарти", ChildCategoryId = childCategories[5].Id },
            new SubChildCategory { Name = "SSD", ChildCategoryId = childCategories[5].Id },
            new SubChildCategory { Name = "Процесори", ChildCategoryId = childCategories[5].Id },
            new SubChildCategory { Name = "Жорсткі диски", ChildCategoryId = childCategories[5].Id },
            new SubChildCategory { Name = "Оперативна пам’ять", ChildCategoryId = childCategories[5].Id },
            new SubChildCategory { Name = "Материнські плати", ChildCategoryId = childCategories[5].Id },
            new SubChildCategory { Name = "Блоки живлення", ChildCategoryId = childCategories[5].Id },
            new SubChildCategory { Name = "Корпуси", ChildCategoryId = childCategories[5].Id },
            new SubChildCategory { Name = "Системи охолодження", ChildCategoryId = childCategories[5].Id },
            //Аксесуари для ноутбуків та ПК
            new SubChildCategory { Name = "Флеш пам’ять", ChildCategoryId = childCategories[6].Id },
            new SubChildCategory { Name = "Мережеві фільтри", ChildCategoryId = childCategories[6].Id },
            new SubChildCategory { Name = "Сумки, рюкзаки та чохли", ChildCategoryId = childCategories[6].Id },
            new SubChildCategory { Name = "Підставки та столики для ноутбуків", ChildCategoryId = childCategories[6].Id },
            new SubChildCategory { Name = "Веб-камери", ChildCategoryId = childCategories[6].Id },
            new SubChildCategory { Name = "Навушники", ChildCategoryId = childCategories[6].Id },
            new SubChildCategory { Name = "Мікрофони", ChildCategoryId = childCategories[6].Id },
            //Офісна техніка
            new SubChildCategory { Name = "БФП/Прінтери", ChildCategoryId = childCategories[7].Id },
            new SubChildCategory { Name = "Витратні матеріали", ChildCategoryId = childCategories[7].Id },
            new SubChildCategory { Name = "Шредери", ChildCategoryId = childCategories[7].Id },
            new SubChildCategory { Name = "Телефони", ChildCategoryId = childCategories[7].Id },
            //Мережеве обладнання
            new SubChildCategory { Name = "Маршрутизатори", ChildCategoryId = childCategories[8].Id },
            new SubChildCategory { Name = "Комутатори", ChildCategoryId = childCategories[8].Id },
            new SubChildCategory { Name = "Мережеві адаптери", ChildCategoryId = childCategories[8].Id },
            new SubChildCategory { Name = "Ретранслятори Wi-Fi", ChildCategoryId = childCategories[8].Id },
            new SubChildCategory { Name = "Бездротові точки доступу", ChildCategoryId = childCategories[8].Id },
            new SubChildCategory { Name = "Мережеві сховища (NAS)", ChildCategoryId = childCategories[8].Id },
            new SubChildCategory { Name = "Патч-корди", ChildCategoryId = childCategories[8].Id },

                    // ////////////////  Смартфони, ТВ і електроніка  ///////////////// //
            //Мобільні телефони  
            new SubChildCategory { Name = "Apple", ChildCategoryId = childCategories[9].Id },
            new SubChildCategory { Name = "Samsung", ChildCategoryId = childCategories[9].Id },
            new SubChildCategory { Name = "Xiaomi", ChildCategoryId = childCategories[9].Id },
            new SubChildCategory { Name = "Motorola", ChildCategoryId = childCategories[9].Id },
            new SubChildCategory { Name = "Nokia", ChildCategoryId = childCategories[9].Id },
            new SubChildCategory { Name = "Смартфони", ChildCategoryId = childCategories[9].Id },
            new SubChildCategory { Name = "Кнопкові телефони", ChildCategoryId = childCategories[9].Id },
            //Павербанки та зарядні станції  
            new SubChildCategory { Name = "Універсальні мобільні батареї", ChildCategoryId = childCategories[10].Id },
            new SubChildCategory { Name = "Зарядні станції", ChildCategoryId = childCategories[10].Id },
            //Аксесуари для телефонів  
            new SubChildCategory { Name = "Навушники", ChildCategoryId = childCategories[11].Id },
            new SubChildCategory { Name = "Кабелі та адаптери", ChildCategoryId = childCategories[11].Id },
            new SubChildCategory { Name = "Карти пам’яті", ChildCategoryId = childCategories[11].Id },
            new SubChildCategory { Name = "Чохли для мобільних телефонів", ChildCategoryId = childCategories[11].Id },
            new SubChildCategory { Name = "Чохли-книжки", ChildCategoryId = childCategories[11].Id },
            new SubChildCategory { Name = "Бампери", ChildCategoryId = childCategories[11].Id },
            new SubChildCategory { Name = "Захисні плівки та скло", ChildCategoryId = childCategories[11].Id },
            //Телевізори
            new SubChildCategory { Name = "Телевізори", ChildCategoryId = childCategories[12].Id },
            new SubChildCategory { Name = "Samsung", ChildCategoryId = childCategories[12].Id },
            new SubChildCategory { Name = "LG", ChildCategoryId = childCategories[12].Id },
            new SubChildCategory { Name = "Xiaomi", ChildCategoryId = childCategories[12].Id },
            new SubChildCategory { Name = "Підставки кріплення для ТБ", ChildCategoryId = childCategories[12].Id },
            new SubChildCategory { Name = "Кабелі та адаптери", ChildCategoryId = childCategories[12].Id },
            new SubChildCategory { Name = "ТВ-антени та ресивери", ChildCategoryId = childCategories[12].Id },
            new SubChildCategory { Name = "Універсальні пульти ДК", ChildCategoryId = childCategories[12].Id },
            new SubChildCategory { Name = "ТБ запчастини та обладнання", ChildCategoryId = childCategories[12].Id },
            //Фото і відео  
            new SubChildCategory { Name = "Фотоапарати", ChildCategoryId = childCategories[13].Id },
            new SubChildCategory { Name = "Відеокамери", ChildCategoryId = childCategories[13].Id },
            new SubChildCategory { Name = "Екшн-камери", ChildCategoryId = childCategories[13].Id },
            new SubChildCategory { Name = "Об’єктиви", ChildCategoryId = childCategories[13].Id },
            new SubChildCategory { Name = "Аксесуари для фото/відео", ChildCategoryId = childCategories[13].Id },
            new SubChildCategory { Name = "Зарядні пристрої для фото та відеокамер", ChildCategoryId = childCategories[13].Id },
            new SubChildCategory { Name = "Акумулятори та батареї", ChildCategoryId = childCategories[13].Id },
            new SubChildCategory { Name = "Студійне обладнання", ChildCategoryId = childCategories[13].Id },
            //Портативна електроніка
            new SubChildCategory { Name = "Смарт-годинники", ChildCategoryId = childCategories[14].Id },
            new SubChildCategory { Name = "Apple Watch", ChildCategoryId = childCategories[14].Id },
            new SubChildCategory { Name = "Фітнес-браслети", ChildCategoryId = childCategories[14].Id },
            new SubChildCategory { Name = "Планшети", ChildCategoryId = childCategories[14].Id },
            new SubChildCategory { Name = "Аксессуари для планшетів", ChildCategoryId = childCategories[14].Id },
            new SubChildCategory { Name = "Електронні книги", ChildCategoryId = childCategories[14].Id },
            new SubChildCategory { Name = "Диктофони", ChildCategoryId = childCategories[14].Id },
            new SubChildCategory { Name = "Bluetooth-гарнітура", ChildCategoryId = childCategories[14].Id },
            new SubChildCategory { Name = "MP3-плеєри", ChildCategoryId = childCategories[14].Id },
            new SubChildCategory { Name = "3D та VR окуляри", ChildCategoryId = childCategories[14].Id },
            //Аудіотехніка
            new SubChildCategory { Name = "Акустичні системи", ChildCategoryId = childCategories[15].Id },
            new SubChildCategory { Name = "Портативні адаптери", ChildCategoryId = childCategories[15].Id },
            new SubChildCategory { Name = "Медіаплеєри", ChildCategoryId = childCategories[15].Id },
            new SubChildCategory { Name = "Домашні кінотеатри", ChildCategoryId = childCategories[15].Id },
            new SubChildCategory { Name = "Музичні центри та магнітоли", ChildCategoryId = childCategories[15].Id },
            new SubChildCategory { Name = "Комп’ютерні колонки", ChildCategoryId = childCategories[15].Id },
                      
                         // ////////////////  Товари для геймерів    ///////////////// //
            //Повербанки та зарядні станції 
            new SubChildCategory { Name = "Повербанки", ChildCategoryId = childCategories[16].Id },
            new SubChildCategory { Name = "Зарядні станції", ChildCategoryId = childCategories[16].Id },
            //Ігрові приставки 
            new SubChildCategory { Name = "Ігри для Nintendo", ChildCategoryId = childCategories[17].Id },
            //Ігрові ноутбуки  
            new SubChildCategory { Name = "Asus", ChildCategoryId = childCategories[18].Id },
            new SubChildCategory { Name = "HP", ChildCategoryId = childCategories[18].Id },
            new SubChildCategory { Name = "Lenovo", ChildCategoryId = childCategories[18].Id },
            new SubChildCategory { Name = "MSI", ChildCategoryId = childCategories[18].Id },
            new SubChildCategory { Name = "Dell", ChildCategoryId = childCategories[18].Id },
            new SubChildCategory { Name = "Ігри для ПК", ChildCategoryId = childCategories[18].Id },
            //PlayStation
            new SubChildCategory { Name = "Ігрові приставки PlayStation 5", ChildCategoryId = childCategories[19].Id },
            new SubChildCategory { Name = "Ігрові приставки PlayStation 4", ChildCategoryId = childCategories[19].Id },
            new SubChildCategory { Name = "Ігрові приставки PlayStation", ChildCategoryId = childCategories[19].Id },
            new SubChildCategory { Name = "Геймпади PlayStation", ChildCategoryId = childCategories[19].Id },
            new SubChildCategory { Name = "Шоломи  VR PlayStation", ChildCategoryId = childCategories[19].Id },
            new SubChildCategory { Name = "Гарнітури  PlayStation", ChildCategoryId = childCategories[19].Id },
            new SubChildCategory { Name = "Аксесуари PlayStation", ChildCategoryId = childCategories[19].Id },
            new SubChildCategory { Name = "Ігри для PlayStation 5", ChildCategoryId = childCategories[19].Id },
            new SubChildCategory { Name = "Ігри для  PlayStation 4", ChildCategoryId = childCategories[19].Id },
            //Ігрові комп'ютери 
            new SubChildCategory { Name = "ARTLINE", ChildCategoryId = childCategories[20].Id },
            new SubChildCategory { Name = "QUBE", ChildCategoryId = childCategories[20].Id },
            new SubChildCategory { Name = "Cobra", ChildCategoryId = childCategories[20].Id },
            //Комплектуючі для геймерів
            new SubChildCategory { Name = "Відеокарти", ChildCategoryId = childCategories[21].Id },
            new SubChildCategory { Name = "Процесори", ChildCategoryId = childCategories[21].Id },
            new SubChildCategory { Name = "Оперативна пам’ять", ChildCategoryId = childCategories[21].Id },
            new SubChildCategory { Name = "Материнські плати", ChildCategoryId = childCategories[21].Id },
            new SubChildCategory { Name = "Жорсткі диски", ChildCategoryId = childCategories[21].Id },
            new SubChildCategory { Name = "Блок живлення", ChildCategoryId = childCategories[21].Id },
            new SubChildCategory { Name = "Системи охолодження", ChildCategoryId = childCategories[21].Id },
            //Атрибутика та сувеніри
            new SubChildCategory { Name = "Браслети", ChildCategoryId = childCategories[22].Id },
            new SubChildCategory { Name = "Брелки", ChildCategoryId = childCategories[22].Id },
            new SubChildCategory { Name = "Гаманці", ChildCategoryId = childCategories[22].Id },
            new SubChildCategory { Name = "Подушки", ChildCategoryId = childCategories[22].Id },
            new SubChildCategory { Name = "Чашки", ChildCategoryId = childCategories[22].Id },
            new SubChildCategory { Name = "Фігурки та статуетки", ChildCategoryId = childCategories[22].Id },
            //Ігрова переферія
            new SubChildCategory { Name = "Навушники", ChildCategoryId = childCategories[23].Id },
            new SubChildCategory { Name = "Ігрові миші", ChildCategoryId = childCategories[23].Id },
            new SubChildCategory { Name = "Ігрові клавіатури", ChildCategoryId = childCategories[23].Id },
            new SubChildCategory { Name = "Ігрові килимки", ChildCategoryId = childCategories[23].Id },
            new SubChildCategory { Name = "Маніпулятори, джойстики", ChildCategoryId = childCategories[23].Id },
            new SubChildCategory { Name = "Геймерські крісла", ChildCategoryId = childCategories[23].Id },
            new SubChildCategory { Name = "Комп’ютерні столи", ChildCategoryId = childCategories[23].Id },
            new SubChildCategory { Name = "Геймерські рюкзаки", ChildCategoryId = childCategories[23].Id },
            new SubChildCategory { Name = "Bluetooth-гарнітури", ChildCategoryId = childCategories[23].Id },
            new SubChildCategory { Name = "МР3-плеєри", ChildCategoryId = childCategories[23].Id },
            new SubChildCategory { Name = "3D та VR окуляри", ChildCategoryId = childCategories[23].Id },  
            //Ігрові монітори  
            new SubChildCategory { Name = "Samsung", ChildCategoryId = childCategories[24].Id },
            new SubChildCategory { Name = "Acer", ChildCategoryId = childCategories[24].Id },
            new SubChildCategory { Name = "AOC", ChildCategoryId = childCategories[24].Id },
            new SubChildCategory { Name = "Asus", ChildCategoryId = childCategories[24].Id },
            new SubChildCategory { Name = "MSI", ChildCategoryId = childCategories[24].Id },
            new SubChildCategory { Name = "QUBE", ChildCategoryId = childCategories[24].Id },
                             // ////////////////  Побутова техніка   ///////////////// //
            //Велика побутова техніка
            new SubChildCategory { Name = "Холодильники", ChildCategoryId = childCategories[25].Id },
            new SubChildCategory { Name = "Морозильні камери", ChildCategoryId = childCategories[25].Id },
            new SubChildCategory { Name = "Пральні машини", ChildCategoryId = childCategories[25].Id },
            new SubChildCategory { Name = "Пральні машини із сушінням", ChildCategoryId = childCategories[25].Id },
            new SubChildCategory { Name = "Посудомийні машини", ChildCategoryId = childCategories[25].Id },
            new SubChildCategory { Name = "Мікрохвильові печі", ChildCategoryId = childCategories[25].Id },
            new SubChildCategory { Name = "Сушильні автомати", ChildCategoryId = childCategories[25].Id },
            new SubChildCategory { Name = "Плити", ChildCategoryId = childCategories[25].Id },
            new SubChildCategory { Name = "Вбудовані духові шафи", ChildCategoryId = childCategories[25].Id },
            new SubChildCategory { Name = "Вбудовані варильні поверхні", ChildCategoryId = childCategories[25].Id },
            new SubChildCategory { Name = "Комплекти вбудованої техніки", ChildCategoryId = childCategories[25].Id },
            new SubChildCategory { Name = "Кухонні витяжки", ChildCategoryId = childCategories[25].Id },
            new SubChildCategory { Name = "Сертифікати та продовження гарантії", ChildCategoryId = childCategories[25].Id },
            new SubChildCategory { Name = "Установка великої побутової техніки", ChildCategoryId = childCategories[25].Id },
            //Догляд та прибирання
            new SubChildCategory { Name = "Акумуляторні пилососи", ChildCategoryId = childCategories[26].Id },
            new SubChildCategory { Name = "Роботи-пилососи", ChildCategoryId = childCategories[26].Id },
            new SubChildCategory { Name = "Пилососи для сухого збирання", ChildCategoryId = childCategories[26].Id },
            new SubChildCategory { Name = "Праски з парогенератором", ChildCategoryId = childCategories[26].Id },
            //Кліматична техніка
            new SubChildCategory { Name = "Настінні кондиціонери", ChildCategoryId = childCategories[27].Id },
            new SubChildCategory { Name = "Мобільні кондиціонери", ChildCategoryId = childCategories[27].Id },
            new SubChildCategory { Name = "Вентилятори", ChildCategoryId = childCategories[27].Id },
            new SubChildCategory { Name = "Бойлери", ChildCategoryId = childCategories[27].Id },
            new SubChildCategory { Name = "Очищувачі повітря", ChildCategoryId = childCategories[27].Id },
            new SubChildCategory { Name = "Зволожувачі повітря", ChildCategoryId = childCategories[27].Id },
            new SubChildCategory { Name = "Осушувачі повітря", ChildCategoryId = childCategories[27].Id },
            new SubChildCategory { Name = "Обігрівачі", ChildCategoryId = childCategories[27].Id },
            new SubChildCategory { Name = "Монтаж кондиціонера", ChildCategoryId = childCategories[27].Id },
            //Краса і здоров’я  
            new SubChildCategory { Name = "Фени", ChildCategoryId = childCategories[28].Id },
            new SubChildCategory { Name = "Тримери", ChildCategoryId = childCategories[28].Id },
            new SubChildCategory { Name = "Електробритви", ChildCategoryId = childCategories[28].Id },
            new SubChildCategory { Name = "Прилади для укладання волосся", ChildCategoryId = childCategories[28].Id },
            new SubChildCategory { Name = "Машинки для стрижки", ChildCategoryId = childCategories[28].Id },
            new SubChildCategory { Name = "Фотоепілятори", ChildCategoryId = childCategories[28].Id },
            new SubChildCategory { Name = "Косметичні дзеркала", ChildCategoryId = childCategories[28].Id },
            //Кухня
            new SubChildCategory { Name = "Кавомашини", ChildCategoryId = childCategories[29].Id },
            new SubChildCategory { Name = "Мультипечі та аерогрилі", ChildCategoryId = childCategories[29].Id },
            new SubChildCategory { Name = "Блендери", ChildCategoryId = childCategories[29].Id },
            new SubChildCategory { Name = "Грилі та електрошашличниці", ChildCategoryId = childCategories[29].Id },
            new SubChildCategory { Name = "Кохонні комбайни", ChildCategoryId = childCategories[29].Id },
            new SubChildCategory { Name = "Мультиварки", ChildCategoryId = childCategories[29].Id },
            new SubChildCategory { Name = "Соковитискачі", ChildCategoryId = childCategories[29].Id },
            new SubChildCategory { Name = "Електрочайники", ChildCategoryId = childCategories[29].Id },
            new SubChildCategory { Name = "Електричні печі", ChildCategoryId = childCategories[29].Id },
            new SubChildCategory { Name = "М’ясорубки", ChildCategoryId = childCategories[29].Id },
            new SubChildCategory { Name = "Тостери", ChildCategoryId = childCategories[29].Id },
            new SubChildCategory { Name = "Хлібопічки", ChildCategoryId = childCategories[29].Id },
            new SubChildCategory { Name = "Міксери", ChildCategoryId = childCategories[29].Id },
            new SubChildCategory { Name = "Фільтри-глечики", ChildCategoryId = childCategories[29].Id },
            new SubChildCategory { Name = "Картріджі для фільтрів", ChildCategoryId = childCategories[29].Id },
            new SubChildCategory { Name = "Йогуртниці", ChildCategoryId = childCategories[29].Id },
            new SubChildCategory { Name = "Маринатори", ChildCategoryId = childCategories[29].Id },
            new SubChildCategory { Name = "Фритюрниці", ChildCategoryId = childCategories[29].Id },
                        // ////////////////  Товари для дому   ///////////////// //
            //Домашній текстиль
            new SubChildCategory { Name = "Матраци", ChildCategoryId = childCategories[30].Id },
            new SubChildCategory { Name = "Подушки", ChildCategoryId = childCategories[30].Id },
            new SubChildCategory { Name = "Ковдри", ChildCategoryId = childCategories[30].Id },
            new SubChildCategory { Name = "Рушники", ChildCategoryId = childCategories[30].Id },
            //Посуд
            new SubChildCategory { Name = "Зберігання продуктів", ChildCategoryId = childCategories[31].Id },
            new SubChildCategory { Name = "Кухонне приладдя", ChildCategoryId = childCategories[31].Id },
            new SubChildCategory { Name = "Посуд для дітей", ChildCategoryId = childCategories[31].Id },
            new SubChildCategory { Name = "Сковороди", ChildCategoryId = childCategories[31].Id },
            new SubChildCategory { Name = "Банки та ємності", ChildCategoryId = childCategories[31].Id },
            //Побутова хімія
            new SubChildCategory { Name = "Засоби для прибирання", ChildCategoryId = childCategories[32].Id },
            new SubChildCategory { Name = "Засоби для прання", ChildCategoryId = childCategories[32].Id },
            new SubChildCategory { Name = "Засоби для миття посуду", ChildCategoryId = childCategories[32].Id },
            new SubChildCategory { Name = "Спеціалізована хімія", ChildCategoryId = childCategories[32].Id },
            new SubChildCategory { Name = "Кондиціонери", ChildCategoryId = childCategories[32].Id },
            new SubChildCategory { Name = "Барвники", ChildCategoryId = childCategories[32].Id },
            //Меблі
            new SubChildCategory { Name = "Крісла", ChildCategoryId = childCategories[33].Id },
            new SubChildCategory { Name = "Стільці", ChildCategoryId = childCategories[33].Id },
            new SubChildCategory { Name = "Безкаркасні меблі", ChildCategoryId = childCategories[33].Id },
            new SubChildCategory { Name = "Дивани", ChildCategoryId = childCategories[33].Id },
            new SubChildCategory { Name = "Корпусні меблі", ChildCategoryId = childCategories[33].Id },
            //Колекціонування 
            new SubChildCategory { Name = "Банкноти та цінні папери", ChildCategoryId = childCategories[34].Id },
            new SubChildCategory { Name = "Зберігання та аксесуари", ChildCategoryId = childCategories[34].Id },
            new SubChildCategory { Name = "Монети", ChildCategoryId = childCategories[34].Id },
            //Каміни, печі, сауни
            new SubChildCategory { Name = "Біокаміни", ChildCategoryId = childCategories[35].Id },
            new SubChildCategory { Name = "Електрокаміни", ChildCategoryId = childCategories[35].Id },
            new SubChildCategory { Name = "Димарі", ChildCategoryId = childCategories[35].Id },
            new SubChildCategory { Name = "Печі, буржуйки, булер'яни", ChildCategoryId = childCategories[35].Id },
                        // ////////////////  Інструменти та автотовари   ///////////////// //
            //Інструменти 
            new SubChildCategory { Name = "Шуруповерти та електровикрутки", ChildCategoryId = childCategories[36].Id },
            new SubChildCategory { Name = "Болгарки", ChildCategoryId = childCategories[36].Id },
            new SubChildCategory { Name = "Перфоратори", ChildCategoryId = childCategories[36].Id },
            new SubChildCategory { Name = "Дрилі та міксери", ChildCategoryId = childCategories[36].Id },
            new SubChildCategory { Name = "Пилки та пліткорізи", ChildCategoryId = childCategories[36].Id },
            new SubChildCategory { Name = "Краскопульти", ChildCategoryId = childCategories[36].Id },
            new SubChildCategory { Name = "Електролобзики", ChildCategoryId = childCategories[36].Id },
            new SubChildCategory { Name = "Вимірювальна техніка", ChildCategoryId = childCategories[36].Id },
            new SubChildCategory { Name = "Фрезери", ChildCategoryId = childCategories[36].Id },
            new SubChildCategory { Name = "Багатофункціональні інструменти", ChildCategoryId = childCategories[36].Id },
            new SubChildCategory { Name = "Клейові пістолети та аксесуари", ChildCategoryId = childCategories[36].Id },
            new SubChildCategory { Name = "Електрорубанки", ChildCategoryId = childCategories[36].Id },
            new SubChildCategory { Name = "Будівельні фени", ChildCategoryId = childCategories[36].Id },
            //Витратні матеріали та приладдя
            new SubChildCategory { Name = "Свердла", ChildCategoryId = childCategories[37].Id },
            new SubChildCategory { Name = "Диски", ChildCategoryId = childCategories[37].Id },
            new SubChildCategory { Name = "Біти", ChildCategoryId = childCategories[37].Id },
            new SubChildCategory { Name = "Пильні полотна", ChildCategoryId = childCategories[37].Id },
            //Устаткування
            new SubChildCategory { Name = "Зварювальне обладнання", ChildCategoryId = childCategories[38].Id },
            new SubChildCategory { Name = "Генератори", ChildCategoryId = childCategories[38].Id },
            new SubChildCategory { Name = "Стабілізатори напруги", ChildCategoryId = childCategories[38].Id },
            new SubChildCategory { Name = "Бетонозмішувачі", ChildCategoryId = childCategories[38].Id },
            new SubChildCategory { Name = "Компресори", ChildCategoryId = childCategories[38].Id },
            new SubChildCategory { Name = "Теплові гармати", ChildCategoryId = childCategories[38].Id },
            new SubChildCategory { Name = "Точильні верстати", ChildCategoryId = childCategories[38].Id },
            new SubChildCategory { Name = "Вантажопідйомне обладгнання", ChildCategoryId = childCategories[38].Id },
            new SubChildCategory { Name = "Інвертори", ChildCategoryId = childCategories[38].Id },
            new SubChildCategory { Name = "Промислові пилососи", ChildCategoryId = childCategories[38].Id },
            //Ручний інструмент
            new SubChildCategory { Name = "Набори інструментів", ChildCategoryId = childCategories[39].Id },
            new SubChildCategory { Name = "Ящики та сумки для інструментів", ChildCategoryId = childCategories[39].Id },
            new SubChildCategory { Name = "Ключі, знімники", ChildCategoryId = childCategories[39].Id },
            new SubChildCategory { Name = "Викрутки", ChildCategoryId = childCategories[39].Id },
            new SubChildCategory { Name = "Вимірювально-розмічальний інсрумент", ChildCategoryId = childCategories[39].Id },
            new SubChildCategory { Name = "Шарнірно-губцевий інструмент", ChildCategoryId = childCategories[39].Id },
            new SubChildCategory { Name = "Заклепочники", ChildCategoryId = childCategories[39].Id },
            //Автотовари
            new SubChildCategory { Name = "Автозапчастини", ChildCategoryId = childCategories[40].Id },
            new SubChildCategory { Name = "Шини", ChildCategoryId = childCategories[40].Id },
            new SubChildCategory { Name = "Автомобільні диски", ChildCategoryId = childCategories[40].Id },
            new SubChildCategory { Name = "Відеореєстратори", ChildCategoryId = childCategories[40].Id },
            new SubChildCategory { Name = "Мастила", ChildCategoryId = childCategories[40].Id },
            new SubChildCategory { Name = "GPS навігатори", ChildCategoryId = childCategories[40].Id },
            new SubChildCategory { Name = "Автокомпресори", ChildCategoryId = childCategories[40].Id },
            new SubChildCategory { Name = "Головні пристрої", ChildCategoryId = childCategories[40].Id },
            new SubChildCategory { Name = "Автоприладдя", ChildCategoryId = childCategories[40].Id },
            new SubChildCategory { Name = "Автосигналізації", ChildCategoryId = childCategories[40].Id },
            new SubChildCategory { Name = "Техдопомога", ChildCategoryId = childCategories[40].Id },
            new SubChildCategory { Name = "Домкрати", ChildCategoryId = childCategories[40].Id },
            new SubChildCategory { Name = "Автоакустика", ChildCategoryId = childCategories[40].Id },
            new SubChildCategory { Name = "Паркувальні системи", ChildCategoryId = childCategories[40].Id },
            new SubChildCategory { Name = "Автохімія", ChildCategoryId = childCategories[40].Id },
            new SubChildCategory { Name = "Автокосметика", ChildCategoryId = childCategories[40].Id },
                       // ////////////////  Сантехніка та ремонт   ///////////////// //
            //Сантехніка та меблі для ванної

            //Будівельні матеріали

            //Інтер'єр та оздоблення

            //Освітлення

            };

            foreach (var subChildCategory in subChildCategories)
            {
                context.SubChildCategories.Add(subChildCategory);
            }

            context.SaveChanges();
        }
    }
}
