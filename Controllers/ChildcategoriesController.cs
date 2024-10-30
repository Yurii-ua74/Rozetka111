using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Rozetka.Data;
using Rozetka.Data.Entity;
using Rozetka.Extensions;
using Rozetka.Models.ViewModels.ProductAndSubChildCategory;
using Rozetka.Models.ViewModels.Products;
using System.Security.Claims;
using System.Text.Json;

namespace Rozetka.Controllers
{
    public class ChildcategoriesController : Controller
    {
        private readonly DataContext _context;

        public ChildcategoriesController(DataContext context)
        {
            _context = context;
        }

        //[Route("Сhildcategories/index")]  
        // GET: Childcategories
        public async Task<IActionResult> Index()
        {
            //var dataContext = _context.Childcategories.Include(c => c.Category);
            var dataContext = _context.Childcategories
                          .Include(c => c.Category)
                          .Where(c => c.Category != null);
            return View(await dataContext.ToListAsync());
        }


        // GET: Childcategories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var childcategory = await _context.Childcategories
                .Include(c => c.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (childcategory == null)
            {
                return NotFound();
            }

            return View(childcategory);
        }

        // GET: Childcategories/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id");
            return View();
        }

        // POST: Childcategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,CategoryId")] Childcategory childcategory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(childcategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id", childcategory.CategoryId);
            return View(childcategory);
        }

        // GET: Childcategories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var childcategory = await _context.Childcategories.FindAsync(id);
            if (childcategory == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id", childcategory.CategoryId);
            return View(childcategory);
        }

        // POST: Childcategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,CategoryId")] Childcategory childcategory)
        {
            if (id != childcategory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(childcategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChildcategoryExists(childcategory.Id))
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id", childcategory.CategoryId);
            return View(childcategory);
        }

        // GET: Childcategories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var childcategory = await _context.Childcategories
                .Include(c => c.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (childcategory == null)
            {
                return NotFound();
            }

            return View(childcategory);
        }



        // POST: Childcategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var childcategory = await _context.Childcategories.FindAsync(id);
            if (childcategory != null)
            {
                _context.Childcategories.Remove(childcategory);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChildcategoryExists(int id)
        {
            return _context.Childcategories.Any(e => e.Id == id);
        }



        /* //////////////////////////////// ///////////////////////////////// */
        /*    викликається по кліку на childcategory на сторінці    */
        public async Task<IActionResult> GetProducts(string childcategory)
        {
            GetProductsFilterViewModelcs viewModel = new GetProductsFilterViewModelcs();

            // Получение или сохранение childcategory
            if (string.IsNullOrEmpty(childcategory))
            {
                childcategory = HttpContext.Session.GetString("ChildCategory") ?? "";
                if (string.IsNullOrEmpty(childcategory))
                {
                    return NotFound();
                }
            }
            else
            {
                string? oldCategory = HttpContext.Session.GetString("ChildCategory");
                if (!string.IsNullOrEmpty(oldCategory)) {
                    if(oldCategory != childcategory)
                    {
                        // Удаляем сохраненные данные выбранных брендов из сессии
                        HttpContext.Session.Remove("SelectedTypes");
                        // Удаляем сохраненные данные выбранных брендов из сессии
                        HttpContext.Session.Remove("SelectedBrands");

                        HttpContext.Session.Remove("MinPrice");
                        HttpContext.Session.Remove("MaxPrice");
                    }
                }                
            }
            HttpContext.Session.SetString("ChildCategory", childcategory);

            // Загрузка подкатегории
            var childcategoryEntity = await _context.Childcategories.FirstOrDefaultAsync(c => c.Name == childcategory);
            if (childcategoryEntity == null)
            {
                return NotFound();
            }

            // Получение продуктов по подкатегории
            var productsQuery = _context.Products
                .Where(p => p.ChildcategoryId == childcategoryEntity.Id)
                .Include(p => p.ProductType)
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Include(p => p.ProductColor)
                .Include(p => p.Reviews)
                .AsQueryable();

            // Применение фильтра по типу продукта
            string? typesJson = HttpContext.Session.GetString("SelectedTypes");
            if (!string.IsNullOrEmpty(typesJson))
            {
                viewModel.IsFilterTypes = true;
                string[] selectedTypes = JsonSerializer.Deserialize<string[]>(typesJson);
                productsQuery = productsQuery.Where(p => selectedTypes!.Contains(p.ProductType!.Title));
            }

            // Применение фильтра по бренду
            string? brandsJson = HttpContext.Session.GetString("SelectedBrands");
            if (!string.IsNullOrEmpty(brandsJson))
            {
                viewModel.IsFilterBrands = true;
                string[] selectedBrands = JsonSerializer.Deserialize<string[]>(brandsJson);
                productsQuery = productsQuery.Where(p => selectedBrands!.Contains(p.Brand!.Title));
            }

            // Применение фильтра по цене
            int? minPrice = HttpContext.Session.GetInt32("MinPrice");
            int? maxPrice = HttpContext.Session.GetInt32("MaxPrice");
            if (minPrice.HasValue && maxPrice.HasValue)
            {
                viewModel.IsFilterPrace = true;
                productsQuery = productsQuery.Where(p => p.Price >= minPrice && p.Price <= maxPrice);
            }

            // Выполнение запроса и получение продуктов
            var products = await productsQuery.ToListAsync();

            // Применение активных акций
            var activeActions = await _context.Actions
                .Where(a => a.StartDate <= DateTime.Now && a.EndDate >= DateTime.Now)
                .ToListAsync();

            foreach (var product in products)
            {
                var action = activeActions.FirstOrDefault(a => a.ProductId == product.Id);
                if (action != null)
                {
                    product.ActionPrice = action.NewPrice;
                }
            }

            // Проверка избранных товаров
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != null)
            {
                var favoriteProductIds = await _context.Favorites
                    .Where(f => f.UserId == userId)
                    .Select(f => f.ProductId)
                    .ToListAsync();

                foreach (var product in products)
                {
                    product.IsInFavorites = favoriteProductIds.Contains(product.Id);
                }
            }

            // Подготовка фильтров и модели представления
            viewModel.Products = products;
            viewModel.Category = products.Select(p => p.Category).FirstOrDefault();
            viewModel.ChildCategory = childcategoryEntity;
            viewModel.FilterTypes = products.Where(p => p.ProductType != null).Select(p => p.ProductType!.Title).Distinct().ToList();
            viewModel.FilterBrands = products.Where(p => p.Brand != null).Select(p => p.Brand!.Title).Distinct().ToList();
            viewModel.StartPrace = products.Min(p => p.Price);
            viewModel.EndPrace = products.Max(p => p.Price);

            return View(viewModel);
        }
       
        [HttpPost]
        public IActionResult FilterByBuyer(string[] selectedbuyers)
        {


            return RedirectToAction(nameof(GetProducts));
        }

        [HttpPost]
        public IActionResult FilterByType(string[] selectedTypes)
        {
            // Сериализуем массив в JSON-строку
            string typesJson = JsonSerializer.Serialize(selectedTypes);

            // Сохраняем JSON-строку в сессии
            HttpContext.Session.SetString("SelectedTypes", typesJson);

            return RedirectToAction(nameof(GetProducts));
        }
        public IActionResult ClearSelectedTypes()
        {
            // Удаляем сохраненные данные выбранных брендов из сессии
            HttpContext.Session.Remove("SelectedTypes");

            return RedirectToAction(nameof(GetProducts));
        }
        [HttpPost]
        public IActionResult FilterByBrand(string[] selectedBrands)
        {

            // Сериализуем массив в JSON-строку
            string brandsJson = JsonSerializer.Serialize(selectedBrands);

            // Сохраняем JSON-строку в сессии
            HttpContext.Session.SetString("SelectedBrands", brandsJson);

            return RedirectToAction(nameof(GetProducts));
        }
        public IActionResult ClearSelectedBrands()
        {
            // Удаляем сохраненные данные выбранных брендов из сессии
            HttpContext.Session.Remove("SelectedBrands");
            
            return RedirectToAction(nameof(GetProducts));
        }

        [HttpPost]
        public IActionResult FilterByPrice(int minPrice, int maxPrice)
        {
            // Сериализуем значения minPrice и maxPrice и сохраняем их в сессии
            HttpContext.Session.SetInt32("MinPrice", minPrice);
            HttpContext.Session.SetInt32("MaxPrice", maxPrice);

            return RedirectToAction(nameof(GetProducts));
        }
        public IActionResult ClearPriceFilter()
        {
            HttpContext.Session.Remove("MinPrice");
            HttpContext.Session.Remove("MaxPrice");

            return RedirectToAction(nameof(GetProducts));
        }



        [HttpPost]
        public IActionResult GetProductsByFilter(List<int> subChildCategoryIds, decimal? minPrice, decimal? maxPrice)
        {           
            // Отримуємо Name ChildCategory з сессії
            var childcategory = HttpContext.Session.GetString("ChildCategory");
            var childcategoryId = HttpContext.Session.GetInt32("ChildCategoryId");

            // Отримуємо всі продукти відповідної підкатегорії
            var products = _context.Products
                .Where(p => p.SubChildCategory.ChildCategoryId == childcategoryId)
                .AsQueryable();

            // Перевіряємо, чи є обрані підкатегорії та чи задано діапазон цін
            bool hasSelectedSubCategories = subChildCategoryIds != null && subChildCategoryIds.Any();
            bool hasMinPrice = minPrice.HasValue;
            bool hasMaxPrice = maxPrice.HasValue;

            if (!hasSelectedSubCategories && (!hasMinPrice || !hasMaxPrice))
            {                                
                // Передаємо змінну у View, щоб визначити, чи були застосовані фільтри
                ViewBag.FiltersApplied = hasSelectedSubCategories || hasMinPrice || hasMaxPrice;
                return new EmptyResult();
            }
            else
            {
                // Фільтруємо продукти за субкатегоріями
                if (hasSelectedSubCategories)
                {
                    products = products.Where(p => subChildCategoryIds.Contains(p.SubChildCategoryId));
                }

                // Фільтруємо продукти за ціною
                if (hasMinPrice)
                {
                    products = products.Where(p => p.Price >= minPrice.Value);
                }

                if (hasMaxPrice)
                {
                    products = products.Where(p => p.Price <= maxPrice.Value);
                }
            }

            // Підключаємо необхідні пов'язані дані
            var filteredProducts = products
                .Include(p => p.ProductType)
                .Include(p => p.Brand)
                .Include(p => p.ProductImages)
                .Include(p => p.Reviews)
                .ToList();

            // Повертаємо часткове представлення з відфільтрованими продуктами
            return PartialView("_ProductsPartial", filteredProducts);
        }


    }


}
