using System.ComponentModel.DataAnnotations;

namespace Rozetka.Models.ViewModels.Account
{
    public class BecomeSellerViewModel
    {
        [Required(ErrorMessage = "Будь ласка, введіть назву продавця.")]
        public string NameSeller { get; set; }
    }
}
