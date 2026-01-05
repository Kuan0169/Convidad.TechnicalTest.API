using Convidad.TechnicalTest.Data.Context;
using Convidad.TechnicalTest.Services.SantaService;
using Convidad.TechnicalTest.Data.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using Xunit;

namespace Convidad.TechnicalTest.Tests.Services
{
    public class ReindeerServiceTest
    {
        public SantaDbContext CreateInMemoryDbContext()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            var options = new DbContextOptionsBuilder<SantaDbContext>()
                .UseSqlite(connection)
                .Options;
            var context = new SantaDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }

    }
}
