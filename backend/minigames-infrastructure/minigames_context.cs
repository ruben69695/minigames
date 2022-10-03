using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Minigames.Core;

namespace Minigames.Infrastructure;

public class MinigamesContext : IdentityDbContext<User>
{
    public DbSet<UserGameData>? UserGameData { get; set; }

    public MinigamesContext(DbContextOptions<MinigamesContext> options) : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Get and apply configurations from the current assembly
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public async static Task Migrate(IServiceProvider provider)
    {
        using (var serviceScope = provider.GetService<IServiceScopeFactory>()!.CreateScope())
        {
            var ctx = serviceScope.ServiceProvider.GetRequiredService<MinigamesContext>();
            ctx.Database.Migrate();

            var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var logger = serviceScope.ServiceProvider.GetRequiredService<ILogger<MinigamesContext>>();

            if (!userManager.Users.Any())
            {
                logger.LogInformation("Creating test user");

                var newUser = new User
                {
                    Email = "ruben.arre6@gmail.com",
                    UserName = "skritlax12"
                };

                await userManager.CreateAsync(newUser, "P@ss.W0rd");
                await roleManager.CreateAsync(new IdentityRole
                {
                    Name = "admin"
                });
                await roleManager.CreateAsync(new IdentityRole
                {
                    Name = "user"
                });

                await userManager.AddToRoleAsync(newUser, "admin");
            }
        }
    }
}