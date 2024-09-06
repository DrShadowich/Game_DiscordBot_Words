using DSharpPlus.Entities;

namespace DiscordBot.Game
{
    public class Player
    {
        private readonly DiscordUser _user = null!;
        private readonly bool _isAdmin = false;
        private bool _isActive = false;
        public DiscordUser User { get { return _user; } }
        public bool Admin { get { return _isAdmin; } }
        public bool Active { get { return _isActive; } }
        public ulong Id { get { return _user.Id; } }
        public Player(DiscordUser user)
        {
            if (IsAdmin(user.Id)) _isAdmin = true;
            _user = user;
        }

        public void StartedGame() => _isActive = true;

        public static bool IsAdmin(ulong Id) => Id == 780823809610481694;
    }
}
