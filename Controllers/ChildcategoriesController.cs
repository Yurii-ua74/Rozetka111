using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rozetka.Data;
using Rozetka.Data.Entity;

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


        // Атрибут маршрутизації для методу, який приймає параметр
        // [Route("Сhildcategories/index{childcategory}")]
        // // метод Index з параметром
        //public IActionResult Index(string childcategory)
        //{
        //    //var dataContext = _context.Childcategories.Include(c => c.Category);
        //    //return View(await dataContext.ToListAsync());
        //    if (string.IsNullOrEmpty(childcategory))
        //    {
        //        return NotFound();
        //    }

        //    // Знайти Id категорії за назвою
        //    var childcategoryEntity = _context.Childcategories.FirstOrDefault(c => c.Name == childcategory);
        //    if (childcategoryEntity == null)
        //    {
        //        // Якщо підкатегорія не знайдена
        //        return NotFound();
        //    }

        //    // Отримати товари для знайденої підкатегорії 
        //    var products = _context.Products
        //        .Where(sc => sc.Childcategory.Name == childcategory)
        //        .ToList();

        //    // Відкрити на новій сторінці
        //    //ViewData["ChildCategoryName"] = childcategory;
        //    HttpContext.Session.SetString("ChildCategory", childcategory);
        //    return View(products);
        //}

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
        

        public async Task<IActionResult> GetSubChildCategories(string childcategory)
        {
            if (string.IsNullOrEmpty(childcategory))
            {
                return NotFound();
            }

            // Знайти Id категорії за назвою
            var childcategoryEntity = _context.Childcategories
                .FirstOrDefault(c => c.Name == childcategory);
            if (childcategoryEntity == null)
            {
                // Якщо категорія не знайдена
                return NotFound();
            }
            HttpContext.Session.SetString("ChildCategory", childcategoryEntity.Name);

            var subchildCategories = await _context.SubChildCategories
                .Where(predicate: sc => sc.Childcategory.Name == childcategory)
                .ToListAsync();
            return View(subchildCategories);
        }

    }
}
