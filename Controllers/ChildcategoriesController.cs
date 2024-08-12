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
            var dataContext = _context.Childcategories.Include(c => c.Category);
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

        public async Task<IActionResult> GetProducts(string childcategory)
        {
            if (string.IsNullOrEmpty(childcategory))
            {
                childcategory = HttpContext.Session.GetString("ChildCategory");
                //return NotFound();
            }
            else
            {
                string oldChildcategory = HttpContext.Session.GetString("ChildCategory");
                if (!string.IsNullOrEmpty(oldChildcategory))
                {
                    if (childcategory != oldChildcategory)
                    {
                        HttpContext.Session.Remove("SelectedBrands"); //удаляем фильтры при смене категории
                        HttpContext.Session.Remove("StartPrice");
                        HttpContext.Session.Remove("EndPrice");
                    }
                }
            }
            HttpContext.Session.SetString("ChildCategory", childcategory);

            // Знайти Id категорії за назвою
            var childcategoryEntity = _context.Childcategories.FirstOrDefault(c => c.Name == childcategory);
            if (childcategoryEntity == null)
            {
                // Якщо підкатегорія не знайдена
                return NotFound();
            }

            //// Отримати товари для знайденої підкатегорії з усіма відповідними даними
            //var products = await _context.Products
            //    .Where(p => p.Childcategory.Name == childcategory)
            //    .Include(p => p.Brand)
            //    .Include(p => p.ProductImages)
            //    .Include(p => p.Reviews)
            //    .ToListAsync();

            // Отримати товари для знайденої підкатегорії з усіма відповідними даними
            var productsQuery = _context.Products
                .Where(p => p.Childcategory.Name == childcategory)
                .Include(p => p.ProductType)
                .Include(p => p.Brand)
                .Include(p => p.ProductImages)
                .Include(p => p.Reviews)
                .AsQueryable();

            // Извлечение фильтров из сессии
            var selectedBrands = HttpContext.Session.GetString("SelectedBrands")?.Split(',') ?? Array.Empty<string>();
            var startPriceString = HttpContext.Session.GetString("StartPrice");
            var endPriceString = HttpContext.Session.GetString("EndPrice");
            // Применение фильтров из сессии
            if (selectedBrands.Length > 0)
            {
                productsQuery = productsQuery.Where(p => selectedBrands.Contains(p.Brand.Title));
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
