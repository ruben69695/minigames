using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minigames.Core;

namespace Minigames.Infrastructure.Configurations;

public class UserGameDataConfiguration : IEntityTypeConfiguration<UserGameData>
{
    public void Configure(EntityTypeBuilder<UserGameData> builder)
    {
        builder.HasKey(k => k.UserId);

        builder.HasOne(g => g.User)
            .WithOne();
    }
}