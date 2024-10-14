using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rozetka.Data;
using System.Security.Claims;

namespace Rozetka.Models.ViewModels.Cart
{
    public class CartCountViewComponent : ViewComponent
    {
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartCountViewComponent(DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return View(0); // Если пользователь не авторизован, возвращаем 0
            }

            // Получаем количество избранных товаров
            var count = await _context.Carts
                .Where(c => c.UserId == userId)
                .SumAsync(c => c.Quantity);

            return View(count); // Возвращаем количество
        }
    }
}
