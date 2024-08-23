using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Rozetka.Data.Entity;
using Rozetka.Models.ViewModels.Account;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Rozetka.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }


        // GET: Account/Register
        public IActionResult Register()
        {
            return View();
        }
        //Відображає форму реєстрації.
        //Викликається при переході до сторінки за URL Account/Register.
        //Повертає порожню форму для введення даних користувача.



        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {              
                var user = new User
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    RegisterDt = DateTime.Now
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }
        //Обробляє дані реєстрації, створює нового користувача і виконує вхід.
        //Викликається при відправці форми реєстрації (метод POST) за URL Account/Register.
        //Перевіряє валідність моделі, створює нового користувача, додає його в систему і виконує вхід,
        //або повертає помилки реєстрації.


        // GET: Account/Login
        public IActionResult Login(string? returnUrl)
        {
            LoginViewModel viewModel = new LoginViewModel() { ReturnUrl = returnUrl };
            return View(viewModel);
        }
        //Відображає форму входу.
        //Викликається при переході до сторінки за URL Account/Login.
        //Повертає порожню форму для введення даних для входу.



        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(viewModel.Email);
                if (user != null)
                {
                    // PasswordSignInAsync приймає користувача, пароль, флаг "Запам'ятати мене" та інші параметри
                    // ASP.NET Identity автоматично обробляє хешування пароля та порівнює його з хешем, збереженим у базі даних.
                    // Якщо паролі збігаються, користувача аутентифікують.
                    var result = await _signInManager.PasswordSignInAsync(user, viewModel.Password, viewModel.RememberMe, lockoutOnFailure: false);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else if (result.IsLockedOut)
                    {
                        ModelState.AddModelError(string.Empty, "Your account is locked out.");
                    }
                    else if (result.IsNotAllowed)
                    {
                        ModelState.AddModelError(string.Empty, "Sign-in is not allowed.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Incorrect email or password.");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "User not found.");
                }
            }
            return View("LoginRetry", viewModel); // Перенаправлення на сторінку для повторної спроби входу
        }
        //Обробляє дані для входу, виконує аутентифікацію користувача.
        //Викликається при відправці форми входу (метод POST) за URL Account/Login.
        //Перевіряє валідність моделі, знаходить користувача, виконує аутентифікацію і
        //перенаправляє користувача на відповідну сторінку.

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }


        // POST: Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]   // захищений від CSRF атак
        public async Task<IActionResult> Logout()
        {
            Console.WriteLine("Logout called");
            //await _signInManager.SignOutAsync();
            await _signInManager.SignOutAsync();
            HttpContext.Session.Clear(); // Очистка сесії
            // Очистка кукі
            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(-1),
                IsPersistent = false
            };
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme, authProperties);
            return RedirectToAction("Index", "Home");
        }



        //// GET: Account/LoginWithGoogle
        //[HttpGet("login/google")]
        //public IActionResult LoginWithGoogle()
        //{
        //    var redirectUrl = Url.Action("GoogleResponse", "Account");
        //    var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
        //    return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        //}

        //// GET: Account/GoogleResponse
        //[HttpGet("google-response")]
        //public async Task<IActionResult> GoogleResponse()
        //{
        //    var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        //    if (!result.Succeeded)
        //    {
        //        Console.WriteLine("Authentication failed.");
        //        return BadRequest();
        //    }

        //    var email = result.Principal.FindFirstValue(ClaimTypes.Email);
        //    var fullName = result.Principal.FindFirstValue(ClaimTypes.Name);

        //    // Разделяем полное имя на имя и фамилию
        //    var names = fullName?.Split(' ', 2);
        //    var firstName = names?.FirstOrDefault();
        //    var lastName = names?.Skip(1).FirstOrDefault();

        //    var user = await _userManager.FindByEmailAsync(email);
        //    if (user == null)
        //    {
        //        // Создайте нового пользователя, если он не существует
        //        user = new User
        //        {
        //            Email = email,
        //            UserName = email,
        //            FirstName = firstName,
        //            LastName = lastName,
        //            RegisterDt = DateTime.Now
        //        };

        //        var createResult = await _userManager.CreateAsync(user);
        //        if (!createResult.Succeeded)
        //        {
        //            // Обработка ошибок создания пользователя
        //            Console.WriteLine("User creation failed.");
        //            return BadRequest(createResult.Errors);
        //        }
        //    }

        //    await _signInManager.SignInAsync(user, isPersistent: false);
        //    Console.WriteLine("User signed in successfully.");
        //    return RedirectToAction("Index", "Home");
        //}


        [AllowAnonymous]
        public IActionResult GoogleLogin()
        {
            string? returnUrl = Url.Action("GoogleResponse");
            var authProperties = _signInManager.ConfigureExternalAuthenticationProperties("Google", returnUrl);
            return new ChallengeResult("Google", authProperties);
        }
        //Починає процес аутентифікації через Google.
        //Викликається при переході до сторінки за URL Account/GoogleLogin.
        //Налаштовує параметри для аутентифікації через Google і
        //викликає виклик зовнішнього провайдера аутентифікації.


        [AllowAnonymous]
        public async Task<IActionResult> GoogleResponse(string? returnUrl)
        {
            ExternalLoginInfo? loginInfo = await _signInManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
                return RedirectToAction("Login");
            var loginResult = await _signInManager
                .ExternalLoginSignInAsync(loginInfo.LoginProvider,
                loginInfo.ProviderKey,
                false);
            string[] userInfo = {
                loginInfo.Principal.FindFirst(ClaimTypes.Surname)!.Value,
                loginInfo.Principal.FindFirst(ClaimTypes.Email)!.Value
            };
            if (loginResult.Succeeded)
            {
                return View(userInfo);
            }
            User? user = await _userManager.FindByEmailAsync(userInfo[1]);
            IdentityResult? identityResult = null;
            if (user == null)
            {
                user = new User()
                {
                    UserName = userInfo[0],
                    Email = userInfo[1]
                };
                identityResult = await _userManager.CreateAsync(user);
            }
            else
                identityResult = await _userManager.AddLoginAsync(user, loginInfo);
            if (identityResult.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return View(userInfo);
            }
            return RedirectToAction("AccessDenied");
        }
        //Обробляє відповідь від Google після аутентифікації.
        //Викликається автоматично після аутентифікації через Google.
        //Отримує інформацію про користувача від Google, виконує вхід або реєстрацію користувача в системі і
        //перенаправляє його на відповідну сторінку.




        // Метод для начала процесса аутентификации через Facebook
        [AllowAnonymous]
        public IActionResult FacebookLogin()
        {
            string? returnUrl = Url.Action("FacebookResponse");
            var authProperties = _signInManager.ConfigureExternalAuthenticationProperties(FacebookDefaults.AuthenticationScheme, returnUrl);
            return new ChallengeResult(FacebookDefaults.AuthenticationScheme, authProperties);
        }

        // Метод для обработки ответа от Facebook
        [AllowAnonymous]
        public async Task<IActionResult> FacebookResponse(string? returnUrl = null)
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
                return RedirectToAction("Login");

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            if (result.Succeeded)
            {
                return RedirectToLocal(returnUrl);
            }

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                // Создаем нового пользователя, если его нет
                user = new User
                {
                    UserName = email,
                    Email = email
                };
                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    // Обработка ошибок создания пользователя
                    return RedirectToAction("AccessDenied");
                }
            }

            // Добавляем внешний логин к пользователю
            var loginResult = await _userManager.AddLoginAsync(user, info);
            if (loginResult.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToLocal(returnUrl);
            }

            return RedirectToAction("AccessDenied");
        }

        // TEST

    }
}