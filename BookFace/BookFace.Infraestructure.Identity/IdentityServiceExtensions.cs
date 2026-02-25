using BookFace.Core.Application.AutoMapper;
using BookFace.Core.Application.Identity.Interfaces;
using BookFace.Infraestructure.Identity.Entities;
using BookFace.Infraestructure.Identity.Identity;
using BookFace.Infraestructure.Identity.Seed;
using BookFace.Infraestructure.Identity.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BookFace.Infraestructure.Identity;

public static class IdentityServiceExtensions
{
    public static void AddIdentityLayer(this IServiceCollection services, IConfiguration configuration)
    {

        // Configura el contexto de Identity (ajusta el nombre del contexto y la cadena de conexión)
        services.AddDbContext<IdentityDBContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("AppConnection")));

        // Registra Identity
        services.AddIdentity<ApplicationUser, IdentityRole<int>>()
            .AddEntityFrameworkStores<IdentityDBContext>()
            .AddDefaultTokenProviders();

        // Registra tu servicio personalizado
        services.AddScoped<IIdentityService, IdentityService>();

        services.AddAutoMapper(typeof(IdentityProfile).Assembly);

    }

    public static async Task RunIdentitySeedAsync(this IServiceProvider service)
    {
        using var scope = service.CreateScope();
        var servicesProvider = scope.ServiceProvider;

        var userManager = servicesProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = servicesProvider.GetRequiredService<RoleManager<IdentityRole>>();

        await DefaultUsser.SeedAsync(userManager);
    }

    
}
