using Microsoft.AspNetCore.Identity;
using Rozetka.Data.Entity;
using System.ComponentModel.DataAnnotations;

namespace Rozetka.Models.UserModel
{
    public class ChangePhoneNumberViewModel
    {
        [Required(ErrorMessage = "Будь ласка, введіть новий номер телефону.")]
        [Phone(ErrorMessage = "Введіть коректний номер телефону.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Будь ласка, введіть ваш пароль.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
