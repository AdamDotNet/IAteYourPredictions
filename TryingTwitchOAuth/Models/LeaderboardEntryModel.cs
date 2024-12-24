namespace TryingTwitchOAuth.Models
{
	public class LeaderboardEntryModel
	{
		public string TwitchUid { get; set; }

		public string TwitchUserName { get; set; }

		public string TwitchDisplayName { get; set; }

		public int Count { get; set; }
	}
}
