using System.ComponentModel.DataAnnotations;

namespace DiscordBot.Game.PlayerItems
{
    public class Stats
    {
        private Dictionary<string, int> _countWords = [];
        private Dictionary<string, int> _stats = [];
        public Guid Id { get; set; }
        public string?[] GetAllStats()
        {
            List<string> theWholeStats = [];
            foreach(var i in _stats)
            {
                theWholeStats.Add($"{i.Key}: {i.Value}.");
            }
            foreach(var i in _countWords)
            {
                theWholeStats.Add($"Сказано {i.Key}: {i.Value} раз.");
            }
            return [.. theWholeStats];
        }
        public void AddStatPara(string statName, int defaultStat) 
        {
            Console.WriteLine(_stats.TryAdd(statName, defaultStat) ? "\nДобавили статистику\n" : $"\nУвеличили статистику {statName}\n");
        } 
        public void AddWordCount(string word)
        {
            if (_countWords.TryGetValue(word, out int value)) _countWords[word] = ++value;
            else _countWords.Add(word, 1);
            return;
        }
    }
}
