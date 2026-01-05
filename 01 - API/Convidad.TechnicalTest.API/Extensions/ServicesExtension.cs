namespace Convidad.TechnicalTest.API.Extensions
{
    public static class ServicesExtension
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<Services.SantaService.ISantaService, Services.SantaService.SantaService>();
            return services;
        }
    }
}
