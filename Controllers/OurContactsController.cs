using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Rozetka.Data;
using Rozetka.Data.Entity;
using Rozetka.Models.ViewModels.Cart;
using System.Security.Claims;

namespace Rozetka.Controllers
{
    public class OurContactsController : Controller
    {
        public IActionResult Index()
        {
            return View("~/Views/Home/OurContacts.cshtml");
        }
    }
}



