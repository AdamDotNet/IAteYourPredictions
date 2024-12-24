namespace TryingTwitchOAuth.Data
{
	public class PredictionEntry
	{
		public int Id { get; set; }

		public string TwitchUid { get; set; }

		public string TwitchUserName { get; set; }

		public string TwitchDisplayName { get; set; }

		public DateTime CastTime { get; set; }

		public bool IsCorrect { get; set; }

		public int OptionId { get; set; }

		public PredictionOption Option { get; set; }

		public int PredictionId { get; set; }

		public Prediction Prediction { get; set; }
	}
}
