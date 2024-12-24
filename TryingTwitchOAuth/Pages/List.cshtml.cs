using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TryingTwitchOAuth.Data;
using TryingTwitchOAuth.Extensions;
using TryingTwitchOAuth.Filtering;

namespace TryingTwitchOAuth.Pages
{
    public class ListModel : PageModel
    {
		private readonly PredictionsDbContext _dbContext;

		public ListModel(PredictionsDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		[BindProperty(SupportsGet = true)]
        public int Skip { get; set; } = 0;

		[BindProperty(SupportsGet = true)]
		public int Take { get; set; } = 10;

		[BindProperty(SupportsGet = true)]
		public OrderBy OrderBy { get; set; } = OrderBy.Descending;

		[BindProperty(SupportsGet = true)]
		public OpenFilter OpenFilter { get; set; }

		[BindProperty(SupportsGet = true)]
		public OpenFilter EntriesOpenFilter { get; set; }

		public bool HasMore { get; set; }

		[BindProperty(SupportsGet = true)]
		public ConferenceFilter ConferenceFilter { get; set; }

		[BindProperty(SupportsGet = true)]
		public ResultFilter ResultFilter { get; set; }

		[BindProperty(SupportsGet = true)]
		public string TwitchUid { get; set; }

		public List<Prediction> Predictions { get; set; }

		public async Task OnGetAsync()
        {
			Skip = Skip < 0 ? 0 : Skip;
			Take = Take < 1 ? 1 : Take;
			Take = Take > 50 ? 50 : Take;

			var query = _dbContext.Predictions.AsQueryable();

			query = OrderBy switch
			{
				OrderBy.Descending => query.OrderByDescending(p => p.CreatedAt),
				OrderBy.Ascending => query.OrderBy(p => p.CreatedAt),
				_ => query
			};

			query = ConferenceFilter switch
			{
				ConferenceFilter.ACF => query.Where(p => p.Conference == Conference.ACF),
				ConferenceFilter.NFC => query.Where(p => p.Conference == Conference.NFC),
				ConferenceFilter.Any => query,
				_ => query
			};

			query = ResultFilter switch
			{
				ResultFilter.Won => query.Where(p => p.Entries.Any(e => e.IsCorrect && e.TwitchUid == User.GetIdentifier())),
				ResultFilter.Lost => query.Where(p => p.Entries.Any(e => !e.IsCorrect && e.TwitchUid == User.GetIdentifier())),
				ResultFilter.Any => query,
				_ => query
			};

			query = OpenFilter switch
			{
				OpenFilter.Open => query.Where(p => p.IsOpen),
				OpenFilter.Closed => query.Where(p => !p.IsOpen),
				OpenFilter.Any => query,
				_ => query
			};

			query = EntriesOpenFilter switch
			{
				OpenFilter.Open => query.Where(p => p.EntriesAreOpen),
				OpenFilter.Closed => query.Where(p => !p.EntriesAreOpen),
				OpenFilter.Any => query,
				_ => query
			};

			if (!string.IsNullOrEmpty(TwitchUid))
			{
				// EF Core will generate a parameter for this user input, so no SQL injection.
				query = query.Where(p => p.Entries.Any(e => e.TwitchUid == TwitchUid));
			}

			query = query.Skip(Skip)
				// Get more than asked for to see if we can enable the next button.
				.Take(Take + 1);

			var results = await query.ToListAsync();
			Predictions = results.Take(Take).ToList();
			HasMore = results.Count > Take;
		}
    }
}
