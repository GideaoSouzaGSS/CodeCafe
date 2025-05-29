using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CodeCafe.Domain.Entities;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable("Messages");
        
        builder.HasKey(m => m.Id);
        
        builder.Property(m => m.Content)
            .IsRequired()
            .HasMaxLength(2000);
            
        builder.Property(m => m.SentAt)
            .IsRequired();
            
        builder.Property(m => m.IsRead)
            .IsRequired()
            .HasDefaultValue(false);
            
        builder.HasOne(m => m.Conversation)
            .WithMany(c => c.Messages)
            .HasForeignKey(m => m.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasIndex(m => new { m.SenderId, m.RecipientId });
        builder.HasIndex(m => new { m.RecipientId, m.SenderId });
        builder.HasIndex(m => m.SentAt);
    }
}