using Microsoft.EntityFrameworkCore;

namespace TryingTwitchOAuth.Data
{
	public class PredictionsDbContext : DbContext
	{
		public PredictionsDbContext(DbContextOptions<PredictionsDbContext> options)
			: base(options)
		{
		}

		public DbSet<Prediction> Predictions { get; set; }

		public IQueryable<Prediction> OrderedPredictions => Predictions.OrderByDescending(p => p.CreatedAt);

		public DbSet<PredictionEntry> PredictionEntries { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Prediction>()
				.HasKey(p => p.Id);
			modelBuilder.Entity<PredictionOption>()
				.HasKey(p => p.Id);
			modelBuilder.Entity<PredictionEntry>()
				.HasKey(p => p.Id);

			modelBuilder.Entity<Prediction>()
				.ToTable("Predictions")
				.HasMany(p => p.Options)
				.WithOne(p => p.Prediction)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Prediction>()
				.HasMany(p => p.Entries)
				.WithOne(p => p.Prediction)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<PredictionOption>()
				.HasMany(p => p.Entries)
				.WithOne(p => p.Option)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
