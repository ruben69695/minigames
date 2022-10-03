using System.Reflection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Minigames.Core;

namespace Minigames.Infrastructure;

public class MinigamesContext : IdentityDbContext<User>
{
    public DbSet<UserGameData>? UserGameData { get; set; }

    public MinigamesContext(DbContextOptions<MinigamesContext> options) : base(options)
    {}

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Get and apply configurations from the current assembly
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
