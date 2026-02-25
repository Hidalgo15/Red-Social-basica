using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace BookFace.Infraestructure.Identity.Identity
{
    public class IdentityDBContextFactory : IDesignTimeDbContextFactory<IdentityDBContext>
    {
        public IdentityDBContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<IdentityDBContext>();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("AppConnection"));

            return new IdentityDBContext(optionsBuilder.Options);
        }
    }
}