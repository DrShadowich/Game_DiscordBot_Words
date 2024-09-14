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
        public DiscordUser? User { get; set; }
        public Stats? Stats { get; set; }
        public bool Admin { get { return _isAdmin; } }
        public bool Active { get { return _isActive; } }
        public PlayerEntity() { }
        public PlayerEntity(DiscordUser user)
        {
            if (IsAdmin(user.Id)) _isAdmin = true;
            User = user;
        }

        public void StartedGame() => _isActive = true;
        public static bool IsAdmin(ulong Id) => Id == 780823809610481694;
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
