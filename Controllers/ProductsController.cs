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
                    var product = await _context.Products.FindAsync(id);
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


        // GET: /Products/Searching
        public async Task<IActionResult> Searching(string inputSearching)
        {
            if (string.IsNullOrEmpty(inputSearching))  // Якщо inputSearching є порожнім або null, 
            {
                //  метод повертає порожню модель ProductSearchViewModel до стандартного представлення
                return View(new ProductSearchViewModel());
            }

            // Якщо inputSearching не порожній, метод використовує контекст бази даних _context для виконання пошуку продуктів.
            // Він здійснює запит до таблиці Products, щоб знайти продукти, у яких заголовок (Title) містить текст, який ввів користувач
            var products = await _context.Products
                .Include(p => p.Childcategory) 
                .Where(p => p.Title.Contains(inputSearching))
                .Select(p => new ProductSearchResult
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    Price = p.Price,
                    CategoryName = p.Childcategory != null ? p.Childcategory.Name : "No Category" 
                })
                .ToListAsync();
            // Результати пошуку (список продуктів) зберігаються у властивості Products об'єкта ProductSearchViewModel,
            // який створюється для передачі даних у представлення
            var viewModel = new ProductSearchViewModel
            {
                Products = products
            };
            // повертає представлення з назвою "Details", передаючи в нього модель viewModel, яка містить результати пошуку
            return View("Details", viewModel); //
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
            // Найти продукт по Id и загрузить связанные данные
            var product = await _context.Products
                                        .Include(p => p.ProductType)
                                        .Include(p => p.Brand)
                                        .Include(p => p.Childcategory)
                                        .Include(p => p.ProductImages)
                                        .Include(p => p.Reviews)
                                        .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            HttpContext.Session.SetString("Product", product.Title);
            return View(product);
        }
    }
}
