using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Rozetka.Models.UserModel
{
    public class UserModel: IdentityUser
    {
        [Required]
        [Display(Name = "Нiкнейм")]
        public string UserName { get; set; }
        
    }
}
