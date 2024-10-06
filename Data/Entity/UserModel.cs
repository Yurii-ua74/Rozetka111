using System.ComponentModel.DataAnnotations;

namespace Rozetka.Data.Entity
{
    public class UserModel
    {
        [Required]
        [Display(Name = "Нiкнейм")]
        public string UserName { get; set; }
    }
}
