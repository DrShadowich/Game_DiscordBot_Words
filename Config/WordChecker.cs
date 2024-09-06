using DiscordBot.Game;

namespace DiscordBot.Config
{
    public static class WordChecker
    {

        public static async Task<bool> IsWordAsync(string undebuggedWord)
        {
            string word = DebugString.GetRightString(undebuggedWord);
            string[] words = Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "DataWord"));
            foreach(string fileName in words)
            {
                using StreamReader fileReader = new(fileName);
                string lines = await fileReader.ReadToEndAsync();
                foreach (string line in lines.Split("\n"))
                {
                    if (line == word) return true;
                }
            }
            return false;
        }
    }
}
