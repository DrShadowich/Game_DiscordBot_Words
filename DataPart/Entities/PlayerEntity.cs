using DiscordBot.Bot;
using DiscordBot.Game.PlayerItems;
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace DiscordBot.DataPart.Entities
{
    public class PlayerEntity
    {
        protected readonly bool _isAdmin = false;
        protected bool _isActive = false;
        [Key]
        public Guid PlayerId { get; set; }
        public DiscordUser User { get; set; }
        public Stats Stats { get; set; } = new();
        public bool Admin { get { return _isAdmin; } }
        public bool Active { get { return _isActive; } }
        public PlayerEntity() 
        {
            User = MainProgram.Client.CurrentUser; // Заглушка
        }
        public PlayerEntity(DiscordUser user)
        {
            if (IsAdmin(user.Id)) _isAdmin = true;
            User = user;
        }

        public void StartedGame() => _isActive = true;
        public static bool IsAdmin(ulong Id) => Id == "(ulong) YOUR DISCORD ID HERE";
        public static string AdminName(DiscordUser? user) => IsAdmin(user is not null ? user.Id : 0) ? "Мой повелитель" : user is not null ? user.Username : "\0";
        public PlayerEntity WithUser(DiscordUser user)
        {
            User = user;
            return this;
        }
        public PlayerEntity WithStats(Stats stats) 
        {
            Stats = stats;
            return this;
        }
    }
}
