using Convidad.TechnicalTest.API.DTOs.Error;
using Convidad.TechnicalTest.Data.Context;
using Convidad.TechnicalTest.Data.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Text.Json;
using Xunit;

namespace Convidad.TechnicalTest.Tests.Integration
{
    public class DeliveryControllerIntegrationTest :
        IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public DeliveryControllerIntegrationTest(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Production");
            });
        }

        [Fact]
        public async Task GetReindeerById_NotFound_ReturnsNotFound()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/delivery/reindeers/00000000-0000-0000-0000-000000000000");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task AssignReindeer_ValidRequest_ReturnsNoContent()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<SantaDbContext>();

            var child = new Child { Name = "Test", CountryCode = "US" };
            var route = new Route { Name = "Test Route", Region = "North Pole" };
            var delivery = new Delivery { ChildId = child.Id, RouteId = route.Id };
            var reindeer = new Reindeer { Name = "Rudolph", PlateNumber = "XMAS-001", Weight = 100, Packets = 50 };

            dbContext.Children.Add(child);
            dbContext.Routes.Add(route);
            dbContext.Deliveries.Add(delivery);
            dbContext.Reindeers.Add(reindeer);
            await dbContext.SaveChangesAsync();

            var client = _factory.CreateClient();
            var requestContent = new StringContent(
                JsonSerializer.Serialize(new { reindeerId = reindeer.Id }),
                System.Text.Encoding.UTF8,
                "application/json");

            // Act
            var response = await client.PostAsync(
                $"/delivery/deliveries/{delivery.Id}/assign-reindeer",
                requestContent);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        private async Task InitializeDatabaseAsync()
        {
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<SantaDbContext>();
            await dbContext.Database.EnsureCreatedAsync();
            // 注意：整合測試通常不使用 DbInitializer，而是手動建立測試資料
        }
    }
}
