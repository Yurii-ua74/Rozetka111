using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Rozetka.Data.Entity;
using Rozetka.Models.UserModel;
using Rozetka.Models.ViewModels.Account;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Rozetka.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
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
            //return View(viewModel);
            return RedirectToAction("Index", "Home");

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
                        // Вернемся на предыдущую страницу
                        string refererUrl = Request.Headers["Referer"].ToString();
                        if (!string.IsNullOrEmpty(refererUrl))
                        {
                            return Redirect(refererUrl);
                        }

                        // Если заголовок Referer пустой, можно вернуться на главную или другую дефолтную страницу
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            Console.WriteLine("Logout called");

            await _signInManager.SignOutAsync();
            HttpContext.Session.Clear(); // Очистка сессии

            // Очистка cookies
            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(-1),
                IsPersistent = false
            };
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme, authProperties);

            // Получаем URL предыдущей страницы
            string refererUrl = Request.Headers["Referer"].ToString();

            // Проверяем, что предыдущая страница - это BonusPage
            if (!string.IsNullOrEmpty(refererUrl) && refererUrl.Contains("/Account/BonusPage"))
            {
                // Перенаправляем на главную страницу, если пользователь был на BonusPage
                return RedirectToAction("Index", "Home");
            }

            // Перенаправляем на предыдущую страницу, если она есть; иначе - на главную
            return !string.IsNullOrEmpty(refererUrl) ? Redirect(refererUrl) : RedirectToAction("Index", "Home");
        }


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
        

        // Get
        // /////  сторінка редагування даних користувача  ///// //
        public async Task<IActionResult> Edit()
        {
            // Отримуємо ім'я користувача
            var userName = User.Identity.Name;

            if (userName == null)
            {
                return NotFound(); // Якщо користувач не знайдений, повертаємо 404
            }

            // Знаходимо користувача за його UserName
            var user = await _userManager.FindByNameAsync(userName);

            if (user == null)
            {
                TempData["Message"] = "користувача не знайдено!";
                return RedirectToAction("Edit"); // або на іншу сторінку
                //return NotFound(); // Якщо користувача немає в базі даних, повертаємо 404
            }  

            // Передаємо дані користувача до вьюшки
            return View(user);
        }

        public async Task<IActionResult> BonusPage()
        {
            // Отримуємо ім'я користувача
            var userName = User.Identity.Name;

            if (userName == null)
            {
                return NotFound(); // Якщо користувач не знайдений, повертаємо 404
            }

            // Знаходимо користувача за його UserName
            var user = await _userManager.FindByNameAsync(userName);

            if (user == null)
            {
                TempData["Message"] = "користувача не знайдено!";
                return RedirectToAction("BonusPage"); // або на іншу сторінку
                //return NotFound(); // Якщо користувача немає в базі даних, повертаємо 404
            }

            // Передаємо дані користувача до вьюшки
            return View(user);
        }


        // /////////  метод для зміни Email  ///////// //
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeEmail(ChangeEmailViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Будь ласка, виправте помилки у формі.";
                return RedirectToAction("Edit", "Account");
            }

            // Отримуємо поточного користувача
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                TempData["ErrorMessage"] = "Користувача не знайдено. Будь ласка, увійдіть знову.";
                return RedirectToAction("Login", "Account");  // редирект на сторінку повторного входу

            }      

            // Перевіряємо правильність пароля
            var passwordCheck = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

            if (!passwordCheck.Succeeded)
            {
                TempData["ErrorMessage"] = "Невірний пароль.";
                return RedirectToAction("Edit", "Account");
            }  //////

            // Оновлюємо email
            user.Email = model.Email;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                // Повертаємо повідомлення про успішну зміну
                TempData["SuccessMessage"] = "Електронну пошту успішно змінено!";               
                return RedirectToAction("Edit", "Account");
            }

            // Якщо сталася помилка при оновленні
            TempData["ErrorMessage"] = "Помилка при зміні номера телефону.";
            return RedirectToAction("Edit", "Account");
        }


        // /////////  метод для зміни PhoneNumber  ///////// //
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePhoneNumber(ChangePhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {                
                TempData["ErrorMessage"] = "Будь ласка, виправте помилки у формі.";
                return RedirectToAction("Edit", "Account");
            }

            // Отримуємо поточного користувача
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                TempData["ErrorMessage"] = "Користувача не знайдено. Будь ласка, увійдіть знову.";
                return RedirectToAction("Login", "Account");  // редирект на сторінку повторного входу

            }

            // Перевіряємо правильність пароля
            var passwordCheck = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

            if (!passwordCheck.Succeeded)
            {
                TempData["ErrorMessage"] = "Невірний пароль.";
                return RedirectToAction("Edit", "Account");
            }

            // Оновлюємо номер телефону
            user.PhoneNumber = model.PhoneNumber;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                // Повертаємо повідомлення про успішну зміну
                TempData["SuccessMessage"] = "Номер телефону успішно змінено!";
                return RedirectToAction("Edit", "Account"); 
            }

            // Якщо сталася помилка при оновленні
            TempData["ErrorMessage"] = "Помилка при зміні номера телефону.";
            return RedirectToAction("Edit", "Account");
        }


        // /////////  метод для зміни Password  ///////// //
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Будь ласка, виправте помилки у формі.";
                return RedirectToAction("Edit", "Account");
            }

            // Отримуємо поточного користувача
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Користувача не знайдено. Будь ласка, увійдіть знову.";
                return RedirectToAction("Login", "Account");  // редирект на сторінку повторного входу
            }

            // Перевіряємо правильність пароля
            var passwordCheck = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!passwordCheck.Succeeded)
            {
                TempData["ErrorMessage"] = "Невірний старий пароль.";
                return RedirectToAction("Edit", "Account");
            }

            // Оновлюємо пароль
            var result = await _userManager.ChangePasswordAsync(user, model.Password, model.NewPassword);
            if (result.Succeeded)
            {
                // Повертаємо повідомлення про успішну зміну
                TempData["SuccessMessage"] = "Пароль успішно змінено!";
                return RedirectToAction("Edit", "Account");
            }

            // Якщо сталася помилка при оновленні
            TempData["ErrorMessage"] = "Помилка при зміні пароля.";
            return RedirectToAction("Edit", "Account");
        }


        // ////// видалення профілю користувача ///// //
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProfile()
        {
            var user = await _userManager.GetUserAsync(User);  // Отримуємо поточного користувача
            if (user == null)
            {
                TempData["ErrorMessage"] = "Користувач не знайдений.";
                return RedirectToAction("Index", "Home");  // Редірект на головну сторінку
            }

            var result = await _userManager.DeleteAsync(user);  // Видалення користувача

            if (result.Succeeded)
            {
                await _signInManager.SignOutAsync();  // Вихід з системи після видалення
                TempData["SuccessMessage"] = "Профіль видалено.";
                return RedirectToAction("Index", "Home");  // Редірект на головну сторінку після успішного видалення
            }

            // Якщо виникла помилка під час видалення
            TempData["ErrorMessage"] = "Не вдалося видалити профіль.";
            return RedirectToAction("Edit", "Account");  // Повернення на сторінку редагування профілю
        }


        // /////  для перевірки авторизації по кліку на Стати продавцем  ///// //
        [HttpGet]
        public IActionResult IsUserLoggedIn()
        {
            // Перевіряємо чи користувач авторизований
            bool isAuthenticated = User.Identity.IsAuthenticated;

            return Json(new { isAuthenticated });
        }


        // ///// повернення даних користувача ///// //
        [HttpGet]
        public async Task<IActionResult> GetUserData()
        {
            // Отримання Id поточного користувача
            var userId = _userManager.GetUserId(User);

            if (userId != null)
            {
                // Отримання даних користувача через UserManager
                var user = await _userManager.FindByIdAsync(userId);

                if (user != null)
                {
                    // Повертаємо дані користувача як JSON
                    return Json(new
                    {
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,                      
                    });
                }
            }

            // Якщо користувач не знайдений або не автентифікований, повертаємо помилку
            return Json(new { error = "Користувач не знайдений або не автентифікований." });
        }


        // Метод для отримання даних від користувача і зміни ролі
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BecomeSeller(BecomeSellerViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Дані не вірні.";
                return RedirectToAction("Index", "Home");
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Такого користувача не знайдено.";
                return RedirectToAction("Index", "Home");
            }

            // Оновлюємо ім'я користувача (UserName) на введену назву продавця
            user.UserName = model.NameSeller;
            user.NormalizedUserName = model.NameSeller.ToUpper(); // Оновлюємо NormalizedUserName
            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                TempData["ErrorMessage"] = "Не вдалося оновити користувача.";
                return RedirectToAction("Index", "Home");
            }

            // Додаємо роль "Seller" користувачу
            var roleExists = await _roleManager.RoleExistsAsync("Seller");
            if (!roleExists)
            {
                await _roleManager.CreateAsync(new IdentityRole("Seller"));
            }

            var addRoleResult = await _userManager.AddToRoleAsync(user, "Seller");
            if (!addRoleResult.Succeeded)
            {
                TempData["ErrorMessage"] = "Не вдала спроба. Спробуйте пізніше ще раз.";
                return RedirectToAction("Index", "Home");
            }

            TempData["SuccessMessage"] = "Вітаю! Ви стали продавцем. Тепер можете додати свої товари.";
            return RedirectToAction("Index", "Home");
        }

        // ///// викликається із сторінки редагування даних кнопкою ///// //
        [HttpPost]
        public async Task<IActionResult> UpdatePersonalData()
        {
            // Отримуємо поточного користувача
            var user = await _userManager.GetUserAsync(User);

            
            if (user == null)
            {
                TempData["ErrorMessage"] = "Користувач не знайдений.";
                return RedirectToAction("Index", "Home");  // Редірект на головну сторінку
            }

            TempData["SuccessMessage"] = "Вітаю! Ваша заявка на зміну даних успішно оформлена, після перевірки модератором дані будуть змінені";

            //return RedirectToAction("Edit"); 
            return RedirectToAction("Edit", "Account");
        }
    }

}
