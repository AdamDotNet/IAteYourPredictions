namespace TryingTwitchOAuth.Data
{
    public class Prediction
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime ModifiedAt { get; set; }

		public Conference Conference { get; set; }

		public string Title { get; set; }

        public bool IsOpen { get; set; }

        public bool EntriesAreOpen { get; set; }

        public List<PredictionOption> Options { get; set; } = [];

        public List<PredictionEntry> Entries { get; set; } = [];
    }
}
