using DiscordBot.Game;
using DSharpPlus.Entities;

namespace DiscordBot.Bot
{
    public static class Embeds
    {
        public static DiscordEmbedBuilder message = new() { Color = DiscordColor.Red };
        public static DiscordEmbed DuelEmbed(Player firstPlayer, Player secondPlayer)
        {
            message.Title = $"{(Player.IsAdmin(firstPlayer.User.Id) ? "Мой повелитель" : $"{firstPlayer.User.Username}")}\nвызвал на дуэль в слова\n{(Player.IsAdmin(secondPlayer.User.Id) ? "Моего повелителя" : $"{secondPlayer.User.Username}")}";
            message.Description = $"{secondPlayer.User.Mention}, вы можете принять игру, написав команду /acceptwords";
            return message.Build();
        }
        public static DiscordEmbed AcceptDuelEmbed(KeyValuePair<Player, Player> players)
        {
            message.Title = $"{(players.Value.Admin ? "Мой повелитель" : $"{players.Value.User.Username}")} принял вызов против {(players.Key.Admin ? "Моего повелителя" : $"{players.Key.User.Username}")}";
            message.Description = $"Игра начинается с {players.Key.User.Mention}." +
                    $"\nЧтобы начать играть просто напишите слово, а Я проверю или исправлю синтаксис вашего сообщения." +
                    $"\nЧтобы закончить игру досрочно, напишите команду /gamestop и причину окончания игры." +
                    $"\nИгра может сама закончиться, если вы не будете писать сообщение втечение 5 минут." +
                    $"\nЧтобы посмотреть информацию о нынешней сессии, просто напишите /gameinfo." +
                    $"\nЧтобы написать какое-то сообщение вне контекста сессии, то в начале сообщения напишите \'*\'." +
                    $"\nУдачной игры!";
            return message.Build();
        }
        public static DiscordEmbed GamesInfoEmbed(ulong GuildId)
        {
            string description = string.Empty;
            foreach (var i in MainProgram.Games)
            {
                if (i.Value.PlayChannel.GuildId == GuildId)
                {
                    description += $"Игра {i.Value.FirstPlayer.User.Mention} vs {i.Value.SecondPlayer.User.Mention}. ID = {i.Key}.\n";
                }
            }
            message.Title = "Текущие сесси";
            message.Description = description;
            return message.Build();
        }
        public static DiscordEmbed GameInfoEmbed(GameManager thisGame)
        {
            int totalMinutes = (DateTime.Now.Hour * 60 + DateTime.Now.Minute) - (thisGame.GameStarted.Hour * 60 + thisGame.GameStarted.Minute);
            int hours = totalMinutes / 60;
            int minutes = totalMinutes % 60;
            message.Title = $"Информация об игре {thisGame.SecondPlayer.User.Username} vs {thisGame.SecondPlayer.User.Username}";
            message.Description = $"Игра {thisGame.FirstPlayer.User.Mention} против {thisGame.SecondPlayer.User.Mention}." +
                $"\nИгровая сессия уже длится {hours} часов {minutes} минут" +
                $"\nВсего слов использовано за сессию: {thisGame.TotalWordsCount}" +
                $"\nНа данный момент ожидается буква: {thisGame.LastWord}" +
                $"\nПоследнее использованное слово: {thisGame.LastString}" +
                $"\nНа данный момент ожидается ход от {(thisGame.TurnOfFirst ? $"{thisGame.FirstPlayer.User.Mention}" : $"{thisGame.SecondPlayer.User.Mention}")}";
            return message.Build();
        }
        public static DiscordEmbed GameStopEmbed(GameManager thisGame)
        {
            message.Title = "Игра окончена!";
            message.Description = $"Игра была завершена по причине: {thisGame.StopReason}";
            return message.Build();
        }
        public static DiscordEmbed ErrorEmbed(DiscordUser sender)
        {
            message.Title = "Ошибка";
            message.Description = $"{(Player.IsAdmin(sender.Id) ? "Мой повелитель" : sender.Mention)}, это слово уже было использовано в данной сессии. Подберите что-то другое.";
            return message.Build();
        }
        public static DiscordEmbed DuelStop(DiscordUser sender)
        {
            message.Title = "Вызов отменен";
            message.Description = $"{sender.Mention} отказался от вызова";
            return message.Build();
        }
    }
}