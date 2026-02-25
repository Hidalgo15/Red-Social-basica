using BookFace.Core.Application.Interfaces.InterfacesService;
using BookFace.Core.Domain.Settings;
using BookFace.Infraestructure.Shared.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BookFace.Infraestructure.Shared
{
    public static class ServicesRegistration
    {
        //Extension method - Decorator pattern
        public static void AddSharedLayerIoc(this IServiceCollection services, IConfiguration config)
        {
            #region Configurations
            services.Configure<MailSettings>(config.GetSection("MailSettings"));
            #endregion

            #region Services IOC
            services.AddScoped<IEmailService, EmailService>();
            #endregion
        }
    }
}
