using Convidad.TechnicalTest.Services;

namespace Convidad.TechnicalTest.API.Extensions
{
    public static class ServicesExtension
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IChildrenService, ChildrenService>();
            services.AddScoped<IWishlistService, WishlistService>();
            services.AddScoped<IDeliveriesService, DeliveriesService>();
            services.AddScoped<IReindeersService, ReindeersService>();
            services.AddScoped<IRouteReindeerService, RouteReindeerService>();
            services.AddScoped<IRoutesService, RoutesService>();

            return services;
        }
    }
}
