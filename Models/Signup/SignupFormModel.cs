using Microsoft.AspNetCore.Mvc;

namespace Rozetka.Models.Signup
{
    public class SignupFormModel
    {
        [FromForm(Name = "signup-firstname")]
        public String FirstName { get; set; } = null!;

        [FromForm(Name = "signup-lastname")]
        public String LastName { get; set; } = null!;

        [FromForm(Name = "signup-phone")]
        public String Phone { get; set; } = null!;

        [FromForm(Name = "signup-email")]
        public String Email { get; set; } = null!;

        [FromForm(Name = "signup-password")]
        public String Password { get; set; } = null!;

        [FromForm(Name = "signup-repeat")]
        public String Repeat { get; set; } = null!;

    }
}
