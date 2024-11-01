using Microsoft.AspNetCore.Mvc;

namespace Rozetka.Controllers
{
    public class InfoController : Controller
    {
        public IActionResult Index() { return View(); }

        public IActionResult AboutUs() { return View(); }

        public IActionResult Vacantions() { return View(); }

        public IActionResult FastBuyChange() { return View(); }

        public IActionResult QuestionsAndAnswers() { return View(); }
    }
}
