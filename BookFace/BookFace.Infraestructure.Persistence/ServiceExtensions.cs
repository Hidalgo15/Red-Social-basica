using BookFace.Core.Application.Interfaces.InterfacesService;
using BookFace.Core.Domain.InterfacesRepository;
using BookFace.Core.Domain.IRepository;
using BookFace.Infraestructure.Persistence.DBContext;
using BookFace.Infraestructure.Persistence.Repositories;
using BookFace.Infraestructure.Persistence.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BookFace.Infraestructure.Persistence
{
    public static class ServiceExtensions
    {
        public static void AddPersistenceLayer(this IServiceCollection services, IConfiguration configuration)
        {
            #region DatabaseConfiguration
            if (configuration.GetValue<bool>("UseDataBaseInMemory"))
            {
                services.AddDbContext<ApplicationContext>(options => options.UseInMemoryDatabase("Testing"));
            }
            else
            {
                var ConnectionString = configuration.GetConnectionString("AppConnection");
                services.AddDbContext<ApplicationContext>(options =>
                options.UseSqlServer(ConnectionString, m => m.MigrationsAssembly(typeof(ApplicationContext).Assembly.FullName)));
            }
            #endregion


            #region RepositoryService 
            services.AddScoped(typeof(IRepository<>), typeof(RepositoryService<>));
            services.AddScoped<IPublicacionRepository, PublicacionRepository>();
            services.AddScoped<IComentarioRepository, ComentarioRepository>();
            services.AddScoped<IReaccionRepository, ReaccionRepository>();
            services.AddScoped<IAmistadRepository, AmistadRepository>();
            services.AddScoped<ISolicitudAmistadRepository, SolicitudAmistadRepository>();
            #endregion
        }
    }
}
