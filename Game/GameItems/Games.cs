using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Game.GameItems
{
    public class GamesContainer
    {
        private Dictionary<long, GameManager> _games = [];
        public ICollection<GameManager> PlayingGames { get { return _games.Values; } }
        public bool FindGame(long byId, out GameManager game)
        {
            if (_games.TryGetValue(byId, out GameManager? thisGame) == false)
            {
                game = null!;
                return false;
            }
            if (thisGame == null) throw new Exception("> GameContainer: Игра была равна null, это неприемлимо!");
            game = thisGame;
            return true;
        }
        public void AddGame(long byId, GameManager game)
        {
            if (_games.TryAdd(byId, game)) Console.WriteLine("> GameContainer: Появилась новая сессия.");
            else Console.WriteLine("> GameContainer: Произошла четная попытка появления новой сессии!");
        }
        public void RemoveGame(long byId)
        {
            if (_games.Remove(byId)) Console.WriteLine("> GameContainer: Игровая сессия успешно удалена.");
            else Console.WriteLine("> GameContainer: Произошла четная попытка удаления игровой сессии.");
        }
    }
}
