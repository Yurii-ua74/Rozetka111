using System.ComponentModel.DataAnnotations;

namespace Rozetka.Models.UserModel
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Новий пароль є обов'язковим.")]
        [StringLength(100, ErrorMessage = "Пароль має містити принаймні {2} символів.", MinimumLength = 6)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Будь ласка, підтвердіть новий пароль.")]
        [Compare("NewPassword", ErrorMessage = "Паролі не співпадають.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Старий пароль є обов'язковим.")]
        public string Password { get; set; }
    }
}
