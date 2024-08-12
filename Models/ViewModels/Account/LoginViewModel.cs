using System.ComponentModel.DataAnnotations;

namespace Rozetka.Models.ViewModels.Account
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = default!;  // default! використовується для позначення того,
                                                       // що це поле не може бути null, хоча його значення
                                                       // ініціалізується пізніше.


        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = default!;


        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; } = false;  // Поле RememberMe дозволяє користувачеві вибрати, чи потрібно
                                                       // зберігати його вхід у системі для наступних візитів.

        public string? ReturnUrl { get; set; }  // Поле ReturnUrl допомагає відновити навігацію до тієї сторінки,
                                                // на яку користувач спробував перейти перед входом
    }
}
