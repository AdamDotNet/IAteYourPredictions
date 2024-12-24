using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using TryingTwitchOAuth.Data;
using TryingTwitchOAuth.Options;
using TryingTwitchOAuth.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<UserService>();

builder.Services.AddDbContext<PredictionsDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var twitchConfig = builder.Configuration.GetRequiredSection("Twitch");
// Copy of config for the client Id and client secret.
var twitchOptions = twitchConfig.Get<TwitchOptions>();
// Also configure a configuration watcher for admin changes.
builder.Services.Configure<TwitchOptions>(twitchConfig);

// Add authentication.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	// Store authentication state in cookies.
	.AddCookie()
	// Perform actual authentication with Twitch.
	.AddTwitch(options =>
    {
        options.ClientId = twitchOptions.ClientId;
        options.ClientSecret = twitchOptions.ClientSecret;
		// Don't ask for the user's email address or any other PII.
		options.Scope.Clear();
	});

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<PredictionsDbContext>();
    await dbContext.Database.MigrateAsync();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapControllers();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
