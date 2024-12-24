namespace TryingTwitchOAuth.Data
{
	public class PredictionOption
	{
		public int Id { get; set; }

		public string DisplayText { get; set; }

		public bool IsWinner { get; set; }

		public int PredictionId { get; set; }

		public Prediction Prediction { get; set; }

		public List<PredictionEntry> Entries { get; set; } = [];
	}
}
