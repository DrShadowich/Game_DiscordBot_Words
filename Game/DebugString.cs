using System.Text;

namespace DiscordBot.Game
{
    public static class DebugString
    {
        private readonly static HashSet<char> _banChars =
            [
                '.', ',', ':', ';', '`', '\'', '|', '\"', '<', '>',
                '(', ')', '{', '}', '[', ']', '!', '@', '#', '$', '%',
                '^', '&', '*', '1', '2', '3', '4', '5', '6', '7', '8', '9', '~', ' ' ];
        public static char GetFirstWord(string undebuggedWord)
        {
            if (_banChars.Contains(undebuggedWord.ToLower()[0])) return GetFirstWord(undebuggedWord.Remove(0));
            else return undebuggedWord[0];
        }
        public static string GetRightString(string message)
        {
            StringBuilder strBuild = new();
            char[] chars = message.ToCharArray();
            for(int i = 0; i < chars.Length; ++i)
            {
                if(_banChars.Contains(chars[i]) == false) strBuild.Append(chars[i] == '_' ? chars[' '] : chars[i]);
            }
            return strBuild.ToString().Split(" ")[0].ToLower();
        }

        public static char GetLastWord(string undebuggedWord)
        {
            if (_banChars.Contains(undebuggedWord.ToLower()[^1])) return GetLastWord(undebuggedWord.Remove(undebuggedWord.Length - 1));
            else return undebuggedWord[^1];
        }
    }
}
