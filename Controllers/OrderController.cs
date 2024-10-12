using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rozetka.Data;
using Rozetka.Data.Entity;
using Rozetka.Models.ViewModels.Cart;
using System.Security.Claims;

namespace Rozetka.Controllers
{
    public class OrderController : Controller
    {

        public IActionResult SuccessOrder()
        {
            return View("~/Views/Order/SuccessOrder.cshtml"); // Укажите полный путь к представлению
        }
    }
}
