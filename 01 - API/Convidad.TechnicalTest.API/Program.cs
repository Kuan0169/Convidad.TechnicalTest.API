using Convidad.TechnicalTest.API.Extensions;
using Convidad.TechnicalTest.Data.Context;
using Convidad.TechnicalTest.Data.Context.Initializer;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Convidad.TechnicalTest.API.Middlewares;

var builder = WebApplication.CreateBuilder(args);

var connection = new SqliteConnection("DataSource=:memory:");
connection.Open();

builder.Services.AddSingleton(connection);

builder.Services.AddDbContext<SantaDbContext>((sp, opt) =>
{
    var conn = sp.GetRequiredService<SqliteConnection>();
    opt.UseSqlite(conn);
});

builder.Services.AddServices();
builder.Services.AddControllers();

builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.ValueLengthLimit = 1024 * 1024;
});

builder.Services.AddOpenApi();

var app = builder.Build();

app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    await next();
});

app.UseMiddleware<GlobalExceptionHandler>();

app.UseMiddleware<RequestTiming>();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SantaDbContext>();

    await db.Database.EnsureCreatedAsync();
    await DbInitializer.InitializeDatabase(db);
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseRouting();

app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();

app.Run();
