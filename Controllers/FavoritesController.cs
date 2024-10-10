using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rozetka.Data;
using Rozetka.Data.Entity;
using System.Security.Claims;

namespace Rozetka.Controllers
{
    public class FavoritesController : Controller
    {
        private readonly DataContext _context;

        public FavoritesController(DataContext context)
        {
            _context = context;
        }

        // GET: Favorites
        public async Task<IActionResult> Index()
        {
            // Получаем ID текущего пользователя
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Проверяем, что пользователь авторизован
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Получаем список избранных товаров для пользователя
            var favorites = await _context.Favorites
                .Where(f => f.UserId == userId)
                .Include(f => f.Product) // Подгружаем информацию о продукте
                .ThenInclude(p => p.ProductImages)
                .ToListAsync();

            // Возвращаем представление с данными
            return View(favorites);
        }

        [Authorize]
        public async Task<IActionResult> AddToFavorites(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Получаем ID пользователя
            if (userId == null)
            {
                return BadRequest("Не удалось определить пользователя.");
            }

            // Проверяем, есть ли уже этот товар в избранном
            var existingWish = await _context.Favorites
                .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);

            if (existingWish == null)
            {
                var favoritesItem = new Data.Entity.Favorites
                {
                    UserId = userId,
                    ProductId = productId
                };

                _context.Favorites.Add(favoritesItem);
                await _context.SaveChangesAsync();
            }

            // Вернемся на предыдущую страницу
            string refererUrl = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(refererUrl))
            {
                return Redirect(refererUrl);
            }

            // Если заголовок Referer пустой, можно вернуться на главную или другую дефолтную страницу
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public async Task<IActionResult> DeleteFromFavorites(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Получаем ID пользователя
            if (userId == null)
            {
                return BadRequest("Не удалось определить пользователя.");
            }

            // Ищем товар в избранном
            var favoritesItem = await _context.Favorites
                .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);

            if (favoritesItem != null)
            {
                _context.Favorites.Remove(favoritesItem);
                await _context.SaveChangesAsync();
            }

            // Вернемся на предыдущую страницу
            string refererUrl = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(refererUrl))
            {
                return Redirect(refererUrl);
            }

            // Если заголовок Referer пустой, можно вернуться на главную или другую дефолтную страницу
            return RedirectToAction("Index", "Home");
        }



        // GET: FavoritesController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: FavoritesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: FavoritesController/Create
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

        // GET: FavoritesController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: FavoritesController/Edit/5
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

        // GET: FavoritesController/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var favorite = await _context.Favorites.FindAsync(id);

            if (favorite == null)
            {
                return NotFound();
            }

            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // POST: FavoritesController/Delete/5
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

        // Метод для получения количества избранных товаров
        public async Task<int> GetFavoritesCount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Получаем ID текущего пользователя

            if (userId == null)
            {
                return 0;
            }

            // Получаем количество товаров в избранном
            var count = await _context.Favorites
                .Where(f => f.UserId == userId)
                .CountAsync();

            return count;
        }
    }
}
