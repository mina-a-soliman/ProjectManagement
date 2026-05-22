using ProjectManagement.API.Extensions;
using ProjectManagement.Application;
using ProjectManagement.Infrastructure;
using ProjectManagement.Infrastructure.Seed;


var builder = WebApplication.CreateBuilder(args);


 #region 
 builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddSwaggerServices();
    
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

 #endregion

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Project Management API v1");
        options.RoutePrefix = "swagger";
    });
}
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// app.UseHttpsRedirection();
 app.MapControllers();


// ── Seed Database ────────────────────────────────────────────────
await DatabaseSeeder.SeedAsync(app.Services);
if(app.Environment.IsDevelopment())
{
    app.MapGet("/",async context =>
    {
        context.Response.Redirect("/swagger");
    });
}
app.Run();
