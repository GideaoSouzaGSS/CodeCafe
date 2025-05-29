using Microsoft.EntityFrameworkCore;

namespace CodeCafe.Data;
public class EventStoreDbContext : DbContext
{
    public DbSet<CodeCafe.Data.Store.EventData> Events { get; set; }

    public EventStoreDbContext(DbContextOptions<EventStoreDbContext> options) : base(options) { }
}