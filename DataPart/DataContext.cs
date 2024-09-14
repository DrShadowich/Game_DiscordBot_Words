using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordBot.DataPart.Entities;
using DSharpPlus.Entities;
using DiscordBot.Game.PlayerItems;

namespace DiscordBot.DataPart
{
    public class DataContext : DbContext
    {
        public DbSet<PlayerEntity> Players { get; set; }
        public DbSet<DiscordUser> DiscordUser { get; set; }
        public DbSet<Stats> PlayersStats { get; set; }
        public DataContext()
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
            Console.WriteLine("Успешно подключились!");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PlayerEntity>().HasOne(p => p.User);
            modelBuilder.Entity<PlayerEntity>().HasOne(p => p.Stats);
            modelBuilder.Entity<DiscordUser>().HasKey(u => u.Id);
            modelBuilder.Entity<PlayerEntity>().HasKey(p => p.PlayerId);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("host=localhost;port=5432;database=DiscordBot;username=postgres;password=yamamoto1");
            optionsBuilder.EnableSensitiveDataLogging();
        }
    }
}
