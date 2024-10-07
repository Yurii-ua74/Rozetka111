using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Rozetka.Models.UserModel
{
    public class ChangeEmailViewModel: IdentityUser
    {
        [Required(ErrorMessage = "Будь ласка, введіть нову електронну пошту.")]
        [EmailAddress(ErrorMessage = "Введіть коректну електронну пошту.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Будь ласка, введіть ваш пароль.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
