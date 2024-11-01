using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Rozetka.Data;
using Rozetka.Data.Entity;
using Rozetka.Models.ViewModels.Products;

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
        public async Task<IActionResult> GetProducts(int subchildcategoryId)
        {
            if (subchildcategoryId <= 0)
            {
                TempData["ErrorMessage"] = "В цьому розділі ще немає товарів!";
                return RedirectToAction("AllCategoriesList", "SubChildCategory");
                
            }

            // Знайти підкатегорію за ID
            var subChildCategory = await _context.SubChildCategories
                .Include(scc => scc.Childcategory)
                    .ThenInclude(cc => cc.Category)
                .FirstOrDefaultAsync(scc => scc.Id == subchildcategoryId);

            if (subChildCategory == null)
            {
                TempData["ErrorMessage"] = "В цьому розділі ще немає товарів!";
                return RedirectToAction("AllCategoriesList", "SubChildCategory");
            }

            // Знайти товари за назвою SubChildCategory
            var products = await _context.Products
                .Include(p => p.SubChildCategory)
                .ThenInclude(scc => scc.Childcategory)
                .ThenInclude(cc => cc.Category)
                .Include(p => p.ProductType)       // Включаємо ProductType
                .Include(p => p.Brand)             // Включаємо Brand
                .Include(p => p.ProductImages)     // Включаємо ProductImages
                .Include(p => p.ProductColor)      // Включаємо ProductColor
                .Include(p => p.Reviews)           // Включаємо Reviews
                .Where(p => p.SubChildCategory.Id == subchildcategoryId)
                .ToListAsync(); // Викликаємо ToListAsync() для отримання списку

            if (products == null || !products.Any())
            {
                TempData["ErrorMessage"] = "В цьому розділі ще немає товарів!";
                return RedirectToAction("AllCategoriesList", "SubChildCategory");
            }

            // Створюємо модель, яка міститиме продукти та інформацію для Breadcrumbs навігації
            var viewModel = new ProductsViewModel
            {
                Products = products,
                Category = subChildCategory.Childcategory.Category.Name,
                ChildCategory = subChildCategory.Childcategory.Name,
                SubChildCategory = subChildCategory.Name
            };

            return View(viewModel);

        }


        /* ////////////////////////////////////////////////////// */


        public async Task<IActionResult> AllCategoriesList()
        {
            var categories = _context.Categories
               .Include(c => c.Childcategory)
               .ThenInclude(cc => cc.SubChildCategories)
               .ToList();

            return View(categories);            
        }
    }
}



