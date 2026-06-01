using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ProductApp.DAL
{
    public static class DALServicesExtension
    {
        public static void AddDALServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("ProductSystem");
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });


            services.AddScoped<ICategoryRepositories, CategoryRepositories>();
            services.AddScoped<IProductRepositories, ProductRepositories>();
            services.AddScoped<ICartRepositories, CartRepositories>();
            services.AddScoped<IOrderRepositories, OrderRepositories>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}
