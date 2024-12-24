using System.Security.Claims;
using AspNet.Security.OAuth.Twitch;

namespace TryingTwitchOAuth.Extensions
{
	public static class UserExtensions
	{
		public static string GetIdentifier(this ClaimsPrincipal principal) => principal.FindFirstValue(ClaimTypes.NameIdentifier);
		//public static string GetIdentifier(this ClaimsPrincipal principal) => "123";

		public static string GetUserIconUrl(this ClaimsPrincipal principal) => principal.FindFirstValue(TwitchAuthenticationConstants.Claims.ProfileImageUrl);

		public static string GetDisplayName(this ClaimsPrincipal principal) => principal.FindFirstValue(TwitchAuthenticationConstants.Claims.DisplayName);
		//public static string GetDisplayName(this ClaimsPrincipal principal) => "TestUser";
	}
}
