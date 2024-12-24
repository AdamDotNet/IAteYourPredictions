using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TryingTwitchOAuth.Data;
using TryingTwitchOAuth.Extensions;
using TryingTwitchOAuth.Services;

namespace TryingTwitchOAuth.Pages
{
    public class AuthorModel : PageModel
    {
        private readonly PredictionsDbContext _dbContext;
		private readonly UserService _userService;
		private readonly ILogger<AuthorModel> _logger;

		public AuthorModel(PredictionsDbContext dbContext, UserService userService, ILogger<AuthorModel> logger)
        {
            _dbContext = dbContext;
			_userService = userService;
			_logger = logger;
		}

        public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
			if (!_userService.UserCanEdit())
			{
				context.Result = RedirectToPage("/Index");
			}
		}

		[BindProperty]
        public Prediction Prediction { get; set; }

        public bool IsCreate { get; set; }

		[BindProperty]
		public int WinnerOptionId { get; set; }

		public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id.HasValue)
            {
                Prediction = await _dbContext.Predictions.Include(p => p.Options).FirstOrDefaultAsync(p => p.Id == id.Value);
                if (Prediction is null)
                {
					// Bad Id, redirect to create.
					return RedirectToPage("./Author", new { id = default(int?) });
				}
			}
            
            if (Prediction is null)
            {
                Prediction = new()
                {
                    IsOpen = true,
                    Conference = Conference.ACF,
                    Options =
                    [
                        new(),
                        new()
                    ]
                };
                IsCreate = true;
            }

            return Page();
        }

		public async Task<IActionResult> OnPostDeleteAsync(int id)
		{
			var prediction = await _dbContext.Predictions.FirstOrDefaultAsync(p => p.Id == id);
			if (prediction is null)
			{
				TempData["Style"] = "alert-fail";
				TempData["Message"] = "Prediction not found.";
				return RedirectToPage("./Author");
			}

            _logger.LogInformation("Deleting prediction {PredictionId} by user {User}.", id, User.GetIdentifier());
			_dbContext.Remove(prediction);
			await _dbContext.SaveChangesAsync();
			TempData["Style"] = "alert-success";
			TempData["Message"] = "Prediction entry has been deleted.";
			return RedirectToPage("./Author", new { prediction.Id });
		}

		public async Task<IActionResult> OnPostCloseEntriesAsync(int id)
        {
            var prediction = await _dbContext.Predictions.FirstOrDefaultAsync(p => p.Id == id);
            if (prediction is null)
            {
                TempData["Style"] = "alert-fail";
                TempData["Message"] = "Prediction not found.";
                return RedirectToPage("./Author");
            }

            prediction.EntriesAreOpen = false;
            prediction.ModifiedAt = DateTime.UtcNow;

			_logger.LogInformation("Closing entries for prediction {PredictionId} by user {User}.", id, User.GetIdentifier());
			await _dbContext.SaveChangesAsync();
            TempData["Style"] = "alert-success";
            TempData["Message"] = "Prediction entry has been closed.";
            return RedirectToPage("./Author", new { prediction.Id });
        }

        public async Task<IActionResult> OnPostOpenEntriesAsync(int id)
        {
            var prediction = await _dbContext.Predictions.FirstOrDefaultAsync(p => p.Id == id);
            if (prediction is null)
            {
                TempData["Style"] = "alert-fail";
                TempData["Message"] = "Prediction not found.";
                return RedirectToPage("./Author");
            }

            prediction.EntriesAreOpen = true;
			prediction.ModifiedAt = DateTime.UtcNow;

			_logger.LogInformation("Opening entries for prediction {PredictionId} by user {User}.", id, User.GetIdentifier());
			await _dbContext.SaveChangesAsync();
            TempData["Style"] = "alert-success";
            TempData["Message"] = "Prediction entry has been opened.";
            return RedirectToPage("./Author", new { prediction.Id });
        }

        public async Task<IActionResult> OnPostDeclareWinnerAsync(int id)
        {
			var prediction = await _dbContext.Predictions.Include(p => p.Options).FirstOrDefaultAsync(p => p.Id == id);
            if (prediction is null)
            {
				TempData["Style"] = "alert-fail";
				TempData["Message"] = "Prediction not found.";
				return RedirectToPage("./Author");
			}

            var winningOption = prediction.Options.FirstOrDefault(o => o.Id == WinnerOptionId);
            if (winningOption is null)
            {
				TempData["Style"] = "alert-fail";
				TempData["Message"] = "Prediction option not found.";
				return RedirectToPage("./Author", new { prediction.Id });
			}

            await using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
					_logger.LogInformation("Declaring prediction {PredictionId} winner as {OptionId} by user {User}.", id, WinnerOptionId, User.GetIdentifier());

					winningOption.IsWinner = true;
					prediction.IsOpen = false;
                    prediction.EntriesAreOpen = false;
					prediction.ModifiedAt = DateTime.UtcNow;
					// Update the prediction.
					await _dbContext.SaveChangesAsync();
					// Set score on all correct entries.
					await _dbContext.PredictionEntries.Where(e => e.PredictionId == id && e.OptionId == WinnerOptionId).ExecuteUpdateAsync(e => e.SetProperty(e => e.IsCorrect, true));

					// Ensure both updates commit for data integrity.
					await transaction.CommitAsync();
				}
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
					TempData["Style"] = "alert-fail";
					TempData["Message"] = $"Failed to set prediction winner: {ex.Message}.";
					return RedirectToPage("./Author", new { prediction.Id });
				}
            }

			TempData["Style"] = "alert-success";
			TempData["Message"] = "Prediction winner declared.";
			return RedirectToPage("./Author", new { prediction.Id });
		}

		public async Task<IActionResult> OnPostSaveAsync(int? id)
        {
            var now = DateTime.UtcNow;
            Prediction existing = null;

            if (id.HasValue)
            {
                existing = await _dbContext.Predictions.Include(p => p.Options).FirstOrDefaultAsync(p => p.Id == id);
            }

            if (existing is null)
            {
                // Create
                Prediction.CreatedAt = now;
                Prediction.ModifiedAt = now;
                Prediction.IsOpen = true;
                _dbContext.Predictions.Add(Prediction);

				_logger.LogInformation("Created prediction {PredictionId} by user {User}.", Prediction.Id, User.GetIdentifier());
				await _dbContext.SaveChangesAsync();
				TempData["Style"] = "alert-success";
				TempData["Message"] = "Prediction saved successfully.";
				return RedirectToPage("./Author", new { Prediction.Id });
            }
            else
            {
                Prediction.ModifiedAt = now;

                // Merge properties of posted values to the existing entry.
                existing.Title = Prediction.Title;

                // Merge options, adding or deleting as necessary.
                for (int i = 0; i < Prediction.Options.Count; i++)
                {
                    var postedOption = Prediction.Options[i];
                    if (i < existing.Options.Count)
                    {
                        existing.Options[i].DisplayText = postedOption.DisplayText;
                    }
                    else
                    {
                        existing.Options.Add(postedOption);
                    }
                }

                while (existing.Options.Count > Prediction.Options.Count)
                {
                    _dbContext.Remove(existing.Options[^1]);
                    existing.Options.RemoveAt(existing.Options.Count - 1);
                }

				_logger.LogInformation("Updated prediction {PredictionId} by user {User}.", existing.Id, User.GetIdentifier());
				await _dbContext.SaveChangesAsync();
                TempData["Style"] = "alert-success";
                TempData["Message"] = "Prediction saved successfully.";
                return RedirectToPage("./Author", new { existing.Id });
            }
        }
    }
}
