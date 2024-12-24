using AspNet.Security.OAuth.Twitch;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace TryingTwitchOAuth.Controllers
{
	[Route("[controller]")]
	public class AuthenticationController : Controller
	{
		[HttpPost("Login")]
		[ValidateAntiForgeryToken]
		public IActionResult Login()
		{
			return Challenge(new AuthenticationProperties { RedirectUri = "/" }, TwitchAuthenticationDefaults.AuthenticationScheme);
		}

		[HttpPost("Logout")]
		[ValidateAntiForgeryToken]
		public IActionResult Logout()
		{
			return SignOut(new AuthenticationProperties { RedirectUri = "/" }, CookieAuthenticationDefaults.AuthenticationScheme);
		}
	}
}
