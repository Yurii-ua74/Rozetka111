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
        private readonly IDataService _dataService; // Сервіс, що інкапсулює логіку роботи з даними
        private readonly IProductService _productService;
        private readonly IProductTypeService _productTypeService;
        private readonly IProductImageService _productImageService;
        private readonly IProductColorService _productColorService;
        private readonly IBrandService _brandService;


       



        public AdvertisementController(IDataService dataService, DataContext context)
        {
            _dataService = dataService;
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
        public async Task<IActionResult> CreateAdvertisement(ProductAdvertisementVM model)
        {
            if (ModelState.IsValid)
            {
                // Припустимо, що у вас є контекст бази даних для доступу до ProductTypes
        var productType = _context.ProductTypes
                                   .FirstOrDefault(pt => pt.Title == model.ProductType);

                if (productType != null)
                {
                    model.ProductTypeId = productType.Id; // Отримуємо ID типу продукту
                }
                else
                {
                    ModelState.AddModelError("ProductType", "Тип продукту не знайдено.");
                    return View(model); // Якщо тип не знайдено, повертаємо помилку
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
                    ProductTypeId = model.ProductTypeId
                };

                // Додаємо продукт до бази даних
                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                // 2. Обробляємо завантажені зображення
                if (model.ImageData != null && model.ImageData.Count > 0)
                {
                    foreach (var image in model.ImageData)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await image.CopyToAsync(memoryStream);
                            var newImage = new ProductImage
                            {
                                ImageData = memoryStream.ToArray(),
                                ProductId = product.Id // Прив'язуємо зображення до продукту
                            };
                            _context.ProductImages.Add(newImage);
                        }
                    }
                }

                // 3. Додаємо колір товару
                //if (!string.IsNullOrEmpty(model.ProductColor))
                //{
                //    var productColor = new ProductColor
                //    {
                //        Title = model.ProductColor,                      
                //    };
                //    _context.ProductColors.Add(productColor);
                //}

                // Зберігаємо всі зміни
                await _context.SaveChangesAsync();

                return RedirectToAction("SuccessPage");
            }

            // Якщо модель некоректна, повертаємо форму з помилками
            return View(model);
        }



        // Об'єднуючий контролер
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> CreateAdvertisement(ProductAdvertisementVM viewModel)
        //{
        //    // 1. Створення продукту
        //    var productResult = await _productsController.Create(viewModel.Product);

        //    if (productResult is NotFoundResult || productResult is BadRequestResult)
        //    {
        //        // Обробка помилок
        //        return BadRequest("помилка при створенні оголошення");
        //    }

        //    // 2. Додавання зображень
        //    // Приведение типа к OkObjectResult, чтобы получить доступ к объекту
        //    var createdProduct = (productResult as OkObjectResult)?.Value as Product;

        //    if (createdProduct == null)
        //    {
        //        return BadRequest("помилка при отриманні продукта");
        //    }

        //    var productId = createdProduct.Id;  // Отримання Id нового продукту

        //    var imageResult = await _productImagesController.Create(productId, viewModel.ImageData);

        //    if (imageResult is NotFoundResult || imageResult is BadRequestResult)
        //    {
        //        return BadRequest("Ошибка при добавлении изображений");
        //    }

        //    // 3. Добавление типа товара
        //    var productTypeResult = await _productTypesController.Create(viewModel.ProductType);

        //    // 4. Добавление цвета
        //    var productColorResult = await _productColorsController.Create(viewModel.ProductColor);

        //    // 5. Добавление бренда
        //    var brandResult = await _brandsController.Create(viewModel.Brand, viewModel.BrandImage);

        //    // 6. Добавление подкатегорий
        //    var childCategoryResult = await _childcategoriesController.Create(viewModel.Childcategory);
        //    var subChildCategoryResult = await _subChildCategoryController.Create(viewModel.SubChildCategory);



        //    return RedirectToAction("Index", "Home"); // Перенаправлення після успішного завершення
        //}

    }
}
