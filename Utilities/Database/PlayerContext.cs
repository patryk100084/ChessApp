using ChessApp.Game;
using Microsoft.EntityFrameworkCore;

namespace ChessApp.Utilities.Database
{
    public class PlayerContext : DbContext
    {
        public PlayerContext() : base()
        {   }

        public DbSet<Player> players { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Player>().HasIndex(p => p.username).IsUnique();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source =.; Integrated Security = True");
        }

    }
}
