using System.Security.Claims;

namespace Rozetka.Middleware
{
    public class AuthSessionMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthSessionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        //public async Task Invoke(HttpContext context, IUserService userService)
        //{
        //    if (context.Session.Keys.Contains("AuthUserId"))
        //    {
        //        var userId = context.Session.GetString("AuthUserId");
        //        if (userId != null) {
        //            var user = await userService.GetUserByIdAsync(userId);
        //            if (user != null)
        //            {
        //                var claims = new List<Claim>
        //                {
        //                    new Claim(ClaimTypes.Sid, user.Id.ToString()),
        //                    new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
        //                    new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
        //                    new Claim(ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty)

        //                };

        //                context.User = new ClaimsPrincipal(new ClaimsIdentity(claims, nameof(AuthSessionMiddleware)));
        //            }
        //        }
        //    }
        //    await _next(context);
        //}
    }
}