﻿using Microsoft.CodeAnalysis.Elfie.Serialization;
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
using System.Linq;


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
            var categories = await _context.Categories.ToListAsync();
            var childcategories = await _context.Childcategories.ToListAsync();
            var subchildcategories = await _context.SubChildCategories.ToListAsync();

            var viewModel = new CreateProductVM
            {
                ProductTypes = new SelectList(type, "Id", "Title"),
                Brands = new SelectList(brands, "Id", "Title"),
                ProductColors = new SelectList(colors, "Id", "Title"),
                Categories = new SelectList(categories, "Id", "Name"),
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
            var categories = await _context.Categories.ToListAsync();
            var childcategories = await _context.Childcategories.ToListAsync();
            var subchildcategories = await _context.SubChildCategories.ToListAsync();
            

            viewModel.ProductTypes = new SelectList(type, "Id", "Title");
            viewModel.Brands = new SelectList(brands, "Id", "Title");
            viewModel.ProductColors = new SelectList(colors, "Id", "Title");
            viewModel.Categories = new SelectList(categories, "Id", "Name");
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
            var categories = await _context.Categories.ToListAsync();
            var childcategories = await _context.Childcategories.ToListAsync();
            var viewModel = new CreateProductVM
            {
                Product = product,
                ProductTypes = new SelectList(type, "Id", "Title"),
                Brands = new SelectList(brands, "Id", "Title"),
                ProductColors = new SelectList(colors, "Id", "Title"),
                Categories = new SelectList(categories, "Id", "Name"),
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
            var categories = await _context.Categories.ToListAsync();
            var childcategories = await _context.Childcategories.ToListAsync();

            viewModel.ProductTypes = new SelectList(type, "Id", "Title");
            viewModel.Brands = new SelectList(brands, "Id", "Title");
            viewModel.ProductColors = new SelectList(colors, "Id", "Title");
            viewModel.Categories = new SelectList(categories, "Id", "Name");
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

            GetProductViewModel model = new GetProductViewModel();

            // Загружаем основной продукт и связанные данные
            model.Product = await _context.Products
                .Include(p => p.ProductType)
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Childcategory)
                .Include(p => p.ProductImages)
                .Include(p => p.ProductColor)
                .Include(p => p.Reviews)
                    .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (model.Product == null)
            {
                return NotFound();
            }

            if (model.Product.SellerId == null)
            {
                model.SellerName = "ШвидкоBUY";
            }
            else {               
                // Получаем email пользователя по его Id
                string? userEmail = await _context.Users
                    .Where(u => u.Id == model.Product.SellerId)
                    .Select(u => u.Email)
                    .FirstOrDefaultAsync();
                model.SellerName = userEmail;
            }

            // Получаем ID пользователя и проверяем наличие в избранном
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != null)
            {
                //проверяем есть ли товар в избранном
                model.Product.IsInFavorites = await _context.Favorites
                    .AnyAsync(w => w.UserId == userId && w.ProductId == id);
            }
            HttpContext.Session.SetString("Product", model.Product.Title);

            // Выбираем рекламные товары той же категории
            model.AdvertisingProducts = await _context.Products
                .Where(c => c.ChildcategoryId == model.Product.ChildcategoryId)
                .Include(p => p.ProductType)
                .Include(p => p.Brand)
                .Include(p => p.ProductImages)
                .ToListAsync();

            // Извлекаем активные акции для продуктов в списке `AdvertisingProducts` и текущего продукта
            var productIds = model.AdvertisingProducts.Select(p => p.Id).Append(model.Product.Id).ToList();
            var activeActions = await _context.Actions
                .Where(a => productIds.Contains(a.ProductId) && a.StartDate <= DateTime.Now && a.EndDate >= DateTime.Now)
                .ToDictionaryAsync(a => a.ProductId, a => a.NewPrice);

            // Устанавливаем цену акции для основного продукта
            if (activeActions.TryGetValue(model.Product.Id, out var actionPrice))
            {
                model.Product.ActionPrice = actionPrice;
            }

            // Присваиваем цену акции рекламным продуктам, если есть активная акция
            foreach (var product in model.AdvertisingProducts)
            {
                if (activeActions.TryGetValue(product.Id, out actionPrice))
                {
                    product.ActionPrice = actionPrice;
                }
            }

            return View(model);
        }


        // метод додавання рейтингу та відгуків
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

            // перевірка, чи залишав цей користувач вже відгук для цього продукту
            var existingReview = await _context.Reviews
                .FirstOrDefaultAsync(r => r.ProductId == formModel.IdProduct && r.UserId == userId);

            if (existingReview != null)
            {
                // Якщо відгук вже є, оновлюємо його
                existingReview.Rating = formModel.Rating;
                existingReview.Comment = formModel.Comment;
                existingReview.DateReview = DateTime.Now;
            }
            else
            {
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
            }

            // Получаем все отзывы для данного продукта
            var allReviews = await _context.Reviews.Where(r => r.ProductId == product.Id).ToListAsync();

            // Рассчитываем новый средний рейтинг для продукта
            product.Rating = allReviews.Average(r => (decimal?)r.Rating) ?? 0;  // Средний рейтинг

            // Сохраняем изменения
            await _context.SaveChangesAsync();

            return RedirectToAction("GetProduct", new { id = formModel.IdProduct }); // Перенаправление на страницу продукта
        }


        // ///// пошук товарів з пошукової строки header ///// // 
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

            // Загружаем все активные акции
            var activeActions = await _context.Actions
                .Where(a => a.StartDate <= DateTime.Now && a.EndDate >= DateTime.Now) // Только активные акции
                .ToListAsync();

            // Присваиваем цену акции, если продукт имеет активную акцию
            foreach (var product in query)
            {
                var action = activeActions.FirstOrDefault(a => a.ProductId == product.Id);
                if (action != null)
                {
                    product.ActionPrice = action.NewPrice; // Устанавливаем цену акции
                }
            }
            // Выполняем запрос
            productViewModel.SearchingResults = await query.ToListAsync();

            return View(productViewModel);
        }      

    }
}

