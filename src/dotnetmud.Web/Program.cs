/*********************************************************************
** Configuration
*/
var builder = WebApplication.CreateBuilder(args);

/*********************************************************************
** Configuration
*/

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
** Configuration
*/
var app = builder.Build();

/*********************************************************************
** Configuration
*/
app.UseHttpsRedirection();
app.UseWebOptimizer();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();

/*********************************************************************
** Configuration
*/
app.MapRazorPages();

/*********************************************************************
** Configuration
*/
app.Run();
