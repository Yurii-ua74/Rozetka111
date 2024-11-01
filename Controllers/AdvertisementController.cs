using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rozetka.Data.Entity;
using Rozetka.Data;
using Rozetka.Models.ViewModels.Products;
using Microsoft.EntityFrameworkCore;
using Rozetka.Servises;
using Microsoft.IdentityModel.Tokens;
using Rozetka.Migrations;
using Microsoft.AspNetCore.Identity;
using System;

namespace Rozetka.Controllers
{
    public class AdvertisementController : Controller
    {
        private readonly DataContext _context;
        // ін'єкція UserManager в контролер
        private readonly UserManager<User> _userManager;

        public AdvertisementController(UserManager<User> userManager, DataContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: Advertisement
        // ///// шукає та передає список оголошень за Id еористувача ///// //
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Отримуємо поточного користувача
            var user = await _userManager.GetUserAsync(User);

            // Перевіряємо, чи є користувач
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var advertisements = _context.MyAdvertisements
                 .Where(a => a.UserId == user.Id)
                 .Include(a => a.Product)
                 .ThenInclude(p => p.ProductColor)
                 .Include(a => a.Product.ProductType)
                 .Include(a => a.Product.Brand)
                 .Include(a => a.Product.ProductImages)
                 .Select(a => a.Product) // Вибираємо тільки продукти
                 .Distinct()
                 .ToList();



            // Перевіряємо, чи є оголошення
            if (advertisements == null || !advertisements.Any())
            {
                ViewBag.Message = "У Вас немає поданих оголошень";
                return View(new List<Product>()); // Передаємо пустий список
            }

            return View(advertisements); // Передаємо список товарів
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
        public ActionResult Delete()
        {           
           return View();
        }

        // POST: Advertisement/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                var advertisement = _context.MyAdvertisements.FirstOrDefault(a => a.ProductId == id);
                if (advertisement != null)
                {
                    var productId = advertisement.ProductId;
                    var userId = advertisement.UserId;

                    // Видалення оголошення
                    _context.MyAdvertisements.Remove(advertisement);

                    // Знайти і видалити продукт
                    var product = _context.Products.Find(productId);

                    if (product != null)
                    {
                        var images = _context.ProductImages
                            .Where(image => image.ProductId == productId)
                            .ToList();

                        if (images.Count > 0)
                        {                           
                            _context.ProductImages.RemoveRange(images);                            
                        }
                        _context.Products.Remove(product);
                    }


                    int changes = _context.SaveChanges(); // Зберегти зміни в базі даних
                    if (changes > 0)
                    {
                        TempData["SuccessMessage"] = "Товар і зображення успішно видалено!";
                        return RedirectToAction("Index"); // Повернути успішний статус
                    }
                }

                TempData["ErrorMessage"] = "Такого товару не знайдено.";
                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Index");
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

        

        // ///// Створення оголошення ///// //
        [HttpPost]
        public async Task<IActionResult> CreateAdvertisement(ProductAdvertisementVM model)
        {
            // Отримуємо поточного користувача
            var user = await _userManager.GetUserAsync(User);

            if (ModelState.IsValid)
            {
                var random = new Random(); // генератор випадкових чисел

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
                    Characteristic = model.Characteristic,
                    Price = model.Price,
                    CategoryId = model.CategoryId,
                    ChildcategoryId = model.ChildcategoryId,
                    SubChildCategoryId = model.SubChildCategoryId,
                    ProductTypeId = model.ProductTypeId,
                    ProductColorId = model.ProductColorId,
                    BrandId = model.BrandId,
                    QuantityInStock = random.Next(1, 20),
                    SellerId = user!.Id
                };

                // Додаємо продукт до бази даних
                _context.Products.Add(product);
                // Перевірка результату збереження продукту
                int saveResult = await _context.SaveChangesAsync();
                int productId;
                if (saveResult > 0) // Якщо зміни успішно збережено
                {
                    // Отримання ProductId щойно збереженого продукту
                    productId = product.Id;
                    try
                    {
                        // 2. Додавання зображень до таблиці ProductImages
                        if (model.ImageData != null && model.ImageData.Count > 0)
                        {
                            foreach (var file in model.ImageData)
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
                                // Отримання поточного користувача
                                var currentUser = await _userManager.GetUserAsync(User);

                                if (currentUser == null)
                                {
                                    TempData["ErrorMessage"] = "Не вдалося знайти користувача.";
                                    return RedirectToAction("CreateAdvertisement");
                                }

                                // Створення зв'язку - оголошення + юзер
                                var advertis = new MyAdvertisement
                                {
                                    ProductId = productId,
                                    DatePosted = DateTime.Now,
                                    UserId = currentUser.Id
                                };

                                // Додаємо зв'язку до бази даних
                                _context.MyAdvertisements.Add(advertis);
                                await _context.SaveChangesAsync();

                                TempData["SuccessMessage"] = "Товар і зображення успішно додано!";
                            }
                            else
                            {
                                TempData["ErrorMessage"] = "Товар додано, але не вдалося додати зображення.";
                            }
                        }
                        else
                        {
                            TempData["SuccessMessage"] = "Товар додано! Рекомендуємо додати фотографії";
                        }
                    }
                    catch (Exception ex) { }

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
