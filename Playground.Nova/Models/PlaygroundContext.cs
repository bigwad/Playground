using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Playground.Nova.Models
{
    public partial class PlaygroundContext : DbContext
    {
        public PlaygroundContext()
        {
        }

        public PlaygroundContext(DbContextOptions<PlaygroundContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Item> Items { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                throw new InvalidOperationException($"DbContext is not configured. Use DbContextOptionsBuilder.UseSqlServer method to configure connection.");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.1-servicing-10028");
        }
    }
}
