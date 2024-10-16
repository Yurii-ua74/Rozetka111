using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rozetka.Data.Entity;
using Rozetka.Data;
using Rozetka.Models.ViewModels.Products;
using Microsoft.EntityFrameworkCore;
using Rozetka.Servises;
using Microsoft.IdentityModel.Tokens;
using Rozetka.Migrations;

namespace Rozetka.Controllers
{
    public class AdvertisementController : Controller
    {
        private readonly DataContext _context;       

        public AdvertisementController(DataContext context)
        {
            //_dataService = dataService;
            _context = context;
        }



        // GET: Advertisement
        public ActionResult Index()
        {
            return View();
        }

        // GET: Advertisement/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Advertisement/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Advertisement/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Advertisement/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Advertisement/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Advertisement/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Advertisement/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }


        // Get
        // /////  викликається лише для користувачів  з роллю Seller ///// //
        public IActionResult CreateAdvertisement()
        {

            var model = new ProductAdvertisementVM
            {         
                ProductColors = _context.ProductColors.ToList(),
                Categories = _context.Categories.ToList(),
                Childcategories = _context.Childcategories.ToList(),
                SubChildCategories = _context.SubChildCategories.ToList()
            };
            return View(model);
        }



        // ///// отримання списку ChildCategories в випадаючий список ///// //
        public IActionResult GetChildCategories(int categoryId)
        {
            var childCategories = _context.Childcategories
                .Where(c => c.CategoryId == categoryId)
                .Select(c => new { c.Id, c.Name })
                .ToList();

            return Json(childCategories);
        }

        // ///// отримання списку SubChildCategories в випадаючий список ///// //
        public IActionResult GetSubChildCategories(int childCategoryId)
        {
            var subChildCategories = _context.SubChildCategories
                .Where(s => s.ChildCategoryId == childCategoryId)
                .Select(s => new { s.Id, s.Name })
                .ToList();

            return Json(subChildCategories);
        }

        


        [HttpPost]
        public async Task<IActionResult> CreateAdvertisement(ProductAdvertisementVM model, List<IFormFile> ImageData)
        {
            if (ModelState.IsValid)
            {
                //  доступ до ProductTypes
                var productType = _context.ProductTypes
                                   .FirstOrDefault(pt => pt.Title == model.ProductType);

                //  доступ до Brands
                var productBrand = _context.Brands
                                   .FirstOrDefault(pt => pt.Title == model.Brand);

                if (productType != null)
                {
                    model.ProductTypeId = productType.Id; // Отримуємо ID типу продукту                   
                }
                else
                {
                    TempData["ErrorMessage"] = "Тип товару не знайдено.";
                    return RedirectToAction("CreateAdvertisement"); // Якщо тип не знайдено, перегружаємо сторінку
                }

                if (productBrand != null)
                {                   
                    model.BrandId = productBrand.Id; // Отримуємо ID бренду
                }
                else
                {
                    TempData["ErrorMessage"] = "Бренд товару не знайдено.";
                    return RedirectToAction("CreateAdvertisement"); // Якщо тип не знайдено, перегружаємо сторінку
                }


                // 1. Створюємо новий продукт
                var product = new Product
                {
                    Title = model.Title,
                    Description = model.Description,
                    Сharacteristic = model.Characteristic,
                    Price = model.Price,
                    CategoryId = model.CategoryId,
                    ChildcategoryId = model.ChildcategoryId,
                    SubChildCategoryId = model.SubChildCategoryId,
                    ProductTypeId = model.ProductTypeId,
                    BrandId = model.BrandId
                };

                // Додаємо продукт до бази даних
                _context.Products.Add(product);
                // Перевірка результату збереження продукту
                int saveResult = await _context.SaveChangesAsync();

                if (saveResult > 0) // Якщо зміни успішно збережено
                {
                    // Отримання ProductId щойно збереженого продукту
                    int productId = product.Id;                   

                    // 2. Додавання зображень до таблиці ProductImages
                    if (ImageData != null && ImageData.Count > 0)
                    {
                        foreach (var file in ImageData)
                        {
                            if (file.Length > 0)
                            {
                                // Конвертація файлу в масив байтів
                                using (var memoryStream = new MemoryStream())
                                {
                                    await file.CopyToAsync(memoryStream);
                                    var image = new ProductImage
                                    {
                                        ProductId = productId, // Зв'язуємо з продуктом
                                        ImageData = memoryStream.ToArray()
                                    };

                                    // Додавання зображення до бази даних
                                    _context.ProductImages.Add(image);
                                }
                            }
                        }

                        // Перевірка результату збереження зображень
                        saveResult = await _context.SaveChangesAsync();

                        if (saveResult > 0)
                        {
                            TempData["SuccessMessage"] = "Товар і зображення успішно додано!";
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Товар додано, але не вдалося додати зображення.";
                        }
                    }
                    else
                    {
                        TempData["SuccessMessage"] = "Товар успішно додано!";
                    }

                    // Повернення на сторінку успішного створення
                    return RedirectToAction("CreateAdvertisement"); 

                }
                else
                {
                    TempData["ErrorMessage"] = "Не вдалося зберегти товар.";
                }                
            }
            // Якщо модель некоректна, повертаємо форму з помилками
            return RedirectToAction("CreateAdvertisement");
        }
    }
}
