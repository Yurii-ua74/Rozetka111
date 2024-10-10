using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Rozetka.Data.Entity;
using Microsoft.AspNetCore.Mvc;
using Rozetka.Data;
using Microsoft.EntityFrameworkCore;
using Rozetka.Models.ViewModels.Products;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rozetka.Migrations;
using Rozetka.Models.ReviewModel;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


namespace Rozetka.Controllers
{
    // Контролер ProductsController відповідає за управління продуктами в базі даних магазину.
    // Він забезпечує CRUD (Create, Read, Update, Delete) операції для продуктів. 
    public class ProductsController : Controller
    {
       
        private readonly DataContext _context;


        // Ініціалізує контролер з контекстом бази даних ShopDbContext та
        // об'єктом IMapper для маппінгу між моделями та DTO (Data Transfer Objects).
        public ProductsController(DataContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.Include(p => p.Brand).Include(p => p.ProductType).Include(p => p.Childcategory).ToListAsync();
            return View(products);           
        }
        //Відображає список усіх продуктів.
        //Викликається, коли користувач переходить на сторінку списку продуктів.
        //Завантажує всі продукти з бази даних разом з пов'язаними брендами та категоріями і передає їх
        //у вигляд для відображення.


        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Childcategory)
                .Include(p => p.Brand)
                .Include(p => p.ProductType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        public async Task<IActionResult> Create()
        {
            var type = await _context.ProductTypes.ToListAsync();
            var brands = await _context.Brands.ToListAsync();
            var colors = await _context.ProductColors.ToListAsync();
            var childcategories = await _context.Childcategories.ToListAsync();
            var subchildcategories = await _context.SubChildCategories.ToListAsync();

            var viewModel = new CreateProductVM
            {
                ProductTypes = new SelectList(type, "Id", "Title"),
                Brands = new SelectList(brands, "Id", "Title"),
                ProductColors = new SelectList(colors, "Id", "Title"),
                Childcategories = new SelectList(childcategories, "Id", "Name"),
                SubChildCategories = new SelectList(subchildcategories, "Id", "Name")
                //Product = new Product()
            };
            return View(viewModel);
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProductVM viewModel)
        {

            if (ModelState.IsValid)
            {
                _context.Products.Add(viewModel.Product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var type = await _context.ProductTypes.ToListAsync();
            var brands = await _context.Brands.ToListAsync();
            var colors = await _context.ProductColors.ToListAsync();
            var childcategories = await _context.Childcategories.ToListAsync();
            var subchildcategories = await _context.SubChildCategories.ToListAsync();
            

            viewModel.ProductTypes = new SelectList(type, "Id", "Title");
            viewModel.Brands = new SelectList(brands, "Id", "Title");
            viewModel.ProductColors = new SelectList(colors, "Id", "Title");
            viewModel.Childcategories = new SelectList(childcategories, "Id", "Name");
            viewModel.SubChildCategories = new SelectList(subchildcategories, "Id", "Name");

            return View(viewModel);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var type = await _context.ProductTypes.ToListAsync();
            var brands = await _context.Brands.ToListAsync();
            var colors = await _context.ProductColors.ToListAsync();
            var childcategories = await _context.Childcategories.ToListAsync();
            var viewModel = new CreateProductVM
            {
                Product = product,
                ProductTypes = new SelectList(type, "Id", "Title"),
                Brands = new SelectList(brands, "Id", "Title"),
                ProductColors = new SelectList(colors, "Id", "Title"),
                Childcategories = new SelectList(childcategories, "Id", "Name")
            };
            //ViewData["ChildcategoryId"] = new SelectList(_context.Childcategories, "Id", "Id", product.ChildcategoryId);
            return View(viewModel);

            //return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CreateProductVM viewModel)
        {
            if (id != viewModel.Product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var product = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == viewModel.Product.Id);
                    if (product == null)
                    {
                        return NotFound();
                    }
                    viewModel.Product.Amount = product.Amount;
                    viewModel.Product.Rating = product.Rating;
                    _context.Update(viewModel.Product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(viewModel.Product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            var type = await _context.ProductTypes.ToListAsync();
            var brands = await _context.Brands.ToListAsync();
            var colors = await _context.ProductColors.ToListAsync();
            var childcategories = await _context.Childcategories.ToListAsync();

            viewModel.ProductTypes = new SelectList(type, "Id", "Title");
            viewModel.Brands = new SelectList(brands, "Id", "Title");
            viewModel.ProductColors = new SelectList(colors, "Id", "Title");
            viewModel.Childcategories = new SelectList(childcategories, "Id", "Name");

            return View(viewModel);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Childcategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        //// GET: /Products/Searching
        //public async Task<IActionResult> Searching(string inputSearching)
        //{
        //    if (string.IsNullOrEmpty(inputSearching))  // Якщо inputSearching є порожнім або null, 
        //    {
        //        //  метод повертає порожню модель ProductSearchViewModel до стандартного представлення
        //        return View(new ProductSearchViewModel());
        //    }
        //    if (inputSearching.Length < 2)
        //    {
        //        return View(new ProductSearchViewModel());
        //    }

        //    // Якщо inputSearching не порожній, метод використовує контекст бази даних _context для виконання пошуку продуктів.
        //    // Він здійснює запит до таблиці Products, щоб знайти продукти, у яких заголовок (Title) містить текст, який ввів користувач
        //    var products = await _context.Products
        //        .Include(p => p.Childcategory) 
        //        .Where(p => p.Title.Contains(inputSearching))
        //        .Select(p => new ProductSearchResult
        //        {
        //            Id = p.Id,
        //            Title = p.Title,
        //            Description = p.Description,
        //            Price = p.Price,
        //            CategoryName = p.Childcategory != null ? p.Childcategory.Name : "No Category" 
        //        })
        //        .ToListAsync();
        //    // Результати пошуку (список продуктів) зберігаються у властивості Products об'єкта ProductSearchViewModel,
        //    // який створюється для передачі даних у представлення
        //    var viewModel = new ProductSearchViewModel
        //    {
        //        Products = products
        //    };
        //    // повертає представлення з назвою "Details", передаючи в нього модель viewModel, яка містить результати пошуку
        //    return View("Details", viewModel); //
        //}


        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
        //Перевіряє, чи існує продукт з певним ідентифікатором.
        //Використовується в методі Edit для перевірки наявності продукту перед оновленням.
        //Перевіряє, чи є продукт з вказаним ідентифікатором в базі даних.


        public async Task<IActionResult> GetProduct(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            // Найти продукт по Id и загрузить связанные данные
            var product = await _context.Products
                                        .Include(p => p.ProductType)
                                        .Include(p => p.Brand)
                                        .Include(p => p.Childcategory)
                                        .Include(p => p.ProductImages)
                                        .Include(p => p.ProductColor)
                                        .Include(p => p.Reviews)
                                        .ThenInclude(r => r.User) // Загружаем связанные данные пользователя
                                        .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            HttpContext.Session.SetString("Product", product.Title);
            return View(product);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> NewReviewAsync(ReviewFormModel? formModel)
        {
            if (formModel == null)
            {
                return BadRequest("Неверные данные");
            }

            // Получаем текущего авторизованного пользователя
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Это ID пользователя в системе ASP.NET Identity

            if (userId == null)
            {
                return BadRequest("Не удалось определить пользователя");
            }

            // Найдем продукт, для которого оставляется отзыв
            var product = await _context.Products.FindAsync(formModel.IdProduct);
            if (product == null)
            {
                return NotFound("Продукт не найден");
            }

            // Создаем новый отзыв (оценка пользователя за данный продукт)
            var newReview = new Review
            {
                ProductId = formModel.IdProduct,
                UserId = userId,
                Rating = formModel.Rating, // Это индивидуальная оценка пользователя
                Comment = formModel.Comment,
                DateReview = DateTime.Now
            };

            // Добавляем отзыв в базу данных
            _context.Reviews.Add(newReview);

            // Увеличиваем количество отзывов у продукта
            product.Amount += 1;

            // Получаем все отзывы для данного продукта
            var allReviews = await _context.Reviews.Where(r => r.ProductId == product.Id).ToListAsync();

            // Рассчитываем новый средний рейтинг для продукта
            product.Rating = allReviews.Average(r => (decimal?)r.Rating) ?? 0;  // Средний рейтинг

            // Сохраняем изменения
            await _context.SaveChangesAsync();

            return RedirectToAction("GetProduct", new { id = formModel.IdProduct }); // Перенаправление на страницу продукта
        }


        // ///// пошук товарів з пошукової строки header ///// //
        [HttpGet]
        public IActionResult Search(string search)
        {
            if (string.IsNullOrWhiteSpace(search) || search.Length < 2)
            {
                // Якщо нічого не знайдено в усіх категоріях, виводимо повідомлення
                ViewBag.Message = "Для пошуку потрібно не меньше двох символів.";
                return View(new List<Product>()); // Повертаємо порожній список продуктів
                //return View("Index"); // Повертаємо на основну сторінку, якщо запит порожній
            }            

            // Спочатку шукаємо збіги в Childcategory
            var productsFromChildcategory = _context.Childcategories
                .Where(cc => cc.Name.Contains(search))
                .SelectMany(cc => cc.SubChildCategories) // Отримуємо підкатегорії
                .SelectMany(scc => scc.Products)    // Отримуємо продукти цих підкатегорій
                .Include(p => p.ProductType)
                .Include(p => p.Brand)
                .Include(p => p.Childcategory)
                .Include(p => p.ProductImages)
                .Include(p => p.ProductColor)
                .Include(p => p.Reviews)
                .ToList();

            if (productsFromChildcategory.Any())
            {
                // Якщо знайдено продукти через Childcategory, повертаємо їх
                return View(productsFromChildcategory);
            }


            // Якщо нічого не знайдено в Childcategory, шукаємо в SubChildCategory
            var productsFromSubChildCategory = _context.SubChildCategories
                .Where(scc => scc.Name.Contains(search))
                .SelectMany(scc => scc.Products)         // Отримуємо продукти цієї підкатегорії
                .Include(p => p.ProductType)
                .Include(p => p.Brand)
                .Include(p => p.Childcategory)
                .Include(p => p.ProductImages)
                .Include(p => p.ProductColor)
                .Include(p => p.Reviews)
                .ToList();

            if (productsFromSubChildCategory.Any())
            {
                // Якщо знайдено продукти через SubChildCategory, повертаємо їх
                return View(productsFromSubChildCategory);
            }



            // Якщо не знайдено в категоріях, шукаємо безпосередньо в Products
            var productsFromProducts = _context.Products
                .Include(p => p.ProductImages) // Включаємо зображення продуктів
                .Where(p => p.Title.Contains(search))
                .ToList();

            if (productsFromProducts.Any())
            {
                // Якщо знайдено продукти, повертаємо їх
                return View(productsFromProducts);
            }

            // Якщо нічого не знайдено в усіх категоріях, виводимо повідомлення
            ViewBag.Message = "Нічого не знайдено за вашим запитом.";
            return View(new List<Product>()); // Повертаємо порожній список продуктів
        }  


        // GET: /Product/GetSearchResult
        public async Task<IActionResult> GetSearchResult(string inputSearching)
        {
            if (string.IsNullOrEmpty(inputSearching))  // Если поисковый запрос пуст или null, 
            {
                // Возвращаем пустую модель ProductSearchViewModel
                return View(new ProductSearchViewModel());
            }

            if (inputSearching.Length < 2)
            {
                return View(new ProductSearchViewModel()); // Возвращаем пустую модель при коротком запросе
            }

            // Разбиваем запрос на слова
            string[] words = inputSearching.Split(' ');

            // Инициализируем модель поиска
            ProductSearchViewModel productViewModel = new ProductSearchViewModel();

            // Загружаем продукты из базы данных
            var query = _context.Products
                    .Include(p => p.ProductType)
                    .Include(p => p.Brand)
                    .Include(p => p.Childcategory)                    
                    .Include(p => p.ProductImages)
                    .Include(p => p.ProductColor)
                    .Include(p => p.Reviews)
                    .AsQueryable();

            foreach (var word in words)
            {
                if (word.Length > 1)
                {
                    query = query.Where(product =>
                       (product.Title != null && product.Title.ToLower().Contains(word.ToLower())) ||
                       (product.ProductType != null && product.ProductType.Title.ToLower().Contains(word.ToLower())) ||
                       (product.Brand != null && product.Brand.Title.ToLower().Contains(word.ToLower())) ||
                       (product.Childcategory != null && product.Childcategory.Name.ToLower().Contains(word.ToLower())));

                }
            }

            // Выполняем запрос
            productViewModel.SearchingResults = await query.ToListAsync();

            return View(productViewModel);
        }
    }
}

