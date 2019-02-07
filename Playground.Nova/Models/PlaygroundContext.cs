using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Playground.Nova.Models
{
    public partial class PlaygroundContext : DbContext
    {
        public PlaygroundContext()
        {
            Database.SetCommandTimeout(TimeSpan.FromMinutes(1));
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
                optionsBuilder.UseSqlServer("data source=localhost;initial catalog=Playground;persist security info=True;user id=sa;password=*****;Connection Timeout=30;Min Pool Size=30;Max Pool Size=512;", x =>
                {
                    x.EnableRetryOnFailure();
                    x.CommandTimeout(60);
                });
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.1-servicing-10028");
        }
    }
}
