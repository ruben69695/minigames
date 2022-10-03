using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Minigames.Application.Common.Interfaces;
using Minigames.Core;
using Minigames.Core.Repositories;
using Minigames.Infrastructure;
using Minigames.Infrastructure.Repositories;
using Minigames.Infrastructure.Services;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration config)
    {
        services
            .AddTransient<IUserGameDataRepository, UserGameDataRepository>();

        services
            .AddTransient<IIdentityService, IdentityService>();

        services
            .AddSingleton<IUserClaimsFactory<User>, UserClaimsFactory>()
            .AddSingleton<IAuthorizationService, AuthorizationService>();

        // Add logging
        services
            .AddLogging(loggingBuilder => loggingBuilder.AddSeq(config.GetSection("Seq")));

        // Add auth
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = config["Jwt:Issuer"],
                    ValidAudience = config["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]))
                };
            });

        // Add db
        services
            .AddDbContext<MinigamesContext>(options => options.UseNpgsql(config["DbConnectionString"]))
            .AddIdentityCore<User>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<MinigamesContext>();

        return services;
    }
}
