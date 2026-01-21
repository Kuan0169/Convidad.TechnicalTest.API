using Convidad.TechnicalTest.API.Extensions;
using Convidad.TechnicalTest.API.Middlewares;
using Convidad.TechnicalTest.Data.Context;
using Convidad.TechnicalTest.Data.Context.Initializer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

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
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.ValueLengthLimit = 1024 * 1024;
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SantaDbContext>();
    await db.Database.EnsureCreatedAsync();
    await DbInitializer.InitializeDatabase(db);
}

app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    await next();
});

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseMiddleware<RequestTiming>();
app.UseMiddleware<GlobalExceptionHandler>();

app.UseRouting();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
