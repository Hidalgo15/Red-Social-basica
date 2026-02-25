using BookFace.Core.Application;
using BookFace.Infraestructure.Identity;
using BookFace.Infraestructure.Persistence;
using BookFace.Infraestructure.Shared;

namespace BookFace
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews()

              .AddJsonOptions(options =>
               {
                   options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
                   options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles; // Si tienes referencias circulares
               });

            builder.Services.AddSession();


            builder.Services.AddIdentityLayer(builder.Configuration);
            builder.Services.AddPersistenceLayer(builder.Configuration);
            builder.Services.AddApplicationLayer(builder.Configuration);
            builder.Services.AddSharedLayerIoc(builder.Configuration);
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();


            builder.Services.AddSingleton<IHttpContextAccessor , HttpContextAccessor>();
            var app = builder.Build();

            app.Services.RunIdentitySeedAsync();


            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
