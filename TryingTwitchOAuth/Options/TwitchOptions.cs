namespace TryingTwitchOAuth.Options
{
	public class TwitchOptions
	{
		public string ClientId { get; set; }

		public string ClientSecret { get; set; }

		public Dictionary<string, string> Admins { get; set; } = new(StringComparer.OrdinalIgnoreCase);
	}
}
