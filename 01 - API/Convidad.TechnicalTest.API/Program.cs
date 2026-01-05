using Convidad.TechnicalTest.API.Extensions;
using Convidad.TechnicalTest.Data.Context;
using Convidad.TechnicalTest.Data.Context.Initializer;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SantaDbContext>();

    await db.Database.EnsureCreatedAsync();
    await DbInitializer.InitializeDatabase(db);
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
