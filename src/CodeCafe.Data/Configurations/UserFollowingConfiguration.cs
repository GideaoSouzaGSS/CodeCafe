using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CodeCafe.Domain.Entities;

namespace CodeCafe.Data.Configurations;

public class UserFollowingConfiguration : IEntityTypeConfiguration<UserFollowing>
{
    public void Configure(EntityTypeBuilder<UserFollowing> builder)
    {
        builder.ToTable("UserFollowings");
        
        builder.HasKey(f => new { f.FollowerId, f.FollowedId });
        
        builder.Property(f => f.FollowedAt)
            .IsRequired();
    }
}