using Microsoft.AspNetCore.Mvc;

namespace Rozetka.Controllers
{
    public class LocationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SaveLocation(string location)
        {
            // Зберігаємо назву міста в сесію
            HttpContext.Session.SetString("SelectedLocation", location);

            return Ok(new { message = "Геолокація збережена" });
        }

        // Метод для отримання геолокації з сесії (за бажанням)
        public IActionResult GetSessionLocation()
        {
            var location = HttpContext.Session.GetString("SelectedLocation");
            if (string.IsNullOrEmpty(location))
            {
                // return NotFound("Геолокація не знайдена в сесії");
                return Json(new { hasLocation = false });
            }
            return Json(new { hasLocation = true, location });
        }
    }
}
