using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Minigames.Core;
using Minigames.Core.Repositories;
using Minigames.Infrastructure.Repositories;

namespace Minigames.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        Console.WriteLine(config["DbConnectionString"]);

        services
            .AddTransient<IUserGameDataRepository, UserGameDataRepository>();

        services
            .AddDbContext<MinigamesContext>(options => options.UseNpgsql(config["DbConnectionString"]))
            .AddIdentityCore<User>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<MinigamesContext>();

        return services;
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
