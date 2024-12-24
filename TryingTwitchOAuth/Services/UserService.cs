using Microsoft.Extensions.Options;
using TryingTwitchOAuth.Extensions;
using TryingTwitchOAuth.Options;

namespace TryingTwitchOAuth.Services
{
	public class UserService
	{
		private readonly IOptionsMonitor<TwitchOptions> _options;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public UserService(IOptionsMonitor<TwitchOptions> options, IHttpContextAccessor httpContextAccessor)
		{
			_options = options;
			_httpContextAccessor = httpContextAccessor;
		}

		public bool UserCanEdit()
		{
			var authenticated = _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated;
			if (!authenticated)
			{
				return false;
			}

			var twitchUid = _httpContextAccessor.HttpContext.User.GetIdentifier();
			if (twitchUid is null)
			{
				return false;
			}

			return _options.CurrentValue.Admins.ContainsKey(twitchUid);
		}
	}
}
