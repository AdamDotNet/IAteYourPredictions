using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TryingTwitchOAuth.Data;
using TryingTwitchOAuth.Extensions;

namespace TryingTwitchOAuth.Pages
{
    public class StatsModel : PageModel
    {
		private readonly PredictionsDbContext _dbContext;

		public StatsModel(PredictionsDbContext dbContext)
		{
			_dbContext = dbContext;
		}
		public int TotalPredictions { get; set; }

		public int WonPredictions { get; set; }

		public int LostPredictions { get; set; }

		public string TwitchUid { get; set; }

		public string TwitchDisplayName { get; set; }

		public string TwitchUserName { get; set; }

		public async Task OnGetAsync(string twitchUid)
        {
			if (string.IsNullOrEmpty(twitchUid) && User.Identity.IsAuthenticated)
			{
				twitchUid = User.GetIdentifier();
				if (string.IsNullOrEmpty(twitchUid))
				{
					TwitchDisplayName = "User Not Found";
					TwitchUserName = "User Not Found";
					return;
				}
			}

			TwitchUid = twitchUid;
			var anyEntry = await _dbContext.PredictionEntries.FirstOrDefaultAsync(p => p.TwitchUid == twitchUid);
			if (anyEntry is not null)
			{
				TwitchDisplayName = anyEntry.TwitchDisplayName;
				TwitchUserName = anyEntry.TwitchUserName;

				// Count of predictions participated in.
				TotalPredictions = await _dbContext.PredictionEntries
					.Where(p => p.TwitchUid == twitchUid)
					.CountAsync();

				// Count of predictions won.
				WonPredictions = await _dbContext.PredictionEntries
					.Where(p => p.TwitchUid == twitchUid && p.IsCorrect)
					.CountAsync();

				// Count of predictions lost.
				LostPredictions = await _dbContext.PredictionEntries
					.Where(p => p.TwitchUid == twitchUid && !p.IsCorrect && !p.Prediction.IsOpen)
					.CountAsync();
			}
			else
			{
				TwitchDisplayName = "User Not Found";
				TwitchUserName = "User Not Found";
			}
		}
	}
}
