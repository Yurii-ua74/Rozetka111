using System.ComponentModel.DataAnnotations;

namespace Rozetka.Models.ViewModels.Account
{
    public class RegisterViewModel
    {
        [Required]                 // поле є обов'язковим
        [Display(Name = "First Name")]  // ім'я буде відображатися для цього поля у формі
        public string? FirstName { get; set; }

        [Required]                 // поле є обов'язковим
        [Display(Name = "Last Name")]  // ім'я буде відображатися для цього поля у формі
        public string? LastName { get; set; }


        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }


        [Required]                 // поле є обов'язковим
        [EmailAddress]             // валідатор, чи є значення правильною адресою е - пошти.
        [Display(Name = "Email")]  // ім'я буде відображатися для цього поля у формі
        public string Email { get; set; }


        [Required]
        [DataType(DataType.Password)] //  поле містить пароль,застосовує відповідне форматування на вигляді.
        [Display(Name = "Password")]
        public string Password { get; set; }


        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        // перевіряє, чи значення поля збігається із значенням поля Password.
        [Compare("Password", ErrorMessage = "Пароль та підтвердження поролю не збігаються!")]
        public string ConfirmPassword { get; set; }
    }
}
