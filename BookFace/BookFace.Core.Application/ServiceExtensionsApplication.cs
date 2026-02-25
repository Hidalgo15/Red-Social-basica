using BookFace.Core.Application.AutoMapper;
using BookFace.Core.Application.Interfaces.InterfacesService;
using BookFace.Core.Application.Service.EntidadService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BookFace.Core.Application
{
    public static class ServiceExtensionsApplication
    {
        public static void AddApplicationLayer(this IServiceCollection services, IConfiguration configuration)
        {

        #region AutoMapper
        services.AddAutoMapper(typeof(UsuarioProfile).Assembly);
        services.AddAutoMapper(typeof(PublicacionProfile).Assembly);
        services.AddAutoMapper(typeof(ComentarioProfile).Assembly);
        services.AddAutoMapper(typeof(AmistadProfile).Assembly);
        services.AddAutoMapper(typeof(SolicitudAmistadProfile).Assembly);
        services.AddAutoMapper(typeof(ReaccionProfile).Assembly);
        services.AddAutoMapper(typeof(UsuarioProfile).Assembly);
            #endregion


            #region Service
        services.AddScoped<IAmistadService, AmistadService>();
      //  services.AddScoped<IUsuarioService, UsuarioService>();
        services.AddScoped<IPublicacionService, PublicacionService>();
        services.AddScoped<IComentarioService, ComentarioService>();
        services.AddScoped<ISolicitudAmistadService, SolicitudAmistadService>();
        services.AddScoped<IReaccionService, ReaccionService>();
            #endregion
        }
    }
}
