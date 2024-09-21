﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rozetka.Data;
using Rozetka.Data.Entity;


namespace Rozetka.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly DataContext _context;

        public CategoriesController(DataContext context)
        {
            _context = context;
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            var dataContext = _context.Categories
                          .Include(c => c.ParentCategory);
            return View(await dataContext.ToListAsync());
        }

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .Include(c => c.ParentCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            ViewData["ParentCategoryId"] = new SelectList(_context.Categories, "Id", "Id");
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,ParentCategoryId")] Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ParentCategoryId"] = new SelectList(_context.Categories, "Id", "Id", category.ParentCategoryId);
            return View(category);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            ViewData["ParentCategoryId"] = new SelectList(_context.Categories, "Id", "Id", category.ParentCategoryId);
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ParentCategoryId")] Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
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
            ViewData["ParentCategoryId"] = new SelectList(_context.Categories, "Id", "Id", category.ParentCategoryId);
            return View(category);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .Include(c => c.ParentCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }

        public async Task<IActionResult> GetChildCategories(string category)
        {
            if (string.IsNullOrEmpty(category))
            {
                return NotFound();
            }

            // Знайти Id категорії за назвою
            var categoryEntity = _context.Categories
                .FirstOrDefault(c => c.Name == category);
            if (categoryEntity == null)
            {
                // Якщо категорія не знайдена
                return NotFound();
            }
            HttpContext.Session.SetString("Category", categoryEntity.Name);

            var childCategories = await _context.Childcategories
                .Where(predicate: sc => sc.Category.Name == category)
                .ToListAsync();
            return View(childCategories);
        }

        // отримання підкатегорій та підпідкатегорій для КАТАЛОГУ
        public IActionResult GetChildAndSubChildCategories(string category)
        {
            //var childSubChildCategories = _context.Childcategories
            //                              .Where(c => c.Category.Name == category)
            //                              .Select(c => new
            //                              {
            //                                  c.Id,
            //                                  c.Name,
            //                                  SubChildCategories = c.SubChildCategories.Select(sc => new { sc.Id, sc.Name }).ToList()
            //                              }).ToList();

            // Повернення часткового представлення
            //return PartialView("_ChildCategoriesPartial", childSubChildCategories);

            //try
            //{
            //    // Отримуємо головну категорію за її назвою
            //    var parentCategory = _context.Categories
            //        .Include(c => c.Childcategory)
            //            .ThenInclude(child => child.SubChildCategories) // Завантажуємо субкатегорії
            //        .FirstOrDefault(c => c.Name == category);

            //    // Перевіряємо, чи знайдено категорію
            //    if (parentCategory == null)
            //    {
            //        return NotFound();
            //    }

            //    // Повертаємо дочірні категорії у вигляді часткового представлення
            //    return PartialView("_ChildCategories", parentCategory.Childcategory);
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //    return StatusCode(500, "Internal server error");
            //}

            try
            {
                // Отримуємо головну категорію за її назвою
                var parentCategory = _context.Categories
                    .Include(c => c.Childcategory)
                        .ThenInclude(child => child.SubChildCategories)
                    .FirstOrDefault(c => c.Name == category);

                // Перевіряємо, чи знайдено категорію
                if (parentCategory == null)
                {
                    Console.WriteLine($"Категорію не знайдено: {category}");
                    return NotFound();
                }

                // Повертаємо дочірні категорії у вигляді часткового представлення
                return PartialView("_ChildCategoriesPartial", parentCategory.Childcategory);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

    }
}

