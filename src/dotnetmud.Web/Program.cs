using dotnetmud.Web.Database;
using dotnetmud.Web.Database.Models;
using dotnetmud.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

/*********************************************************************
** Configuration
*/
var builder = WebApplication.CreateBuilder(args);

string dbConnectionString = builder.Configuration.GetRequiredConnectionString("IdentityDb");

/*********************************************************************
** Services
*/
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(dbConnectionString);
    if (builder.Environment.IsDevelopment())
    {
        options.EnableDetailedErrors();
        options.EnableSensitiveDataLogging();
        options.EnableThreadSafetyChecks();
    }
});
builder.Services.AddScoped<IDatabaseCreator, DatabaseCreator>();


var identityOptions = builder.Configuration.BindIdentityOptions("Identity:Options");
builder.Services.AddSingleton(Options.Create(identityOptions));
builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Lockout.CopyFrom(identityOptions.Lockout);
        options.Password.CopyFrom(identityOptions.Password);
        options.SignIn.RequireConfirmedAccount = true;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddHttpContextAccessor();
builder.Services.AddJsEngine();
builder.Services.AddWebOptimizer(pipeline =>
{
    pipeline.AddScssBundle("/css/site.css", "/css/site.scss");
    if (!builder.Environment.IsDevelopment())
    {
        pipeline.MinifyCssFiles();
        pipeline.AddFiles("text/css", "/css/*");
    }
});
builder.Services.AddRazorPages();

/*********************************************************************
** Initialization
*/
var app = builder.Build();

using (CancellationTokenSource cts = new(TimeSpan.FromMinutes(5)))
{
    using var scope = app.Services.CreateAsyncScope();
    var dbCreator = scope.ServiceProvider.GetRequiredService<IDatabaseCreator>();
    await dbCreator.InitializeDatabaseAsync(cts.Token);
}

/*********************************************************************
** HTTP Pipeline
*/
app.UseHttpsRedirection();
app.UseWebOptimizer();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();

/*********************************************************************
** Endpoint Mapping
*/
app.MapRazorPages();

/*********************************************************************
** Run the application
*/
app.Run();
