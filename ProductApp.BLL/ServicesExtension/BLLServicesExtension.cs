using Microsoft.Extensions.DependencyInjection;

namespace ProductApp.BLL
{
    public static class BLLServicesExtension
    {
        public static void AddBLLServices(this IServiceCollection services)
        {
            services.AddScoped<ICategoryManager, CategoryManager>();
            services.AddScoped<IProductManager, ProductManager>();
            services.AddScoped<IAccountManager, AccountManager>();
            services.AddScoped<ICartManager, CartManager>();
            services.AddScoped<IOrderManager, OrderManager>();
            services.AddScoped<IImageManager, ImageManager>();
        }
    }
}
