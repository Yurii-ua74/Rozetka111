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
        public IActionResult Legalinformation() { return View(); }
        public IActionResult Paymentmethods() { return View(); }
        public IActionResult Delivery() { return View(); }
        public IActionResult OpenPickupPoint() { return View(); }
    }
}
