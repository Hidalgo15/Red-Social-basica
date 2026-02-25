using BookFace.Infraestructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace BookFace.Infraestructure.Identity.Seed
{
    public static class DefaultUsser
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager)
        {
            ApplicationUser user = new()
            {
                Nombre = "Emmanuel",
                Apellido = "Perez",
                Email = "emmanuelperezramirez15@gmail.com",
                EmailConfirmed = true,
                UserName = "Emma15",
                
            };

            if (await userManager.Users.AllAsync(u => u.Id != user.Id))
            {
                var entityUser = await userManager.FindByEmailAsync(user.Email);
                if (entityUser == null)
                {
                    await userManager.CreateAsync(user, "A123456b+");
                    
                }
            }

        }
    }
}
