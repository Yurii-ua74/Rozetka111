﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rozetka.Data;
using Rozetka.Data.Entity;
using Rozetka.Models.ViewModels.ProductAndSubChildCategory;

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
        

        //public async Task<IActionResult> GetSubChildCategories(string childcategory)
        //{
        //    if (string.IsNullOrEmpty(childcategory))
        //    {
        //        return NotFound();
        //    }

        //    // Знайти Id категорії за назвою
        //    var childcategoryEntity = _context.Childcategories
        //        .FirstOrDefault(c => c.Name == childcategory);
        //    if (childcategoryEntity == null)
        //    {
        //        // Якщо категорія не знайдена
        //        return NotFound();
        //    }
        //    HttpContext.Session.SetString("ChildCategory", childcategoryEntity.Name);

        //    var subchildCategories = await _context.SubChildCategories
        //        .Where(predicate: sc => sc.Childcategory.Name == childcategory)
        //        .ToListAsync();
        //    return View(subchildCategories);
        //}


        public async Task<IActionResult> GetProducts(string childcategory)
        {
            if (string.IsNullOrEmpty(childcategory))
            {
                return NotFound();
            }

            // Знайти підкатегорію за назвою
            var childcategoryEntity = await _context.Childcategories
                .FirstOrDefaultAsync(c => c.Name == childcategory);
            if (childcategoryEntity == null)
            {
                // Якщо підкатегорія не знайдена
                return NotFound();
            }
            HttpContext.Session.SetString("ChildCategory", childcategoryEntity.Name);

            // Знайти субпідкатегорії, пов'язані з підкатегорією
            var subchildCategories = await _context.SubChildCategories
                .Where(sc => sc.ChildCategoryId == childcategoryEntity.Id)
                .ToListAsync();
            if (subchildCategories == null || !subchildCategories.Any())
            {
                return NotFound();
            }

            // Об'єднати всі товари з субпідкатегорій в один список
            var products = new List<Product>();

            foreach (var subchildCategory in subchildCategories)
            {
                var subProducts = await _context.Products
                    .Where(p => p.SubChildCategoryId == subchildCategory.Id)
                    .Include(p => p.ProductType)
                    .Include(p => p.Brand)
                    .Include(p => p.Childcategory)
                    .Include(p => p.ProductImages)
                    .Include(p => p.Reviews)
                    .Take(6) // Обмеження до 6 товарів
                    .ToListAsync();

                products.AddRange(subProducts);
            }

            // Створити ViewModel та передати його у View
            var viewModel = new ProductAndSubChildCategoryViewModel
            {
                Products = products,
                SubChildCategories = subchildCategories
            };

            // Передати список продуктів у View
            return View(viewModel);
        }







        /* ////////////////////////////////////////////  */
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



        /* ////////////////////////////////////////////  */
        [HttpPost]
        public RedirectToActionResult AddFilter(string[]? selectedBrands, decimal? startPrice, decimal? endPrice)
        {
            if (selectedBrands.Any())
            {
                HttpContext.Session.SetString("SelectedBrands", string.Join(",", selectedBrands));
            }
            if (startPrice != null)
            {
                HttpContext.Session.SetString("StartPrice", startPrice.ToString());
            }
            if (endPrice != null)
            {
                HttpContext.Session.SetString("EndPrice", endPrice.ToString());
            }

            return RedirectToAction(nameof(GetProducts));
        }

        public RedirectToActionResult FullDeleteFilters()
        {
            HttpContext.Session.Remove("SelectedBrands");
            HttpContext.Session.Remove("StartPrice");
            HttpContext.Session.Remove("EndPrice");
            return RedirectToAction(nameof(GetProducts));
        }

        public RedirectToActionResult DeleteFilterBrand()
        {
            HttpContext.Session.Remove("SelectedBrands");
            return RedirectToAction(nameof(GetProducts));
        }
        public RedirectToActionResult DeleteFilterPrice()
        {
            HttpContext.Session.Remove("StartPrice");
            HttpContext.Session.Remove("EndPrice");
            return RedirectToAction(nameof(GetProducts));
        }

    }
}
