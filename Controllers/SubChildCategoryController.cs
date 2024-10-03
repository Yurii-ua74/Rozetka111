using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Rozetka.Data;
using Rozetka.Data.Entity;

namespace Rozetka.Controllers
{
    public class SubChildCategoryController : Controller
    {
        private readonly DataContext _context;
       
        public SubChildCategoryController(DataContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var dataContext = _context.SubChildCategories
                          .Include(c => c.Childcategory)
                          .Where(c => c.Childcategory != null);
            return View(await dataContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subchildcategory = await _context.SubChildCategories
                .Include(c => c.Childcategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subchildcategory == null)
            {
                return NotFound();
            }

            return View(subchildcategory);
        }

        // GET: Childcategories/Create
        public IActionResult Create()
        {
            ViewData["ChildCategoryId"] = new SelectList(_context.Childcategories, "Id", "Id");
            return View();
        }

        // POST: Childcategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,ChildCategoryId")] SubChildCategory subchildcategory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(subchildcategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id", subchildcategory.ChildCategoryId);
            return View(subchildcategory);
        }

        // GET: Childcategories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subchildcategory = await _context.SubChildCategories.FindAsync(id);
            if (subchildcategory == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Childcategories, "Id", "Id", subchildcategory.ChildCategoryId);
            return View(subchildcategory);
        }

        // POST: Childcategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ChildCategoryId")] SubChildCategory subchildcategory)
        {
            if (id != subchildcategory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(subchildcategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubChildCategoryExists(subchildcategory.Id))
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id", subchildcategory.ChildCategoryId);
            return View(subchildcategory);
        }

        // GET: SubChildcategories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subchildcategory = await _context.SubChildCategories
                .Include(c => c.Childcategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subchildcategory == null)
            {
                return NotFound();
            }

            return View(subchildcategory);
        }

        // POST: Childcategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subchildcategory = await _context.SubChildCategories.FindAsync(id);
            if (subchildcategory != null)
            {
                _context.SubChildCategories.Remove(subchildcategory);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SubChildCategoryExists(int id)
        {
            return _context.SubChildCategories.Any(e => e.Id == id);
        }


        /* ///////////////  викликаєтьсяя з каталогу від субпідкатегорій  /////////////// */
        public async Task<IActionResult> GetProducts(string subchildcategory)
        {
            if (string.IsNullOrEmpty(subchildcategory))
            {
                subchildcategory = HttpContext.Session.GetString("SubChildCategory");
                //return NotFound();
            }
            else
            {
                string oldSubChildCategory = HttpContext.Session.GetString("SubChildCategory");
                if (!string.IsNullOrEmpty(oldSubChildCategory))
                {
                    if (subchildcategory != oldSubChildCategory)
                    {
                        HttpContext.Session.Remove("selectedSubChildCategories"); //удаляем фильтры при смене категории
                        HttpContext.Session.Remove("minPrice");
                        HttpContext.Session.Remove("maxPrice");
                    }
                }
            }
            HttpContext.Session.SetString("SubChildCategory", subchildcategory);

            // Знайти Id категорії за назвою
            var subchildcategoryEntity = _context.SubChildCategories.FirstOrDefault(c => c.Name == subchildcategory);
            if (subchildcategoryEntity == null)
            {
                // Якщо підкатегорія не знайдена
                return NotFound();
            }

            // Отримати товари для знайденої підкатегорії з усіма відповідними даними
            var productsQuery = _context.Products
                .Where(p => p.SubChildCategory.Name == subchildcategory)
                .Include(p => p.ProductType)
                .Include(p => p.Brand)
                .Include(p => p.ProductImages)
                .Include(p => p.Reviews)
                .AsQueryable();

            // Извлечение фильтров из сессии
            var selectedSubChildCategories = HttpContext.Session.GetString("selectedSubChildCategories")?.Split(',') ?? Array.Empty<string>();
            var startPriceString = HttpContext.Session.GetString("minPrice");
            var endPriceString = HttpContext.Session.GetString("maxPrice");
            // Применение фильтров из сессии
            if (selectedSubChildCategories.Length > 0)
            {
                productsQuery = productsQuery.Where(p => selectedSubChildCategories.Contains(p.SubChildCategory.Name));
                //ViewBag.SelectedBrands = selectedBrands;
            }

            if (decimal.TryParse(startPriceString, out var startPrice))
            {
                productsQuery = productsQuery.Where(p => p.Price >= startPrice);
                //ViewBag.StartPrice = startPrice;
            }

            if (decimal.TryParse(endPriceString, out var endPrice))
            {
                productsQuery = productsQuery.Where(p => p.Price <= endPrice);
                //ViewBag.EndPrice = endPrice;
            }

            // Выполнение запроса и получение продуктов
            var products = await productsQuery.ToListAsync();

            return View(products);

        }


        /* ////////////////////////////////////////////////////// */



        /* ////////////////////////////////////////////////////// */




        [HttpPost]
        public IActionResult GetProductsBySubChildCategories(List<int> subchildcategoryIds)
        {
            // Отримуємо товари відповідно до вибраних субкатегорій
            var products = _context.Products
                .Where(p => subchildcategoryIds.Contains(p.SubChildCategoryId))
                .Include(p => p.ProductType)
                .Include(p => p.Brand)
                .Include(p => p.ProductImages)
                .Include(p => p.Reviews)
                .Take(6)  // Обмежуємо кількість товарів до 6 на субкатегорію
                .ToList();

            return PartialView("_ProductsPartial", products);
        }
        /* ///////// */


        //public IActionResult FilterByPrice(decimal? startPrice, decimal? endPrice)
        //{
        //    // Отримуємо всі товари
        //    var products = _context.Products
        //        .Include(p => p.ProductType)  // Включаємо тип продукту
        //        .Include(p => p.Brand)         // Включаємо бренд
        //        .Include(p => p.ProductImages)  // Включаємо зображення продукту
        //        .Include(p => p.Reviews)       // Включаємо відгуки
        //        .AsQueryable();

        //    // Якщо вказана мінімальна ціна, фільтруємо за нею
        //    if (startPrice.HasValue)
        //    {
        //        products = products.Where(p => p.Price >= startPrice.Value);
        //    }

        //    // Якщо вказана максимальна ціна, фільтруємо за нею
        //    if (endPrice.HasValue)
        //    {
        //        products = products.Where(p => p.Price <= endPrice.Value);
        //    }

        //    // Повертаємо часткове представлення з відфільтрованими товарами
        //    return PartialView("_ProductsPartial", products);
        //}


        //public IActionResult FilterProducts(decimal? startPrice, decimal? endPrice, List<int> subchildcategoryIds)
        //{
        //    if (!subchildcategoryIds.IsNullOrEmpty() && !startPrice.HasValue && !endPrice.HasValue)
        //    {
        //        // Отримуємо товари відповідно до вибраних субкатегорій
        //         var products = _context.Products
        //            .Where(p => subchildcategoryIds.Contains(p.SubChildCategoryId))
        //            .Include(p => p.ProductType)
        //            .Include(p => p.Brand)
        //            .Include(p => p.ProductImages)
        //            .Include(p => p.Reviews)
        //            .Take(6)  // Обмежуємо кількість товарів до 6 на субкатегорію
        //            .ToList();
        //        return PartialView("_ProductsPartial", products);
        //    }
        //    else if (startPrice.HasValue && endPrice.HasValue && subchildcategoryIds.IsNullOrEmpty())
        //    {
        //        // Отримуємо всі товари
        //        var products = _context.Products
        //            .Include(p => p.ProductType)  // Включаємо тип продукту
        //            .Include(p => p.Brand)         // Включаємо бренд
        //            .Include(p => p.ProductImages)  // Включаємо зображення продукту
        //            .Include(p => p.Reviews)       // Включаємо відгуки
        //            .AsQueryable();
        //        return PartialView("_ProductsPartial", products);
        //    }
        //    else
        //    {
        //        // Отримуємо товари відповідно до вибраних субкатегорій
        //        var products = _context.Products
        //           .Where(p => subchildcategoryIds.Contains(p.SubChildCategoryId))
        //           .Include(p => p.ProductType)
        //           .Include(p => p.Brand)
        //           .Include(p => p.ProductImages)
        //           .Include(p => p.Reviews)
        //           .Take(6)  // Обмежуємо кількість товарів до 6 на субкатегорію
        //           .ToList();
        //        return PartialView("_ProductsPartial", products);
        //    }


        //// Якщо startPrice вказана, додаємо фільтрацію за мінімальною ціною
        //if (startPrice.HasValue)
        //{
        //    productsQuery = (List<Product>)productsQuery.Where(p => p.Price >= startPrice.Value);
        //}

        //// Якщо endPrice вказана, додаємо фільтрацію за максимальною ціною
        //if (endPrice.HasValue)
        //{
        //    productsQuery = (List<Product>)productsQuery.Where(p => p.Price <= endPrice.Value);
        //}

        //// Виконання запиту та отримання списку товарів
        //var products = productsQuery.ToList();

        // Повертаємо результат
        // return PartialView("_ProductsPartial", products);

        // Можна додатково обмежити кількість товарів, якщо потрібно, наприклад, показати перші 6 товарів
        //products = products.Take(6);

        //// Повертаємо часткове представлення з відфільтрованими товарами
        //return PartialView("_ProductsPartial", products);
        //}




        /*  ///////////////////////////  */

        //[HttpPost]
        //public RedirectToActionResult AddFilter(string[]? selectedBrands, decimal? startPrice, decimal? endPrice)
        //{
        //    if (selectedBrands.Any())
        //    {
        //        HttpContext.Session.SetString("SelectedBrands", string.Join(",", selectedBrands));
        //    }
        //    if (startPrice != null)
        //    {
        //        HttpContext.Session.SetString("StartPrice", startPrice.ToString());
        //    }
        //    if (endPrice != null)
        //    {
        //        HttpContext.Session.SetString("EndPrice", endPrice.ToString());
        //    }

        //    return RedirectToAction(nameof(GetProducts));
        //}

        //public RedirectToActionResult FullDeleteFilters()
        //{
        //    HttpContext.Session.Remove("SelectedBrands");
        //    HttpContext.Session.Remove("StartPrice");
        //    HttpContext.Session.Remove("EndPrice");
        //    return RedirectToAction(nameof(GetProducts));
        //}

        //public RedirectToActionResult DeleteFilterBrand()
        //{
        //    HttpContext.Session.Remove("SelectedBrands");
        //    return RedirectToAction(nameof(GetProducts));
        //}
        //public RedirectToActionResult DeleteFilterPrice()
        //{
        //    HttpContext.Session.Remove("StartPrice");
        //    HttpContext.Session.Remove("EndPrice");
        //    return RedirectToAction(nameof(GetProducts));
        //}


        [HttpPost]
        public RedirectToActionResult AddFilter(string[]? selectedSubChildCategories, decimal? minPrice, decimal? maxPrice)
        {
            if (selectedSubChildCategories.Any())
            {
                HttpContext.Session.SetString("selectedSubChildCategories", string.Join(",", selectedSubChildCategories));
            }
            if (minPrice != null)
            {
                HttpContext.Session.SetString("minPrice", minPrice.ToString());
            }
            if (maxPrice != null)
            {
                HttpContext.Session.SetString("maxPrice", maxPrice.ToString());
            }

            return RedirectToAction(nameof(GetProducts));
        }

        public RedirectToActionResult FullDeleteFilters()
        {
            HttpContext.Session.Remove("selectedSubChildCategories");
            HttpContext.Session.Remove("minPrice");
            HttpContext.Session.Remove("maxPrice");
            return RedirectToAction(nameof(GetProducts));
        }

        public RedirectToActionResult DeleteFilterBrand()
        {
            HttpContext.Session.Remove("selectedSubChildCategories");
            return RedirectToAction(nameof(GetProducts));
        }
        public RedirectToActionResult DeleteFilterPrice()
        {
            HttpContext.Session.Remove("minPrice");
            HttpContext.Session.Remove("maxPrice");
            return RedirectToAction(nameof(GetProducts));
        }
    }
}



