using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CodeCafe.Domain.Entities;

public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        builder.ToTable("Conversations");
        
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.User1Id)
            .IsRequired();
            
        builder.Property(c => c.User2Id)
            .IsRequired();
            
        builder.Property(c => c.LastMessageAt)
            .IsRequired();
            
        builder.HasIndex(c => new { c.User1Id, c.User2Id });
        builder.HasIndex(c => new { c.User2Id, c.User1Id });
    }
}