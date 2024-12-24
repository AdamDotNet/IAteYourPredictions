using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TryingTwitchOAuth.Data;
using TryingTwitchOAuth.Extensions;

namespace TryingTwitchOAuth.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
		private readonly PredictionsDbContext _dbContext;

		public IndexModel(ILogger<IndexModel> logger, PredictionsDbContext dbContext)
        {
            _logger = logger;
			_dbContext = dbContext;
		}

		public Prediction Prediction { get; set; }

		public PredictionEntry Entry { get; set; }

		[BindProperty]
		public int PredictionId { get; set; }

		[BindProperty]
		public int OptionId { get; set; }

		public async Task OnGetAsync(int? id)
        {
			if (!id.HasValue)
			{
				Prediction = await _dbContext.OrderedPredictions.Where(p => p.IsOpen).Include(p => p.Options).FirstOrDefaultAsync();
			}
			else
			{
				Prediction = await _dbContext.Predictions.Where(p => p.Id == id).Include(p => p.Options).FirstOrDefaultAsync();
			}

			if (Prediction is not null)
			{
				Entry = await _dbContext.PredictionEntries.FirstOrDefaultAsync(p => p.PredictionId == Prediction.Id && p.TwitchUid == User.GetIdentifier());
			}

			if (User.Identity.IsAuthenticated && Prediction is not null && Prediction.IsOpen && !Prediction.EntriesAreOpen)
			{
				TempData["Message"] = "The prediction is closed to entries.";
				TempData["Style"] = "alert-warning";
			}
		}

        public async Task<IActionResult> OnPostAsync()
        {
			if (!User.Identity.IsAuthenticated)
			{
				TempData["Message"] = "Please sign in to predict.";
				TempData["Style"] = "alert-danger";
				return RedirectToPage("./Index");
			}

			var prediction = await _dbContext.Predictions.Where(p => p.Id == PredictionId).Include(p => p.Options).FirstOrDefaultAsync();
			if (prediction is null)
			{
				TempData["Message"] = "Prediction not found.";
				TempData["Style"] = "alert-danger";
				return RedirectToPage("./Index");
			}

			var existingPrediction = await _dbContext.PredictionEntries.FirstOrDefaultAsync(p => p.PredictionId == PredictionId && p.TwitchUid == User.GetIdentifier());
			if (existingPrediction is not null)
			{
				TempData["Message"] = "You cannot cast multiple predictions.";
				TempData["Style"] = "alert-danger";
				return RedirectToPage("./Index");
			}

			var option = prediction.Options.FirstOrDefault(o => o.Id == OptionId);
			if (option is null)
			{
				TempData["Message"] = "Prediction option not found.";
				TempData["Style"] = "alert-danger";
				return RedirectToPage("./Index");
			}

			var entry = new PredictionEntry
			{
				Prediction = prediction,
				Option = option,
				CastTime = DateTime.UtcNow,
				TwitchUid = User.GetIdentifier(),
				TwitchUserName = User.Identity.Name,
				TwitchDisplayName = User.GetDisplayName()
			};

			_dbContext.PredictionEntries.Add(entry);
			await _dbContext.SaveChangesAsync();

			TempData["Message"] = "Prediction has been made!";
            TempData["Style"] = "alert-success";
            return RedirectToPage("./Index");
		}
    }
}
