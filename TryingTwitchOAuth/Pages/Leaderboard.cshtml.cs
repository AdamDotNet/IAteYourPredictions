using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TryingTwitchOAuth.Data;
using TryingTwitchOAuth.Filtering;
using TryingTwitchOAuth.Models;

namespace TryingTwitchOAuth.Pages
{
    public class LeaderboardModel : PageModel
    {
		private readonly PredictionsDbContext _dbContext;

		public LeaderboardModel(PredictionsDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		[BindProperty(SupportsGet = true)]
		public int Skip { get; set; } = 0;

		[BindProperty(SupportsGet = true)]
		public int Take { get; set; } = 10;

		[BindProperty(SupportsGet = true)]
		public OrderBy OrderBy { get; set; } = OrderBy.Descending;

		public bool HasMore { get; set; }

		public List<LeaderboardEntryModel> LeaderboardEntries { get; set; }

		public async Task OnGetAsync()
        {
			Skip = Skip < 0 ? 0 : Skip;
			Take = Take < 1 ? 1 : Take;
			Take = Take > 50 ? 50 : Take;

			var query = _dbContext.PredictionEntries
				.Where(e => e.IsCorrect)
				.GroupBy(e => e.TwitchUid)
				.Select(g => new LeaderboardEntryModel
				{
					TwitchUid = g.Key,
					TwitchUserName = g.First().TwitchUserName,
					TwitchDisplayName = g.First().TwitchDisplayName,
					Count = g.Count()
				});

			query = OrderBy switch
			{
				OrderBy.Ascending => query.OrderBy(e => e.Count),
				OrderBy.Descending => query.OrderByDescending(e => e.Count),
				_ => query
			};

			query = query.Skip(Skip).Take(Take + 1);

			var results = await query.ToListAsync();
			LeaderboardEntries = results.Take(Take).ToList();
			HasMore = results.Count > Take;
		}
    }
}
